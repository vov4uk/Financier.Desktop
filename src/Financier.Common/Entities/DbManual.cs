using Financier.Common.Model;
using Financier.DataAccess.Abstractions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Financier.Common.Entities
{
    public static class DbManual
    {
        private static List<AccountFilterModel> _accounts;
        private static List<LocationModel> _location;
        private static List<CategoryModel> _category;
        private static List<CategoryModel> _topCategory;
        private static List<CurrencyModel> _currencies;
        private static List<PayeeModel> _payee;
        private static List<ProjectModel> _project;
        private static List<YearMonths> _yearMonths;
        private static List<Years> _years;

        public static async Task SetupAsync(IFinancierDatabase financierDatabase)
        {
            if (_accounts == null)
            {
                var accounts = await financierDatabase.ExecuteQuery<AccountFilterModel>(@"
SELECT a._id,
       a.title,
       a.is_active,
       a.sort_order,
       a.currency_id,
       a.total_amount,
       a.type,
       c.Name as currency_name
FROM   account a
INNER JOIN currency c ON a.currency_id = c._id
WHERE  a.title IS NOT NULL
ORDER  BY 3 DESC, 4 ASC"
);
                _accounts = new List<AccountFilterModel>(accounts);
                _accounts.Insert(0, new AccountFilterModel());
            }

            if (_location == null)
            {
                var locations = await financierDatabase.ExecuteQuery<LocationModel>(@"
SELECT _id,
       title,
       is_active,
       resolved_address
FROM   locations
WHERE  title IS NOT NULL
ORDER  BY 3 DESC, 2 ASC"
);
                _location = new List<LocationModel>(locations);
                _location.Insert(0, new LocationModel());
            }

            if (_category == null)
            {
                var categories = await financierDatabase.ExecuteQuery<CategoryModel>(@"
SELECT _id,
       title,
       LEFT,
       [right],
       type,
       (SELECT Count(*)
        FROM   category x
        WHERE  x.LEFT < ctx.LEFT
               AND x.[right] > ctx.[right]) AS level
FROM   category ctx
ORDER  BY LEFT,
          sort_order");
                _category = new List<CategoryModel>(categories);
                _category.Insert(0, new CategoryModel());

                _topCategory = new List<CategoryModel>(categories.Where(x => x.Level == 0 && x.Id > 0));
                _topCategory.Insert(0, new CategoryModel());
            }

            if (_currencies == null)
            {
                var currencies = await financierDatabase.ExecuteQuery<CurrencyModel>("select * from currency");

                _currencies = new List<CurrencyModel>(currencies);
                _currencies.Insert(0, new CurrencyModel()
                {
                    Name = "All currencies"
                });
            }

            if (_payee == null)
            {
                var payees = await financierDatabase.ExecuteQuery<PayeeModel>(@"
SELECT _id,
       title,
       is_active
FROM   payee
WHERE  title IS NOT NULL
ORDER  BY is_active DESC, title ASC");
                _payee = new List<PayeeModel>(payees);
                _payee.Insert(0, new PayeeModel());
            }

            if (_project == null)
            {
                var projects = await financierDatabase.ExecuteQuery<ProjectModel>(@"
SELECT _id,
       title,
       is_active
FROM   project
WHERE  title IS NOT NULL
ORDER  BY is_active DESC, title ASC");
                _project = new List<ProjectModel>(projects);
                _project.Insert(0, new ProjectModel());
            }

            if (_yearMonths == null)
            {
                var yearMonths = await financierDatabase.ExecuteQuery<YearMonths>(@"
SELECT DISTINCT date_year  AS year,
                date_month AS month
FROM   v_report_transactions
ORDER  BY 1 DESC,
          2 DESC");
                _yearMonths = new List<YearMonths>(yearMonths);
                _yearMonths.Insert(0, new YearMonths());
            }

            if (_years == null)
            {
                var years = await financierDatabase.ExecuteQuery<Years>(@"
SELECT DISTINCT date_year AS year
FROM   v_report_transactions
ORDER  BY 1 DESC ");
                _years = new List<Years>(years);
                _years.Insert(0, new Years());
            }
        }

        public static List<AccountFilterModel> Account => _accounts;

        public static List<CategoryModel> Category => _category;

        public static List<CategoryModel> TopCategories => _topCategory;

        public static List<CurrencyModel> Currencies => _currencies;

        public static List<PayeeModel> Payee => _payee;

        public static List<ProjectModel> Project => _project;

        public static List<YearMonths> YearMonths => _yearMonths;

        public static List<Years> Years => _years;

        public static List<LocationModel> Location => _location;

        public static void ResetAllManuals()
        {
            _accounts = null;
            _category = null;
            _topCategory = null;
            _currencies = null;
            _payee = null;
            _project = null;
            _yearMonths = null;
            _years = null;
            _location = null;
        }

        public static void ReseManuals(string manual)
        {
            switch (manual)
            {
                case nameof(Payee): _payee = null; break;
                case nameof(Location):_location = null; break;
                case nameof(Project):_project = null; break;
                default:
                    break;
            }
        }

        internal static void SetupTests(List<CategoryModel> categories)
        {
            _category = categories;
        }

        internal static void SetupTests(List<PayeeModel> payee)
        {
            _payee = payee;
        }

        internal static void SetupTests(List<LocationModel> loc)
        {
            _location = loc;
        }

        internal static void SetupTests(List<CurrencyModel> cur)
        {
            _currencies = cur;
        }

        internal static void SetupTests(List<AccountFilterModel> acc)
        {
            _accounts = acc;
        }

        internal static void SetupTests(List<ProjectModel> pj)
        {
            _project = pj;
        }
    }
}