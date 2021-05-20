namespace FinancistoAdapter.Entities
{
    [Entity(Backup.CCARD_CLOSING_DATE_TABLE)]
    public class CCardClosingDate : Entity
    {
        [EntityProperty("account_id")]
        public int AccountId { get; set; }

        [EntityProperty("period")]
        public int Period { get; set; }

        [EntityProperty("closing_day")]
        public int ClosingDay { get; set; }
    }
}
