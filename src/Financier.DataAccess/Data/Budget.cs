using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace Financier.DataAccess.Data
{
    [DebuggerDisplay("{Title}")]
    [Table(Backup.BUDGET_TABLE)]
    public class Budget : Entity, IIdentity
    {
        [Column(Backup.IdColumn)]
        public int Id { get; set; } = -1;

        [Column(Backup.TitleColumn)]
        public string Title { get; set; }

        [Column("category_id")]
        public string Categories { get; set; }

        [Column("project_id")]
        public string ProjectId { get; set; }

        [Column("currency_id")]
        public long CurrencyId { get; set; } = -1;

        [Column("budget_currency_id")]
        public int? Currency { get; set; }

        [Column("budget_account_id")]
        public int? Account { get; set; }

        [Column("amount")]
        public long Amount { get; set; }

        [Column("include_subcategories")]
        public bool IncludeSubcategories { get; set; }

        [Column("expanded")]
        public bool Expanded { get; set; }

        [Column("include_credit")]
        public bool IncludeCredit { get; set; } = true;

        [Column("start_date")]
        public long StartDate { get; set; }

        [Column("end_date")]
        public long EndDate { get; set; }

        [Column("recur")]
        public string Recur { get; set; }

        [Column("recur_num")]
        public long RecurNum { get; set; }

        [Column("is_current")]
        public bool IsCurrent { get; set; }

        [Column("parent_budget_id")]
        public long ParentBudgetId { get; set; }

        [Column(Backup.UpdatedOnColumn)]
        public long UpdatedOn { get; set; }

        [Column("remote_key")]
        public string RemoteKey { get; set; }
    }
}