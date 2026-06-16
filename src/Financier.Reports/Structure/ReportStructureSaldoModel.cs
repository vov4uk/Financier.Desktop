using Financier.Common.Model;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Financier.Reports
{
    public class ReportStructureSaldoModel : BaseModel
    {

        public string AssetsDefaultCurrencyBalanceStr => $"{AssetsDefaultCurrencyBalance}{DefaultCurrencySymbol}";

        public string LiabilitiesDefaultCurrencyBalanceStr => $"{LiabilitiesDefaultCurrencyBalance}{DefaultCurrencySymbol}";

        public string NetWorthDefaultCurrencyBalanceStr => $"{NetWorthDefaultCurrencyBalance}{DefaultCurrencySymbol}";

        public string AssetsUSDBalanceStr => $"{AssetsUSDBalance}$";

        public string LiabilitiesUSDBalanceStr => $"{LiabilitiesUSDBalance}$";

        public string NetWorthUSDBalanceStr => $"{NetWorthUSDBalance}$";

        [DisplayName("assets_home_currency")]
        public double? AssetsDefaultCurrencyBalance { get; set; }

        [DisplayName("liabilities_home_currency")]
        public double? LiabilitiesDefaultCurrencyBalance { get; set; }

        [DisplayName("assets_usd")]
        public double? AssetsUSDBalance { get; set; }

        [DisplayName("liabilities_usd")]
        public double? LiabilitiesUSDBalance { get; set; }

        [DisplayName("net_worth_usd")]
        public double? NetWorthUSDBalance { get; set; }

        [DisplayName("net_worth_home_currency")]
        public double? NetWorthDefaultCurrencyBalance { get; set; }

        public string DefaultCurrencySymbol { get; set; }

        [DisplayName("date")]
        public DateOnly Date { get; set; }
    }

    public class ReportStructureSaldoRawModel
    {
        [Column("account_title")]
        [DisplayName("account")]
        public string Title { get; protected set; }

        [Column("account_id")]
        public long AccountId { get; protected set; }

        [Column("account_is_active")]
        public long AccountIsActive { get; protected set; }

        [Column("is_include_into_totals")]
        public bool AccountIsIncludeInTotals { get; protected set; }

        [Column("account_type")]
        public string AccountType { get; protected set; }

        [Column("sort_order")]
        public long SortOrder { get; protected set; }

        [DisplayName("Balance")]
        [Column("balance")]
        public double? Balance { get; protected set; }

        public string BalanceStr => $"{Balance}{BalanceSymbol}";

        public string DefaultCurrencyBalanceStr => $"{DefaultCurrencyBalance}{DefaultCurrencySymbol}";

        [DisplayName("sign")]
        [Column("symbol")]
        public string BalanceSymbol { get; protected set; }

        [DisplayName("total_home_currency")]
        [Column("balance_default_crr")]
        public double? DefaultCurrencyBalance { get; protected set; }
        [DisplayName("total_usd")]
        [Column("balance_usd")]
        public double? USDBalance { get; protected set; }

        [Column("default_crr_symbol")]
        public string DefaultCurrencySymbol { get; protected set; }

        [Column("date")]
        [DisplayName("date")]
        public string Date { get; protected set; }
    }
}
