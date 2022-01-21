using Financier.Reports.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Financier.Reports.Reports
{
    public class ReportStructureActivesModel : BaseReportModel
    {
        [Column("account_title")]
        [DisplayName("Актив")]
        public string Title { get; protected set; }

        [Column("account_id")]
        public long AccountId { get; protected set; }

        [Column("account_is_active")]
        public long AccountIsActive { get; protected set; }

        [Column("is_include_into_totals")]
        public long AccountIsIncludeInTotals { get; protected set; }

        [Column("sort_order")]
        public long SortOrder { get; protected set; }

        [Column("balance")]
        public double? Balance { get; protected set; }

        [DisplayName("Баланс")]
        public string BalanceStr => $"{Balance}{BalanceSymbol}";

        [DisplayName("Всего в домашней валюте")]
        public string DefaultCurrencyBalanceStr => $"{DefaultCurrencyBalance}{DefaultCurrencySymbol}";

        [Column("symbol")]
        public string BalanceSymbol { get; protected set; }

        [Column("balance_default_crr")]
        public double? DefaultCurrencyBalance { get; protected set; }

        [Column("default_crr_symbol")]
        public string DefaultCurrencySymbol { get; protected set; }

        [Column("date")]
        [DisplayName("Дата")]
        public string Date { get; protected set; }
    }
}