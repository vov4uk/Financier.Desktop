using Financier.DataAccess;
using Financier.DataAccess.Data;
using Financier.DataAccess.Monobank;
using Financier.DataAccess.View;
using FinancistoAdapter;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Financier.Desktop.ViewModel
{
    public class FinancierVM : BindableBase
    {
        FinancierDatabase db;
        private List<Entity> keyLessEntities = new List<Entity>();

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
                    _menuNavigateCommand = new DelegateCommand<Type>(CancelOrder);

                return _menuNavigateCommand;
            }
        }

        void CancelOrder(Type type)
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
            _pages = new List<BindableBase>
                {
                    new AccountsVM(),
                    new BlotterVM(),
                    new BudgetsVM(),
                    new CategoriesVM(),
                    new CurrenciesVM(),
                    new ExchangeRatesVM(),
                    new LocationsVM(),
                    new PayeesVM(),
                    new ProjectsVM()
                }.AsReadOnly();
        }

        public FinancierVM()
        {
            CreatePages();
        }

        public async Task OpenBackup(string backupPath)
        {
            CreatePages();

            OpenBackupPath = backupPath;
            var entities = EntityReader.ParseBackupFile(backupPath).ToList();
            keyLessEntities.Clear();
            keyLessEntities.AddRange(entities.OfType<CCardClosingDate>());
            keyLessEntities.AddRange(entities.OfType<CategoryAttribute>());
            keyLessEntities.AddRange(entities.OfType<TransactionAttribute>());

            db = new FinancierDatabase();
            await db.Import(entities);

            using (var uow = db.CreateUnitOfWork())
            {
                var allAccounts = await uow.GetRepository<Account>().GetAllAsync(x => x.Currency);
                var allTransactions = await uow.GetRepository<BlotterTransactions>().GetAllAsync(x => x.from_account_currency, x => x.to_account_currency);
                var allCategories = await uow.GetRepository<Category>().GetAllAsync();
                var allRates = await uow.GetRepository<CurrencyExchangeRate>().GetAllAsync(x => x.FromCurrency, x => x.ToCurrency);
                _pages.OfType<AccountsVM>().First().Entities.AddRange(allAccounts.OrderByDescending(x => x.IsActive).ThenBy(x => x.SortOrder).ToList());
                _pages.OfType<BlotterVM>().First().Entities = new RangeObservableCollection<BlotterTransactions>(allTransactions.OrderByDescending(x => x.datetime).ToList());
                _pages.OfType<CategoriesVM>().First().Entities.AddRange(allCategories.ToList());
                _pages.OfType<ExchangeRatesVM>().First().Entities.AddRange(allRates.ToList());
            }

            _pages.OfType<ProjectsVM>().First().Entities.AddRange(entities.OfType<Project>().ToList());
            _pages.OfType<PayeesVM>().First().Entities.AddRange(entities.OfType<Payee>().ToList());
            _pages.OfType<LocationsVM>().First().Entities.AddRange(entities.OfType<Location>().ToList());
            _pages.OfType<CurrenciesVM>().First().Entities.AddRange(entities.OfType<Currency>().ToList()); 
            _pages.OfType<BudgetsVM>().First().Entities.AddRange(entities.OfType<Budget>().ToList());
        }

        public async Task SaveBackup(string backupPath)
        {
            SaveBackupPath = backupPath;
            List<Entity> itemsToBackup = new();
            itemsToBackup.AddRange(keyLessEntities);
            using (var uow = db.CreateUnitOfWork())
            {
                var Budget = await uow.GetRepository<Budget>().GetAllAsync();
                itemsToBackup.AddRange(Budget);
                var TransactionAttribute = await uow.GetRepository<TransactionAttribute>().GetAllAsync();
                itemsToBackup.AddRange(TransactionAttribute);
                var CurrencyExchangeRate = await uow.GetRepository<CurrencyExchangeRate>().GetAllAsync();
                itemsToBackup.AddRange(CurrencyExchangeRate);
                var Currency = await uow.GetRepository<Currency>().GetAllAsync();
                itemsToBackup.AddRange(Currency);
                var Location = await uow.GetRepository<Location>().GetAllAsync();
                itemsToBackup.AddRange(Location.Where(x => x.Id > 0));
                var Payee = await uow.GetRepository<Payee>().GetAllAsync();
                itemsToBackup.AddRange(Payee);
                var Project = await uow.GetRepository<Project>().GetAllAsync();
                itemsToBackup.AddRange(Project);
                var transactions = await uow.GetRepository<Transaction>().GetAllAsync();
                itemsToBackup.AddRange(transactions);
                var accounts = await uow.GetRepository<Account>().GetAllAsync();
                itemsToBackup.AddRange(accounts.OrderBy(x => x.Id));
                var AttributeDefinition = await uow.GetRepository<AttributeDefinition>().GetAllAsync();
                itemsToBackup.AddRange(AttributeDefinition.Where(x => x.Id > 0));
                var CategoryAttribute = await uow.GetRepository<CategoryAttribute>().GetAllAsync();
                itemsToBackup.AddRange(CategoryAttribute);
                var CCardClosingDate = await uow.GetRepository<CCardClosingDate>().GetAllAsync();
                itemsToBackup.AddRange(CCardClosingDate);
                var SmsTemplate = await uow.GetRepository<SmsTemplate>().GetAllAsync();
                itemsToBackup.AddRange(SmsTemplate.Where(x => x.Id > 0));
                var Category = await uow.GetRepository<Category>().GetAllAsync();
                itemsToBackup.AddRange(Category.Where(x => x.Id > 0));
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
                _pages.OfType<BlotterVM>().First().Entities.Clear();
                _pages.OfType<BlotterVM>().First().Entities.AddRange(allTransactions.OrderByDescending(x => x.datetime).ToList());
            }
        }
    }
}
