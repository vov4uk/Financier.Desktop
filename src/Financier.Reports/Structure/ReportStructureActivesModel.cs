using Financier.Common.Model;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Financier.Reports
{
    public class ReportStructureActivesModel : BaseModel
    {
        [Column("account_title")]
        [DisplayName("Account")]
        public string Title { get; protected set; }

        [Column("account_id")]
        public long AccountId { get; protected set; }

        [Column("account_is_active")]
        public long AccountIsActive { get; protected set; }

        [Column("is_include_into_totals")]
        public long AccountIsIncludeInTotals { get; protected set; }

        [Column("sort_order")]
        public long SortOrder { get; protected set; }

        [DisplayName("Balance")]
        [Column("balance")]
        public double? Balance { get; protected set; }

        public string BalanceStr => $"{Balance}{BalanceSymbol}";

        public string DefaultCurrencyBalanceStr => $"{DefaultCurrencyBalance}{DefaultCurrencySymbol}";

        [DisplayName("Sign")]
        [Column("symbol")]
        public string BalanceSymbol { get; protected set; }

        [DisplayName("Total (home currency)")]
        [Column("balance_default_crr")]
        public double? DefaultCurrencyBalance { get; protected set; }

        [Column("default_crr_symbol")]
        public string DefaultCurrencySymbol { get; protected set; }

        [Column("date")]
        [DisplayName("Date")]
        public string Date { get; protected set; }
    }
}