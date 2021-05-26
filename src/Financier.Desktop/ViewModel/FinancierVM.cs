using Financier.DataAccess;
using Financier.DataAccess.Data;
using Financier.DataAccess.View;
using FinancistoAdapter;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Financier.Desktop.ViewModel
{
    public class FinancierVM : BindableBase
    {
        FinancierDatabase db;

        private string backupPath;
        public string BackupPath
        {
            get { return backupPath; }
            set { SetProperty(ref backupPath, value); }
        }

        public RangeObservableCollection<Account> Accounts => _accounts;
        public RangeObservableCollection<Category> Categories => _categories;
        public RangeObservableCollection<Project> Projects => _projects;
        public RangeObservableCollection<Payee> Payees => _payees;
        public RangeObservableCollection<Location> Locations => _locations;
        public RangeObservableCollection<Currency> Currencies => _currencies;
        public RangeObservableCollection<CurrencyExchangeRate> Rates => _rates;
        public RangeObservableCollection<Budget> Budgets => _budget;
        public RangeObservableCollection<TransactionsView> Transactions => _transactions;
        public RangeObservableCollection<TransactionAttribute> TransactionAttributes => _transactionAtr;


        private RangeObservableCollection<Account> _accounts;
        private RangeObservableCollection<Category> _categories;
        private RangeObservableCollection<Project> _projects;
        private RangeObservableCollection<Payee> _payees;
        private RangeObservableCollection<Location> _locations;
        private RangeObservableCollection<Currency> _currencies;
        private RangeObservableCollection<CurrencyExchangeRate> _rates;
        private RangeObservableCollection<TransactionsView> _transactions;
        private RangeObservableCollection<TransactionAttribute> _transactionAtr;
        private RangeObservableCollection<Budget> _budget;

        public FinancierVM()
        {
            _accounts = new RangeObservableCollection<Account>();
            _categories = new RangeObservableCollection<Category>();
            _projects = new RangeObservableCollection<Project>();
            _payees = new RangeObservableCollection<Payee>();
            _locations = new RangeObservableCollection<Location>();
            _currencies = new RangeObservableCollection<Currency>();
            _rates = new RangeObservableCollection<CurrencyExchangeRate>();
            _budget = new RangeObservableCollection<Budget>();
            _transactions = new RangeObservableCollection<TransactionsView>();
            _transactionAtr = new RangeObservableCollection<TransactionAttribute>();
        }

        public async Task GetEntities(string backupPath)
        {
            BackupPath = backupPath;
            var entities = EntityReader.GetEntities(backupPath).ToList();

            db = new FinancierDatabase();
            await db.Import(entities);

            using (var uow = db.CreateUnitOfWork())
            {
                var allAccounts = await uow.GetRepository<Account>().GetAllAsync(x => x.Currency);
                var allTransactions = await uow.GetRepository<BlotterTransactions>().GetAllAsync(x => x.from_account_currency, x => x.to_account_currency);
                var allCategories = await uow.GetRepository<Category>().GetAllAsync();

                _accounts = new RangeObservableCollection<Account>(allAccounts.OrderBy(x => x.IsActive).ThenBy(x => x.SortOrder));
                _transactions = new RangeObservableCollection<TransactionsView>(allTransactions.OrderByDescending(x => x.datetime).ToList());
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
            //_transactions.AddRange(entities.OfType<Transaction>().ToList());
            _transactionAtr.Clear();
            _transactionAtr.AddRange(entities.OfType<TransactionAttribute>().ToList());
            _budget.Clear();
            _budget.AddRange(entities.OfType<Budget>().ToList());
        }
    }
}
