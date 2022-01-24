using Financier.DataAccess.Abstractions;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Financier.Reports.Common
{
    public static class DbManual
    {
        private static ObservableCollection<AccountModel> _account;
        private static ObservableCollection<CategoryModel> _category;
        private static ObservableCollection<CurrencyModel> _currencies;
        private static ObservableCollection<PayeeModel> _payee;
        private static ObservableCollection<ProjectModel> _project;
        private static ObservableCollection<YearMonths> _yearMonths;
        private static ObservableCollection<Years> _years;

        public static async Task Setup(IFinancierDatabase financierDatabase)
        {
            if (_account == null)
            {
                var accounts = await financierDatabase.ExecuteQuery<AccountModel>(
                    "select _id, " +
                    "title " +
                    "from account " +
                    "where title is not null " +
                    "order by 2 desc");
                _account = new ObservableCollection<AccountModel>(accounts);
                _account.Insert(0, new AccountModel());
            }

            if (_category == null)
            {
              var categories = await financierDatabase.ExecuteQuery<CategoryModel>(
                  "select " +
                  "_id, " +
                  "title, " +
                  "(select count(*) from category x where x.left < ctx.left and x.[right] > ctx.[right] ) as level " +
                  "from category ctx " +
                  "order by left, sort_order");
                _category = new ObservableCollection<CategoryModel>(categories);
                _category.Insert(0, new CategoryModel());
            }

            if (_currencies == null)
            {
                var currencies = await financierDatabase.ExecuteQuery<CurrencyModel>("select * from currency");

                _currencies = new ObservableCollection<CurrencyModel>(currencies);
                _currencies.Insert(0, new CurrencyModel()
                {
                    Name = "All currencies"
                });
            }

            if (_payee == null)
            {
                var payees = await financierDatabase.ExecuteQuery<PayeeModel>(
                    "select _id, " +
                    "title " +
                    "from payee " +
                    "where title is not null " +
                    "order by 2 desc");
                _payee = new ObservableCollection<PayeeModel>(payees);
                _payee.Insert(0, new PayeeModel());
            }

            if (_project == null)
            {
                var projects = await financierDatabase.ExecuteQuery<ProjectModel>(
                    "select _id, " +
                    "title " +
                    "from project " +
                    "where title is not null " +
                    "order by 2 desc");
                _project = new ObservableCollection<ProjectModel>(projects);
                _project.Insert(0, new ProjectModel());
            }

            if (_yearMonths == null)
            {
                var yearMonths = await financierDatabase.ExecuteQuery<YearMonths>(
                    "select distinct date_year as year, " +
                    "date_month as month " +
                    "from v_report_transactions " +
                    "order by 1 desc, " +
                    "2 desc");
                _yearMonths = new ObservableCollection<YearMonths>(yearMonths);
                _yearMonths.Insert(0, new YearMonths());
            }

            if (_years == null)
            {
                var years = await financierDatabase.ExecuteQuery<Years>(
                    "select distinct date_year as year " +
                    "from v_report_transactions " +
                    "order by 1 desc");
                _years = new ObservableCollection<Years>(years);
                _years.Insert(0, new Years());
            }
        }

        public static ObservableCollection<AccountModel> Account => _account;

        public static ObservableCollection<CategoryModel> Category => _category;

        public static ObservableCollection<CurrencyModel> Currencies => _currencies;

        public static ObservableCollection<PayeeModel> Payee => _payee;

        public static ObservableCollection<ProjectModel> Project => _project;

        public static ObservableCollection<YearMonths> YearMonths => _yearMonths;

        public static ObservableCollection<Years> Years => _years;

        public static void ResetManuals()
        {
            _project = null;
            _category = null;
            _account = null;
            _currencies = null;
            _yearMonths = null;
            _years = null;
        }
    }
}