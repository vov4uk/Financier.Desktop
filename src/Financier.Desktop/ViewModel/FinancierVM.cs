using Financier.DataAccess;
using Financier.DataAccess.Abstractions;
using Financier.DataAccess.Data;
using Financier.DataAccess.Monobank;
using Financier.DataAccess.View;
using Financier.Desktop.Converters;
using Financier.Desktop.Entities;
using FinancistoAdapter;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Financier.Desktop.ViewModel
{
    public class FinancierVM : BindableBase
    {
        FinancierDatabase db;
        private List<Entity> keyLessEntities = new List<Entity>();
        private AccountsVM accountsVM;
        private BlotterVM blotterVM;

        private BindableBase currentPage;
        public BindableBase CurrentPage
        {
            get { return currentPage; }
            set
            {
                SetProperty(ref currentPage, value);
                RaisePropertyChanged(nameof(CurrentPage));
            }
        }

        private string openBackupPath;
        public string OpenBackupPath
        {
            get { return openBackupPath; }
            set { SetProperty(ref openBackupPath, value); }
        }

        private string saveBackupPath;
        public string SaveBackupPath
        {
            get { return saveBackupPath; }
            set { SetProperty(ref saveBackupPath, value); }
        }

        /// <summary>
        /// Returns the command which, when executed, cancels the order 
        /// and causes the Wizard to be removed from the user interface.
        /// </summary>
        private DelegateCommand<Type> _menuNavigateCommand;
        public DelegateCommand<Type> MenuNavigateCommand
        {
            get
            {
                if (_menuNavigateCommand == null)
                    _menuNavigateCommand = new DelegateCommand<Type>(NavigateToType);

                return _menuNavigateCommand;
            }
        }

        void NavigateToType(Type type)
        {
            CurrentPage = Pages.FirstOrDefault(x => x.GetType().BaseType.GetGenericArguments().Single() == type);
        }

        private ReadOnlyCollection<BindableBase> _pages;
        public ReadOnlyCollection<BindableBase> Pages
        {
            get
            {
                return _pages;
            }
        }

        void CreatePages()
        {
            accountsVM = new AccountsVM();
            if (blotterVM != null) { blotterVM.OpenTransactionRaised -= BlotterVM_OpenTransactionRaised; }
            blotterVM = new BlotterVM();
            blotterVM.OpenTransactionRaised += BlotterVM_OpenTransactionRaised;
            _pages = new List<BindableBase>
                {
                    accountsVM,
                    blotterVM,
                    new BudgetsVM(),
                    new CategoriesVM(),
                    new CurrenciesVM(),
                    new ExchangeRatesVM(),
                    new LocationsVM(),
                    new PayeesVM(),
                    new ProjectsVM()
                }.AsReadOnly();
        }

        private async void BlotterVM_OpenTransactionRaised(object sender, int e)
        {
            TransactionVM context;
            using (var uow = db.CreateUnitOfWork())
            {
                var transaction = await uow.GetRepository<BlotterTransactions>().FindByAsync(x => x._id == e, x => x.original_currency, x => x.category);
                var subTransactions = await uow.GetRepository<Transaction>().FindManyAsync(x => x.ParentId == e, o => o.OriginalCurrency, c => c.Category);

                var transactionVM = ConvertTransaction(transaction);
                if (subTransactions?.Any() == true)
                {
                    var sub = subTransactions.Select(x => ConvertTransaction(x));
                    transactionVM.SubTransactions = new ObservableCollection<TransactionVM>(sub);
                }
                context = transactionVM ?? new TransactionVM();

                var allAccounts = await uow.GetRepository<Account>().FindManyAsync(x => x.IsActive ,x => x.Currency);
                var allCategories = await uow.GetRepository<Category>().GetAllAsync();
                var allPayees = await uow.GetRepository<Payee>().FindManyAsync(x => x.IsActive);
                var currencies = await uow.GetRepository<Currency>().GetAllAsync();
                var locations = await uow.GetRepository<Location>().FindManyAsync(x => x.IsActive == true);
                var projects = await uow.GetRepository<Project>().FindManyAsync(x => x.IsActive == true);
                context.Accounts = new ObservableCollection<Account>(allAccounts);
                context.Categories = new ObservableCollection<Category>(allCategories);
                context.Payees = new ObservableCollection<Payee>(allPayees);
                context.Currencies = new ObservableCollection<Currency>(currencies);
                context.Locations = new ObservableCollection<Location>(locations);
                context.Projects = new ObservableCollection<Project>(projects);
            }
            var dialog = new Window();
            {
                dialog.Content = new TransactionControl() { DataContext = context };
                dialog.Height = 640;
                dialog.Width = 340;
                dialog.Show();
            }
        }

        private TransactionVM ConvertTransaction(BlotterTransactions transaction)
        {
            return new TransactionVM
            {
                Id = transaction._id,
                AccountId = transaction.from_account_id,
                CategoryId = transaction.category_id,
                Category = transaction.category,
                PayeeId = transaction.payee_id,
                CurrencyId = transaction.original_currency_id,
                Currency = transaction.original_currency,
                LocationId = transaction.location_id,
                ProjectId = transaction.project_id,
                Note = transaction.note,
                FromAmount = transaction.from_amount,
                OriginalFromAmount = transaction.original_from_amount,
                IsAmountNegative = transaction.from_amount < 0,
                Date = new DateTimeConverter().Convert(transaction.datetime),
            };
        }
        private TransactionVM ConvertTransaction(Transaction transaction)
        {
            return new TransactionVM
            {
                Id = transaction.Id,
                AccountId = transaction.FromAccountId,
                CategoryId = transaction.CategoryId,
                Category = transaction.Category,
                PayeeId = transaction.PayeeId,
                CurrencyId = transaction.OriginalCurrencyId,
                Currency = transaction.OriginalCurrency,
                LocationId = transaction.LocationId,
                ProjectId = transaction.ProjectId,
                Note = transaction.Note,
                FromAmount = transaction.FromAmount,
                OriginalFromAmount = transaction.OriginalFromAmount,
                IsAmountNegative = transaction.FromAmount < 0,
                Date = new DateTimeConverter().Convert(transaction.DateTime),
            };
        }

        public FinancierVM()
        {
            CreatePages();
        }

        public async Task OpenBackup(string backupPath)
        {
            var start = DateTime.Now;
            CreatePages();
            var sb = new StringBuilder();

            OpenBackupPath = backupPath;
            var entities = EntityReader.ParseBackupFile(backupPath).ToList();

            db = new FinancierDatabase();
            await db.Import(entities);

            using (var uow = db.CreateUnitOfWork())
            {
                var allAccounts = await uow.GetRepository<Account>().GetAllAsync(x => x.Currency);
                var allTransactions = await uow.GetRepository<BlotterTransactions>().GetAllAsync(x => x.from_account_currency, x => x.to_account_currency);
                var allCategories = await uow.GetRepository<Category>().GetAllAsync();
                var allRates = await uow.GetRepository<CurrencyExchangeRate>().GetAllAsync(x => x.FromCurrency, x => x.ToCurrency);

                AddEntities(allAccounts.OrderByDescending(x => x.IsActive).ThenBy(x => x.SortOrder).ToList(), sb);
                AddEntities(allTransactions.OrderByDescending(x => x.datetime).ToList(), sb, true);
                AddEntities(allCategories.Where(x => x.Id > 0).ToList(), sb);
                AddEntities(allRates.ToList(), sb);
            }
             AddEntities(entities.OfType<Project>().ToList(), sb);
             AddEntities(entities.OfType<Payee>().ToList(), sb);
             AddEntities(entities.OfType<Location>().ToList(), sb);
             AddEntities(entities.OfType<Currency>().ToList(), sb);
             AddEntities(entities.OfType<Budget>().ToList(), sb);

            keyLessEntities.Clear();

            AddKeylessEntities(entities.OfType<CCardClosingDate>().ToList(), sb);
            AddKeylessEntities(entities.OfType<CategoryAttribute>().ToList(), sb);
            AddKeylessEntities(entities.OfType<TransactionAttribute>().ToList(), sb);

            var duration = DateTime.Now - start;
            sb.AppendLine($"Duration : {duration}");
            var info = new InfoVM { Text = sb.ToString() };
            info.RequestClose += Info_RequestClose;
            CurrentPage = info;
        }

        private void Info_RequestClose(object sender, EventArgs e)
        {
            CurrentPage = Pages.OfType<AccountsVM>().First();
        }

        private void AddKeylessEntities<T>(List<T> entities, StringBuilder sb)
        where T:Entity
        {
            sb?.AppendLine($"Imported {entities.Count} {typeof(T).Name}");
            keyLessEntities.AddRange(entities);
        }
        private async Task AddBackupEntities<T>(IUnitOfWork uow, List<Entity> itemsToBackup, Func<T, bool> predicate = null, Func<T, int> keySelector = null)
        where T:Entity
        {
            var allItems = await uow.GetRepository<T>().GetAllAsync();
            if (predicate != null)
            {
                allItems = allItems.Where(predicate).ToList();
            }
            if (predicate != null)
            {
                allItems = allItems.OrderBy(keySelector).ToList();
            }
            itemsToBackup.AddRange(allItems);
        }

        private void AddEntities<T>(List<T> entities, StringBuilder sb, bool replace = false)
        where T:Entity
        {
            sb?.AppendLine($"Imported {entities.Count} {typeof(T).Name}");
            var vm = _pages.FirstOrDefault(x => x.GetType().BaseType.GetGenericArguments().Single() == typeof(T)) as EntityBaseVM<T>;
            if (replace)
            {
                vm.Entities = new RangeObservableCollection<T>(entities);
            }
            else
            {
                vm.Entities.AddRange(entities);
            }
        }

        public async Task SaveBackup(string backupPath)
        {
            SaveBackupPath = backupPath;
            List<Entity> itemsToBackup = new();
            itemsToBackup.AddRange(keyLessEntities);
            using(IUnitOfWork uow = db.CreateUnitOfWork())
            {
                await AddBackupEntities<Budget>(uow, itemsToBackup);
                await AddBackupEntities<TransactionAttribute>(uow, itemsToBackup);
                await AddBackupEntities<CurrencyExchangeRate>(uow, itemsToBackup);
                await AddBackupEntities<Currency>(uow, itemsToBackup);
                await AddBackupEntities<Location>(uow, itemsToBackup, x => x.Id > 0);
                await AddBackupEntities<Payee>(uow, itemsToBackup);
                await AddBackupEntities<Project>(uow, itemsToBackup);
                await AddBackupEntities<Transaction>(uow, itemsToBackup);
                await AddBackupEntities<Account>(uow, itemsToBackup, keySelector: x => x.Id);
                await AddBackupEntities<AttributeDefinition>(uow, itemsToBackup, x => x.Id > 0);
                await AddBackupEntities<CategoryAttribute>(uow, itemsToBackup);
                await AddBackupEntities<CCardClosingDate>(uow, itemsToBackup);
                await AddBackupEntities<SmsTemplate>(uow, itemsToBackup);
                await AddBackupEntities<Category>(uow, itemsToBackup, x => x.Id > 0);
            }

            using var bw = new BackupWriter(backupPath, EntityReader.BackupVersion);
            bw.GenerateBackup(itemsToBackup);
        }


        public async Task ImportMonoTransactions(int accountId, List<MonoTransaction> transactions)
        {
            await db.ImportMonoTransactions(accountId, transactions);

            await db.RebuildRunningBalanceForAccount(accountId);

            using (var uow = db.CreateUnitOfWork())
            {
                var allTransactions = await uow.GetRepository<BlotterTransactions>().GetAllAsync(x => x.from_account_currency, x => x.to_account_currency);
                AddEntities(allTransactions.OrderByDescending(x => x.datetime).ToList(), null, true);
            }
        }
    }
}
