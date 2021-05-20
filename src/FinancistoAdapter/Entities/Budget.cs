using System.Diagnostics;

namespace FinancistoAdapter.Entities
{
    [DebuggerDisplay("{Title}")]
    [Entity(Backup.BUDGET_TABLE)]
    public class Budget : Entity, IIdentity
    {
        [EntityProperty(IdColumn)]
        public int Id { get; set; } = -1;

        [EntityProperty(TitleColumn)]
        public string Title { get; set; }

        [EntityProperty("category_id")]
        public string Categories { get; set; }

        [EntityProperty("project_id")]
        public string Projects { get; set; }

        [EntityProperty("currency_id")]
        public long CurrencyId { get; set; } = -1;

        [EntityProperty("budget_currency_id")]
        public int? Currency { get; set; }

        [EntityProperty("budget_account_id")]
        public int? Account { get; set; }

        [EntityProperty("amount")]
        public long Amount { get; set; }

        [EntityProperty("include_subcategories")]
        public bool IncludeSubcategories { get; set; }

        [EntityProperty("expanded")]
        public bool Expanded { get; set; }

        [EntityProperty("include_credit")]
        public bool IncludeCredit { get; set; } = true;

        [EntityProperty("start_date")]
        public long StartDate { get; set; }

        [EntityProperty("end_date")]
        public long EndDate { get; set; }

        [EntityProperty("recur")]
        public string Recur { get; set; }

        [EntityProperty("recur_num")]
        public long RecurNum { get; set; }

        [EntityProperty("is_current")]
        public bool IsCurrent { get; set; }

        [EntityProperty("parent_budget_id")]
        public long ParentBudgetId { get; set; }

        [EntityProperty(UpdatedOnColumn)]
        public long UpdatedOn { get; set; }

        [EntityProperty("remote_key")]
        public string RemoteKey { get; set; }
    }
}
