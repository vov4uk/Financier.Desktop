using System.Diagnostics;

namespace FinancistoAdapter.Entities
{
    [DebuggerDisplay("{Title}")]
    [Entity(Backup.ACCOUNT_TABLE)]
    public class Account : Entity, IIdentity
    {
        [EntityProperty(IdColumn)]
        public int Id { get; set; } = -1;

        [EntityProperty(IsActiveColumn )]
        public bool IsActive { get; set; } = true;

        [EntityProperty(TitleColumn)]
        public string Title { get; set; }

        [EntityProperty("creation_date")]
        public long CreationDate { get; set; }

        [EntityProperty("last_transaction_date")]
        public long LastTransactionDate { get; set; }

        [EntityProperty("currency_id")]
        public int CurrencyId { get; set; }

        [EntityProperty("type")]
        public string Type { get; set; }

        [EntityProperty("card_issuer")]
        public string CardIssuer { get; set; }

        [EntityProperty("issuer")]
        public string Issuer { get; set; }

        [EntityProperty("number")]
        public string Number { get; set; }

        [EntityProperty("total_amount")]
        public long TotalAmount { get; set; }

        [EntityProperty("total_limit")]
        public long LimitAmount { get; set; }

        [EntityProperty(SortOrderColumn)]
        public int SortOrder { get; set; }

        [EntityProperty("is_include_into_totals")]
        public bool IsIncludeIntoTotals { get; set; } = true;

        [EntityProperty("last_account_id")]
        public long LastAccountId { get; set; }

        [EntityProperty("last_category_id")]
        public long LastCategoryId { get; set; }

        [EntityProperty("closing_day")]
        public int ClosingDay { get; set; }

        [EntityProperty("payment_day")]
        public int PaymentDay { get; set; }

        [EntityProperty("note")]
        public string Note { get; set; }

        [EntityProperty(UpdatedOnColumn)]
        public long UpdatedOn { get; set; }
    }
}
