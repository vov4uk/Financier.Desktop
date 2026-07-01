using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Financier.Common.Attribute;
using Financier.Common.Model;
using Financier.DataAccess.Abstractions;
using Newtonsoft.Json;

namespace Financier.Common.Entities
{
    [ExcludeFromCodeCoverage]
    public static class DbManual
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private static List<AccountFilterModel> _accounts;
        private static List<LocationModel> _location;
        private static List<CategoryModel> _category;
        private static List<CategoryModel> _topCategory;
        private static List<CurrencyModel> _currencies;
        private static List<PayeeModel> _payee;
        private static List<ProjectModel> _project;
        private static List<YearMonths> _yearMonths;
        private static List<Years> _years;
        private static List<RuleModel> _rules = new List<RuleModel>();
        private static Dictionary<Mcc, int[]> _mccEnums;
        private static Dictionary<string, Mcc> _mccTitles;
        private static Dictionary<int, Mcc> _mccCodes;
        private static List<List<string>> _allCurrencies;

        public static async Task SetupAsync(IFinancierDatabase financierDatabase)
        {

            if (financierDatabase == null)
            {
                return;
            }

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
       a.last_transaction_id,
       a.number,
       c.Name as currency_name,
       a.card_issuer,
       a.issuer
FROM   account a
INNER JOIN currency c ON a.currency_id = c._id
WHERE  a.title IS NOT NULL
ORDER  BY 3 DESC, 4 ASC"
);
                _accounts = [.. accounts];
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
                _location = [.. locations];
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
                _category = [.. categories];
                _category.Insert(0, new CategoryModel());

