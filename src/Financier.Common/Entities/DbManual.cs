using Financier.Common.Model;
using Financier.DataAccess.Abstractions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Financier.Common.Entities
{
    [ExcludeFromCodeCoverage]
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
        private static List<RuleModel> _rules = new List<RuleModel>();

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

        public static readonly Dictionary<string, int[]> MCCCategories = new Dictionary<string, int[]>
        {
            { "Автосервіс", new [] {5511, 5531, 5532, 5533, 7531, 7534, 7535, 7538, 7542} },
            { "Авіаквитки" , new [] { 3110, 3111, 3112, 3113, 3114, 3115, 3116, 3117, 3118, 3119, 3120, 3121, 3122, 3123, 3124, 3125, 3126, 3127, 3128, 3129, 3130, 3131, 3132, 3133, 3134, 3135, 3136, 3137, 3138, 3139, 3140, 3141, 3142, 3143, 3144, 3145, 3146, 3147, 3148, 3150, 3151, 3152, 3153, 3154, 3155, 3156, 3157, 3158, 3159, 3160, 3161, 3162, 3163, 3164, 3165, 3166, 3167, 3168, 3169, 3170, 3171, 3172, 3173, 3174, 3175, 3176, 3177, 3178, 3179, 3180, 3181, 3182, 3183, 3184, 3185, 3186, 3187, 3188, 3189, 3190, 3191, 3192, 3193, 3194, 3195, 3196, 3197, 3198, 3199, 3200, 3201, 3202, 3203, 3204, 3205, 3206, 3207, 3208, 3209, 3210, 3211, 3212, 3213, 3214, 3215, 3216, 3217, 3218, 3219, 3220, 3221, 3222, 3223, 3224, 3225, 3226, 3227, 3228, 3229, 3230, 3231, 3232, 3233, 3234, 3235, 3236, 3237, 3281, 3282, 3283, 3284, 3285, 3286, 3287, 3288, 3289, 3290, 3291, 3292, 3293, 3294, 3295, 3296, 3297, 3298, 3299, 3301, 4511, 3000, 3238, 3239, 3240, 3241, 3242, 3243, 3244, 3245, 3246, 3247, 3248, 3249, 3250, 3251, 3252, 3253, 3254, 3256, 3257, 3258, 3259, 3260, 3261, 3262, 3263, 3264, 3265, 3266, 3267, 3268, 3270, 3274, 3275, 3276, 3277, 3278, 3279 } },
            { "АЗС", new []{5172, 5541, 5542, 5983 } },
            { "Аптеки",new [] {5912, 5122, 5292, 5295}},
            { "Готелі", new int[]{ 3700, 3699, 3698, 3697, 3696, 3695, 3694, 3693, 3692, 3691, 3690, 3689, 3688, 3687, 3686, 3685, 3684, 3683, 3682, 3681, 3680, 3679, 3678, 3677, 3676, 3675, 3674, 3673, 3672, 3671, 3670, 3669, 3668, 3667, 3666, 3665, 3664, 3663, 3662, 3661, 3660, 3659, 3658, 3657, 3656, 3655, 3654, 3653, 3652, 3651, 3650, 3649, 3648, 3647, 3646, 3645, 3644, 3643, 3642, 3641, 3640, 3639, 3638, 3637, 3636, 3635, 3634, 3633, 3632, 3631, 3630, 3629, 3628, 3627, 3626, 3625, 3624, 3623, 3622, 3621, 3620, 3619, 3618, 3617, 3616, 3615, 3614, 3613, 3612, 3611, 3610, 3609, 3608, 3607, 3606, 3605, 3604, 3603, 3602, 3601, 3600, 3599, 3598, 3597, 3596, 3595, 3594, 3593, 3592, 3591, 3590, 3589, 3588, 3587, 3586, 3585, 3584, 3583, 3582, 3581, 3580, 3579, 3578, 3577, 3576, 3575, 3574, 3573, 3572, 3571, 3570, 3569, 3568, 3567, 3566, 3565, 3564, 3563, 3562, 3561, 3560, 3559, 3558, 3557, 3556, 3555, 3554, 3553, 3552, 3551, 3550, 3549, 3548, 3547, 3546, 3545, 3544, 3543, 3542, 3541, 3540, 3539, 3538, 3537, 3536, 3535, 3534, 3533, 3532, 3531, 3530, 3529, 3528, 3527, 3526, 3525, 3524, 3523, 3522, 3521, 3520, 3519, 3518, 3517, 3516, 3515, 3514, 3513, 3512, 3511, 3510, 3509, 3508, 3507, 3506, 3505, 3504, 3503, 3502 } },
            { "Взуття",new [] {5139, 5661}},
            { "Все для дому",new [] {5200, 5211, 5231, 5713, 5712, 5719, 5131}},
            { "Duty Free",new [] {5309}},
            { "Дозвілля",new [] {7932, 7933, 7996, 7998, 7999, 5733, 5818, 7841, 7993, 7221, 7395}},
            { "Хімчистки",new [] {7216, 7210, 7211, 7251, 7217}},
            { "Дитячі товари",new [] {5641, 5945}},
            { "Зоомагазини",new [] {0742, 5995}},
            { "Ігри та додатки",new [] {5816, 5817, 5815}},
            { "Кафе та ресторани",new [] {5811, 5812, 5813, 5814}},
            { "Квіти",new [] {5992, 0780, 5261, 5193}},
            { "Кіно та театри",new [] {7829, 7832, 7922}},
            { "Книги", new []{5192, 5942, 5943, 5949, 5970}},
            { "Кондитерські",new [] {5441, 5462}},
            { "Краса та догляд", new []{7230, 7297, 7298, 5977}},
            { "Маркетплейси", new [] {5300, 5964, 5310, 5311, 5331, 5399, 5999 }},
            { "Медзаклади", new [] { 8021, 8062, 8071, 8099, 4119, 8011, 8031, 8041, 8042, 8049, 8050, 8043 }},
            { "Одяг", new [] { 5611, 5931, 5137, 5699, 5691, 5681, 5651, 5621 }},
            { "Продукти", new [] { 5411, 5499, 5451, 5422, 5412, 5921 }},
            { "Спорт та фітнес", new [] { 5655, 5940, 5941, 7911, 7941, 7992, 7997 }},
            { "Таксі", new [] { 4121 }},
            { "Техніка", new [] { 4812, 5722, 5732, 5946 }},
            { "Транспорт", new [] { 4131, 4111, 4112 }},
            { "Доставка", new [] { 7399, 8999 } },
            { "Переказ", new [] { 4829, 6010, 6012 } },
            { "Комунальні", new [] { 4900 } },
            { "Банкомат", new [] { 6011 } },
            { "Мобільний", new [] { 4814 } },
            { "Благодійна організація", new [] { 8398 } },
        };

        public static List<string> MCCCategoriesKeys => MCCCategories.Keys.ToList();

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

        public static void ResetManuals(string manual)
        {
            switch (manual)
            {
                case nameof(Payee):    _payee = null; break;
                case nameof(Location): _location = null; break;
                case nameof(Project):  _project = null; break;
                case nameof(Account):  _accounts = null; break;
                default:
                    break;
            }
        }

        public static async Task LoadRulesAsync()
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
    }
}