using Financier.DataAccess.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace Financier.DataAccess.View
{
    public abstract class TransactionsView : Entity
    {
        [Column(Backup.IdColumn)]
        public int Id { get; set; }

        [Column("parent_id")]
        public int ParentId { get; set; }

        [Column("from_account_id")]
        public int FromAccountId { get; set; }

        [Column("from_account_title")]
        public string FromAccountTitle { get; set; }

        [Column("from_account_is_include_into_totals")]
        public bool FromAccountIsIncludeIntoTotals { get; set; }

        [ForeignKey("FromAccountCurrency")]
        [Column("from_account_currency_id")]
        public int FromAccountCurrencyId { get; set; }

        [Column("to_account_id")]
        public int? ToAccountId { get; set; }

        [Column("to_account_title")]
        public string ToAccountTitle { get; set; }

        [ForeignKey("ToAccountCurrency")]
        [Column("to_account_currency_id")]
        public int? ToAccountCurrencyId { get; set; }

        [ForeignKey("Category")]
        [Column("category_id")]
        public int? CategoryId { get; set; }

        [Column("category_title")]
        public string CategoryTitle { get; set; }

        [Column("category_left")]
        public int CategoryLeft { get; set; }

        [Column("category_right")]
        public int CategoryRight { get; set; }

        [Column("category_type")]
        public int CategoryType { get; set; }

        [Column("project_id")]
        public int? ProjectId { get; set; }

        [Column("project")]
        public string Project { get; set; }

        [Column("location_id")]
        public int? LocationId { get; set; }

        [Column("location")]
        public string Location { get; set; }

        [Column("payee_id")]
        public int? PayeeId { get; set; }

        [Column("payee")]
        public string Payee { get; set; }

        [Column("note")]
        public string Note { get; set; }

        [Column("from_amount")]
        public long FromAmount { get; set; }

        [Column("to_amount")]
        public long ToAmount { get; set; }

        [Column("datetime")]
        public long DateTime { get; set; }

        [ForeignKey("OriginalCurrency")]
        [Column("original_currency_id")]
        public int? OriginalCurrencyId { get; set; }

        [Column("original_from_amount")]
        public long OriginalFromAmount { get; set; }

        [Column("is_template")]
        public int IsTemplate { get; set; }

        [Column("template_name")]
        public string TemplateName { get; set; }

        [Column("recurrence")]
        public string Recurrence { get; set; }

        [Column("notification_options")]
        public string NotificationOptions { get; set; }

        [Column("status")]
        public string Status { get; set; }

        [Column("is_ccard_payment")]
        public int IsCcardPayment { get; set; }

        [Column("last_recurrence")]
        public long LastRecurrence { get; set; }

        [Column("attached_picture")]
        public string AttachedPicture { get; set; }

        [Column("from_account_balance")]
        public int? FromAccountBalance { get; set; }

        [Column("to_account_balance")]
        public int? ToAccountBalance { get; set; }

        [Column("is_transfer")]
        public long IsTransfer { get; set; }

        public virtual Currency FromAccountCurrency { get; set; }
        public virtual Currency ToAccountCurrency { get; set; }
        public virtual Currency OriginalCurrency { get; set; }
        public virtual Category Category { get; set; }
    }
}