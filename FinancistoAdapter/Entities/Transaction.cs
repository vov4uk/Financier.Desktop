using FinancistoAdapter.Converters;
using System;

namespace FinancistoAdapter.Entities
{
    [Entity(Backup.TRANSACTION_TABLE)]
    public class Transaction : Entity, IIdentity
    {
        [EntityProperty(IdColumn)]
        public int Id { get; set; } = -1;

        [EntityProperty("from_account_id")]
        public int From { get; set; }

        [EntityProperty("to_account_id")]
        public int To { get; set; }

        [EntityProperty("category_id")]
        public int Category { get; set; }

        [EntityProperty("note")]
        public string Note { get; set; }

        [EntityProperty("datetime", Converter = typeof(DateTimeConverter))]
        public DateTime? DateTime { get; set; }

        [EntityProperty("from_amount", Converter = typeof(AmountConverter))]
        public double FromAmount { get; set; }

        [EntityProperty("to_amount", Converter = typeof(AmountConverter))]
        public double ToAmount { get; set; }

        [EntityProperty("payee_id")]
        public int Payee { get; set; }

        [EntityProperty("project_id")]
        public int Project { get; set; }

        [EntityProperty("location_id")]
        public int Location { get; set; }

        [EntityProperty("parent_id")]
        public int Parent { get; set; }

        [EntityProperty("blob_key")]
        public string BlobKey { get; set; }

        [EntityProperty("original_currency_id")]
        public long OriginalCurrencyId { get; set; }

        [EntityProperty("original_from_amount")]
        public long OriginalFromAmmount { get; set; }

        [EntityProperty(UpdatedOnColumn)]
        public long UpdatedOn { get; set; }

        [EntityProperty("last_recurrence")]
        public long LastRecurrence { get; set; }

        [EntityProperty("is_ccard_payment")]
        public bool IsCcardPayment { get; set; }

        [EntityProperty("is_template")]
        public bool IsTemplate { get; set; }

        [EntityProperty("status")]
        public string Status { get; set; }

        [EntityProperty("latitude")]
        public string Latitude { get; set; }

        [EntityProperty("longitude")]
        public string Longitude { get; set; }

        [EntityProperty("accuracy")]
        public string Accuracy { get; set; }

        [EntityProperty("provider")]
        public string Provider { get; set; }

        [EntityProperty("template_name")]
        public string TemplateName { get; set; }

    }
}
