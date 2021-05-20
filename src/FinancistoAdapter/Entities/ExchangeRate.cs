namespace FinancistoAdapter.Entities
{
    [Entity(Backup.EXCHANGE_RATES_TABLE)]
    public class ExchangeRate : Entity
    {

        [EntityProperty("from_currency_id")]
        public int FromCurrency { get; set; }

        [EntityProperty("to_currency_id")]
        public int ToCurrency { get; set; }

        [EntityProperty("rate_date")]
        public long Date { get; set; }

        [EntityProperty("rate")]
        public string Rate { get; set; }

        [EntityProperty(UpdatedOnColumn)]
        public long UpdatedOn { get; set; }

    }
}
