namespace Financier.DataAccess.Data
{
    public class Backup
    {
        public const string TRANSACTION_TABLE = "transactions";
        public const string ACCOUNT_TABLE = "account";
        public const string CURRENCY_TABLE = "currency";
        public const string CATEGORY_TABLE = "category";
        public const string BUDGET_TABLE = "budget";
        public const string PROJECT_TABLE = "project";
        public const string ATTRIBUTES_TABLE = "attributes";
        public const string SMS_TEMPLATES_TABLE = "sms_template";
        public const string CATEGORY_ATTRIBUTE_TABLE = "category_attribute";
        public const string TRANSACTION_ATTRIBUTE_TABLE = "transaction_attribute";
        public const string LOCATIONS_TABLE = "locations";
        public const string PAYEE_TABLE = "payee";
        public const string CCARD_CLOSING_DATE_TABLE = "ccard_closing_date";
        public const string EXCHANGE_RATES_TABLE = "currency_exchange_rate";
        public const string DATABASE_VERSION = "DATABASE_VERSION";
        public const string VERSION_NAME = "VERSION_NAME";
        public const string VERSION_CODE = "VERSION_CODE";
        public const string PACKAGE = "PACKAGE";
        public const string START = "#START";
        public const string END = "#END";

        public static string[] RESTORE_SCRIPTS = {
            "_20100114_1158_alter_accounts_types",
            "_20110903_0129_alter_template_splits",
            "_20171230_1852_alter_electronic_account_type"
    };
        public static string[] BACKUP_TABLES = {
            ACCOUNT_TABLE, ATTRIBUTES_TABLE, CATEGORY_ATTRIBUTE_TABLE,
            TRANSACTION_ATTRIBUTE_TABLE, BUDGET_TABLE, CATEGORY_TABLE,
            CURRENCY_TABLE, LOCATIONS_TABLE, PROJECT_TABLE, TRANSACTION_TABLE,
            PAYEE_TABLE, CCARD_CLOSING_DATE_TABLE, SMS_TEMPLATES_TABLE,
            "split", /* todo: seems not used, found only in old 20110422_0051_create_split_table.sql, should be removed then */
            EXCHANGE_RATES_TABLE
    };
}
}