                _topCategory = [.. categories.Where(x => x.Level == 0 && x.Id > 0)];
                _topCategory.Insert(0, new CategoryModel());
            }

            if (_currencies == null)
            {
                var currencies = await financierDatabase.ExecuteQuery<CurrencyModel>(
                    "SELECT * FROM currency");

                _currencies = new List<CurrencyModel>(currencies);
                _currencies.Insert(0, new CurrencyModel()
                {
                    Name = Localization.LocalizationService.Instance.all_currencies
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
                _payee = [.. payees];
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
                _project = [.. projects];
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
                _yearMonths = [.. yearMonths];
                _yearMonths.Insert(0, new YearMonths());
            }

            if (_years == null)
            {
                var years = await financierDatabase.ExecuteQuery<Years>(@"
SELECT DISTINCT date_year AS year
FROM   v_report_transactions
ORDER  BY 1 DESC ");
                _years = [.. years];
                _years.Insert(0, new Years());
            }
        }

        public static List<AccountFilterModel> Account => _accounts ?? new();

        public static List<CategoryModel> Category => _category ?? new();

        public static List<CategoryModel> SubCategory => _category?.Where(x => x.Id > 0).ToList() ?? new();

        public static List<CategoryModel> TopCategories => _topCategory ?? new();

        public static List<CurrencyModel> Currencies => _currencies ?? new();

        public static List<PayeeModel> Payee => _payee ?? new ();

        public static List<ProjectModel> Project => _project ?? new();

        public static List<YearMonths> YearMonths => _yearMonths ?? new();

        public static List<Years> Years => _years ?? new();

        public static List<LocationModel> Location => _location ?? new();

        public static List<RuleModel> Rules => _rules;

        public static Dictionary<Mcc, int[]> MCCEnums
        {
            get
            {

                if (_mccEnums == null)
                {
                    InitializaMccCodes();
                }

                return _mccEnums;
            }
        }

        public static Dictionary<string, Mcc> MCCTitles
        {
            get
            {
                if (_mccTitles == null)
                {
                    InitializaMccCodes();
                }

                return _mccTitles;
            }
        }

        public static Dictionary<int, Mcc> MCCCodes
        {
            get
            {
                if (_mccCodes == null)
                {
                    InitializaMccCodes();
                }

                return _mccCodes;
            }
        }

        public static List<List<string>> AllCurrencies
        {
            get
            {
                if (_allCurrencies == null)
                {
                    var asm = typeof(DbManual).Assembly;
                    using var stream = asm.GetManifestResourceStream("Financier.Common.Assets.currencies.csv");
                    if (stream != null)
                    {
                        using var reader = new StreamReader(stream);
                        var lines = new List<List<string>>();
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#'))
                                continue;
                            var fields = ParseCsvLine(line);
                            if (fields.Count == 6 || fields.Count == 7)
                                lines.Add(fields);
                        }
                        _allCurrencies = lines;
                    }
                    else
                    {
                        _allCurrencies = new List<List<string>>();
                    }
                }
                return _allCurrencies;
            }
        }

        private static List<string> ParseCsvLine(string line)
        {
            var fields = new List<string>();
            var current = new System.Text.StringBuilder();
            bool inQuotes = false;
            foreach (char c in line)
            {
                if (c == '"') { inQuotes = !inQuotes; }
                else if (c == ',' && !inQuotes) { fields.Add(current.ToString()); current.Clear(); }
                else { current.Append(c); }
            }
            fields.Add(current.ToString());
            return fields;
        }

        public static void ResetAllDatabaseManuals()
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

        public static void ResetManuals(string manual)
        {
            switch (manual)
            {
                case nameof(Payee):            _payee = null; break;
                case nameof(Location):         _location = null; break;
                case nameof(Project):          _project = null; break;
                case nameof(Account):          _accounts = null; break;
                case nameof(MCCEnums):         _mccEnums = null; break;
                case nameof(MCCTitles):        _mccTitles = null; break;
                case nameof(Currencies):       _currencies = null; break;
                case nameof(Category):         _category = null; _topCategory = null; break;
                default:
                    break;
            }
        }

        public static async Task LoadRulesAsync()
        {
            try
            {
                var directory = Environment.CurrentDirectory;
                var path = Path.Combine(directory, "rules.json");
                if (File.Exists(path))
                {
                    string rulesJson = await File.ReadAllTextAsync(path);
                    var rules = JsonConvert.DeserializeObject<List<RuleModel>>(rulesJson);
                    if (rules?.Any() == true)
                    {
                        _rules = rules;
                    }
                }
            }
            catch (Exception ex)
            {
                _rules = new List<RuleModel>();
                Logger.Error(ex, "Error occurred while loading rules.");
            }

        }
        public static async Task SaveRulesAsync()
        {
            var directory = Environment.CurrentDirectory;
            var path = Path.Combine(directory, "rules.json");
            if (!File.Exists(path))
            {
                File.Create(path).Dispose();
            }
            string rulesJson = JsonConvert.SerializeObject(_rules);
            await File.WriteAllTextAsync(path, rulesJson);
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

        internal static void SetupTests(List<RuleModel> rl)
        {
            _rules = rl;
        }

        private static void InitializaMccCodes()
        {
            Type t = typeof(Mcc);
            Array result = Enum.GetValues(t);
            _mccEnums = new Dictionary<Mcc, int[]>();
            _mccTitles = new Dictionary<string, Mcc>();
            _mccCodes = new Dictionary<int, Mcc>();
            foreach (Mcc item in result)
            {
                MemberInfo mi = t.GetTypeInfo().GetMember(item.ToString()).FirstOrDefault();
                if (mi != null)
                {
                    var mccAttribute = mi.GetCustomAttribute<MccCodesAttribute>();
                    if (mccAttribute != null)
                    {
                        _mccEnums.Add(item, mccAttribute.Codes);
                        foreach (var code in mccAttribute.Codes)
                        {
                            _mccCodes.Add(code, item);
                        }
                    }

                    var locAttribute = mi.GetCustomAttribute<LocalizedMccDescriptionAttribute>();
                    if (locAttribute != null)
                    {
                        _mccTitles.Add(locAttribute.Description, item);
                    }
                }
            }
        }

    }
}
