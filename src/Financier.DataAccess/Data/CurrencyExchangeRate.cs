using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Financier.DataAccess.Data
{
    [Table(Backup.EXCHANGE_RATES_TABLE)]
    public class CurrencyExchangeRate : Entity, IIdentity
    {

        [Key, Column("from_currency_id"), ForeignKey("FromCurrency")]
        public int FromCurrencyId { get; set; }

        [Key, Column("to_currency_id"), ForeignKey("ToCurrency")]
        public int ToCurrencyId { get; set; }

        [Key, Column("rate_date")]
        public long Date { get; set; }

        [Column("rate")]
        public float Rate { get; set; }

        [Column(UpdatedOnColumn)]
        public long UpdatedOn { get; set; }
        
        [Column("remote_key")]
        public long RemoteKey { get; set; }

        public Currency FromCurrency { get; set; }

        public Currency ToCurrency { get; set; }

        [NotMapped]
        public int Id { get; set; } = 1; // Need for backup, on backup export only items with id > 0
    }
}
