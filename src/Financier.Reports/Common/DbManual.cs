using Financier.DataAccess.Abstractions;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Financier.Reports.Common
{
    public static class DbManual
    {
        private static ObservableCollection<AccountModel> _accounts;
        private static ObservableCollection<CategoryModel> _category;
        private static ObservableCollection<CategoryModel> _topCategory;
        private static ObservableCollection<CurrencyModel> _currencies;
        private static ObservableCollection<PayeeModel> _payee;
        private static ObservableCollection<ProjectModel> _project;
        private static ObservableCollection<YearMonths> _yearMonths;
        private static ObservableCollection<Years> _years;

        public static async Task Setup(IFinancierDatabase financierDatabase)
        {
            if (_accounts == null)
            {
                var accounts = await financierDatabase.ExecuteQuery<AccountModel>(@"
SELECT _id,
       title
FROM   account
WHERE  title IS NOT NULL
ORDER  BY 2 DESC"
);
                _accounts = new ObservableCollection<AccountModel>(accounts);
                _accounts.Insert(0, new AccountModel());
            }

            if (_category == null)
            {
              var categories = await financierDatabase.ExecuteQuery<CategoryModel>(
@"SELECT _id,
       title,
       LEFT,
       [right],
       (SELECT Count(*)
        FROM   category x
        WHERE  x.LEFT < ctx.LEFT
               AND x.[right] > ctx.[right]) AS level
FROM   category ctx
ORDER  BY LEFT,
          sort_order");
                _category = new ObservableCollection<CategoryModel>(categories);
                _category.Insert(0, new CategoryModel());

                _topCategory = new ObservableCollection<CategoryModel>(categories.Where(x => x.Level == 0 && x.ID > 0));
                _topCategory.Insert(0, new CategoryModel());
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
                var payees = await financierDatabase.ExecuteQuery<PayeeModel>(@"
SELECT _id,
       title
FROM   payee
WHERE  title IS NOT NULL
ORDER  BY 2 DESC");
                _payee = new ObservableCollection<PayeeModel>(payees);
                _payee.Insert(0, new PayeeModel());
            }

            if (_project == null)
            {
                var projects = await financierDatabase.ExecuteQuery<ProjectModel>(@"
SELECT _id,
       title
FROM   project
WHERE  title IS NOT NULL
ORDER  BY 2 DESC");
                _project = new ObservableCollection<ProjectModel>(projects);
                _project.Insert(0, new ProjectModel());
            }

            if (_yearMonths == null)
            {
                var yearMonths = await financierDatabase.ExecuteQuery<YearMonths>(
@"SELECT DISTINCT date_year  AS year,
                date_month AS month
FROM   v_report_transactions
ORDER  BY 1 DESC,
          2 DESC");
                _yearMonths = new ObservableCollection<YearMonths>(yearMonths);
                _yearMonths.Insert(0, new YearMonths());
            }

            if (_years == null)
            {
                var years = await financierDatabase.ExecuteQuery<Years>(@"
SELECT DISTINCT date_year AS year
FROM   v_report_transactions
ORDER  BY 1 DESC ");
                _years = new ObservableCollection<Years>(years);
                _years.Insert(0, new Years());
            }
        }

        public static ObservableCollection<AccountModel> Account => _accounts;

        public static ObservableCollection<CategoryModel> Category => _category;

        public static ObservableCollection<CategoryModel> TopCategories => _topCategory;

        public static ObservableCollection<CurrencyModel> Currencies => _currencies;

        public static ObservableCollection<PayeeModel> Payee => _payee;

        public static ObservableCollection<ProjectModel> Project => _project;

        public static ObservableCollection<YearMonths> YearMonths => _yearMonths;

        public static ObservableCollection<Years> Years => _years;

        public static void ResetManuals()
        {
            _project = null;
            _category = null;
            _accounts = null;
            _currencies = null;
            _yearMonths = null;
            _years = null;
        }
    }
}