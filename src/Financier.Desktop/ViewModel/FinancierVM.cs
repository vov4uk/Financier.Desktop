using FinancistoAdapter;
using FinancistoAdapter.Entities;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Linq;

namespace FinancierDesktop.ViewModel
{
    public class FinancierVM : BindableBase
    {
        private string backupPath;
        public string BackupPath
        {
            get { return backupPath; }
            set { SetProperty(ref backupPath, value); }
        }

        public ReadOnlyObservableCollection<Account> Accounts { get; }
        public ReadOnlyObservableCollection<Category> Categories { get; }
        public ReadOnlyObservableCollection<Project> Projects { get; }
        public ReadOnlyObservableCollection<Payee> Payees { get; }
        public ReadOnlyObservableCollection<Location> Locations { get; }
        public ReadOnlyObservableCollection<Currency> Currencies { get; }
        public ReadOnlyObservableCollection<ExchangeRate> Rates { get; }

        public ObservableCollection<Transaction> Transactions => _transactions;
        public ObservableCollection<TransactionAttribute> TransactionAttributes => _transactionAtr;


        private int credit;
        public int Credit
        {
            get { return credit; }
            set { SetProperty(ref credit, value); }
        }

        private RangeObservableCollection<Account> _accounts;
        private RangeObservableCollection<Category> _categories;
        private RangeObservableCollection<Project> _projects;
        private RangeObservableCollection<Payee> _payees;
        private RangeObservableCollection<Location> _locations;
        private RangeObservableCollection<Currency> _currencies;
        private RangeObservableCollection<ExchangeRate> _rates;
        private RangeObservableCollection<Transaction> _transactions;
        private RangeObservableCollection<TransactionAttribute> _transactionAtr;

        public FinancierVM()
        {
            _accounts = new RangeObservableCollection<Account>();
            Accounts = new ReadOnlyObservableCollection<Account>(_accounts);

            _categories = new RangeObservableCollection<Category>();
            Categories = new ReadOnlyObservableCollection<Category>(_categories);

            _projects = new RangeObservableCollection<Project>();
            Projects = new ReadOnlyObservableCollection<Project>(_projects);

            _payees = new RangeObservableCollection<Payee>();
            Payees = new ReadOnlyObservableCollection<Payee>(_payees);

            _locations = new RangeObservableCollection<Location>();
            Locations = new ReadOnlyObservableCollection<Location>(_locations);

            _currencies = new RangeObservableCollection<Currency>();
            Currencies = new ReadOnlyObservableCollection<Currency>(_currencies);

            _rates = new RangeObservableCollection<ExchangeRate>();
            Rates = new ReadOnlyObservableCollection<ExchangeRate>(_rates);

            _transactions = new RangeObservableCollection<Transaction>();
            _transactionAtr = new RangeObservableCollection<TransactionAttribute>();
        }

        public void GetEntities(string backupPath)
        {
            BackupPath = backupPath;
            var entities = EntityReader.GetEntities(backupPath).ToList();
            _accounts.AddRange(entities.OfType<Account>().ToList());
            _categories.AddRange(entities.OfType<Category>().ToList());
            _projects.AddRange(entities.OfType<Project>().ToList());
            _payees.AddRange(entities.OfType<Payee>().ToList());
            _locations.AddRange(entities.OfType<Location>().ToList());
            _currencies.AddRange(entities.OfType<Currency>().ToList());
            _rates.AddRange(entities.OfType<ExchangeRate>().ToList());
            _transactions.AddRange(entities.OfType<Transaction>().ToList());
            _transactionAtr.AddRange(entities.OfType<TransactionAttribute>().ToList());
        }
    }
}
