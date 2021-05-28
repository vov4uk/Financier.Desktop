using Financier.DataAccess;
using Financier.DataAccess.Data;
using Financier.DataAccess.View;
using Financier.Desktop.Converters;
using Financier.Desktop.MonoWizard.Model;
using FinancistoAdapter;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Financier.Desktop.ViewModel
{
    public class FinancierVM : BindableBase
    {
        FinancierDatabase db;

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

        public List<Account> Accounts => _accounts;
        public RangeObservableCollection<Category> Categories => _categories;
        public RangeObservableCollection<Project> Projects => _projects;
        public RangeObservableCollection<Payee> Payees => _payees;
        public RangeObservableCollection<Location> Locations => _locations;
        public RangeObservableCollection<Currency> Currencies => _currencies;
        public RangeObservableCollection<CurrencyExchangeRate> Rates => _rates;
        public RangeObservableCollection<Budget> Budgets => _budget;
        public List<TransactionsView> Transactions => _transactions;
        public RangeObservableCollection<TransactionAttribute> TransactionAttributes => _transactionAtr;


        private List<Account> _accounts;
        private RangeObservableCollection<Category> _categories;
        private RangeObservableCollection<Project> _projects;
        private RangeObservableCollection<Payee> _payees;
        private RangeObservableCollection<Location> _locations;
        private RangeObservableCollection<Currency> _currencies;
        private RangeObservableCollection<CurrencyExchangeRate> _rates;
        private List<TransactionsView> _transactions;
        private RangeObservableCollection<TransactionAttribute> _transactionAtr;
        private RangeObservableCollection<Budget> _budget;

        public FinancierVM()
        {
            _accounts = new List<Account>();
            _categories = new RangeObservableCollection<Category>();
            _projects = new RangeObservableCollection<Project>();
            _payees = new RangeObservableCollection<Payee>();
            _locations = new RangeObservableCollection<Location>();
            _currencies = new RangeObservableCollection<Currency>();
            _rates = new RangeObservableCollection<CurrencyExchangeRate>();
            _budget = new RangeObservableCollection<Budget>();
            _transactions = new List<TransactionsView>();
            _transactionAtr = new RangeObservableCollection<TransactionAttribute>();
        }

        public async Task OpenBackup(string backupPath)
        {
            OpenBackupPath = backupPath;
            var entities = EntityReader.ParseBackupFile(backupPath).ToList();

            db = new FinancierDatabase();
            await db.Import(entities);

            using (var uow = db.CreateUnitOfWork())
            {
                var allAccounts = await uow.GetRepository<Account>().GetAllAsync(x => x.Currency);
                var allTransactions = await uow.GetRepository<BlotterTransactions>().GetAllAsync(x => x.from_account_currency, x => x.to_account_currency);
                var allCategories = await uow.GetRepository<Category>().GetAllAsync();

                _accounts = allAccounts.OrderByDescending(x => x.IsActive).ThenBy(x => x.SortOrder).ToList();
                _transactions = new List<TransactionsView>(allTransactions.OrderByDescending(x => x.datetime).ToList());
                _categories = new RangeObservableCollection<Category>(allCategories);
            }

            _projects.Clear();
            _projects.AddRange(entities.OfType<Project>().ToList());
            _payees.Clear();
            _payees.AddRange(entities.OfType<Payee>().ToList());
            _locations.Clear();
            _locations.AddRange(entities.OfType<Location>().ToList());
            _currencies.Clear();
            _currencies.AddRange(entities.OfType<Currency>().ToList());
            _rates.Clear();
            _rates.AddRange(entities.OfType<CurrencyExchangeRate>().ToList());
            _transactionAtr.Clear();
            _transactionAtr.AddRange(entities.OfType<TransactionAttribute>().ToList());
            _budget.Clear();
            _budget.AddRange(entities.OfType<Budget>().ToList());
        }

        public async Task SaveBackup(string backupPath)
        {
            SaveBackupPath = backupPath;
            List<Entity> itemsToBackup = new List<Entity>();
            using (var uow = db.CreateUnitOfWork())
            {
                itemsToBackup.AddRange(_projects);
                itemsToBackup.AddRange(_payees);
                itemsToBackup.AddRange(_locations.Where(x => x.Id > 0));
                itemsToBackup.AddRange(_currencies);
                itemsToBackup.AddRange(_rates);
                itemsToBackup.AddRange(_transactionAtr);
                itemsToBackup.AddRange(_budget);

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

            using (var bw = new BackupWriter(backupPath, EntityReader.BackupVersion))
            {
                bw.GenerateBackup(itemsToBackup);
            }
        }

        public async Task ImportMonoTransactions(int accountId, List<MonoTransaction> transactions)
        {
            var converter = new DateTimeConverter();
            using (var uow = db.CreateUnitOfWork())
            {
                var transactionsRepo = uow.GetRepository<Transaction>();
                var transToAdd = transactions.Select(x => new Transaction
                {   Id = 0,
                    FromAccountId = accountId,
                    FromAmount = (long)(x.CardCurrencyAmount * 100),
                    OriginalFromAmmount = x.ExchangeRate == null ? 0 : (long)(x.OperationAmount * 100),
                    OriginalCurrencyId = x.ExchangeRate == null ? 0 : _currencies.FirstOrDefault(c => c.Name == x.OperationCurrency)?.Id ?? 0,
                    CategoryId = 0,
                    LocationId = _locations.FirstOrDefault(l => l.Name.Contains(x.Description, System.StringComparison.OrdinalIgnoreCase))?.Id ?? 0,
                    Note = x.Description,
                    DateTime = (long)converter.ConvertBack(x.Date, typeof(long), null, CultureInfo.InvariantCulture)
                }).ToList();

                await transactionsRepo.AddRangeAsync(transToAdd);
                await uow.SaveChangesAsync();
            }

            await db.RebuildRunningBalanceForAccount(accountId);

            using (var uow = db.CreateUnitOfWork())
            {
                var allTransactions = await uow.GetRepository<BlotterTransactions>().GetAllAsync(x => x.from_account_currency, x => x.to_account_currency);
                _transactions = new List<TransactionsView>(allTransactions.OrderByDescending(x => x.datetime).ToList());
            }
        }
    }
}
