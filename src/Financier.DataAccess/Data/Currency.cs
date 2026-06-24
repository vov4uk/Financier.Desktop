using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Text;

namespace Financier.DataAccess.Data
{
    [DebuggerDisplay("{Title}")]
    [Table(Backup.CURRENCY_TABLE)]
    public class Currency : Entity, IIdentity
    {
        [Column(Backup.IdColumn)]
        public int Id { get; set; } = -1;

        [Column(Backup.IsActiveColumn)]
        public bool IsActive { get; set; } = true;

        [Column(Backup.TitleColumn)]
        public string Title { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("symbol")]
        public string Symbol { get; set; }

        [Column("symbol_format")]
        public string SymbolFormat { get; set; }

        [Column("is_default")]
        public bool IsDefault { get; set; }

        [Column("decimals")]
        public int Decimals { get; set; } = 2;

        [Column("decimal_separator")]
        public string DecimalSeparator { get; set; }

        [Column("group_separator")]
        public string GroupSeparator { get; set; }

        [Column("number_format")]
        public string NumberFormat;

        [Column("update_exchange_rate")]
        public bool UpdateExchangeRate;

        [Column(Backup.UpdatedOnColumn)]
        public long UpdatedOn { get; set; }


        public static Currency defaultCurrency()
        {
            Currency c = new Currency
            {
                Id = 2,
                Name = "USD",
                Title = "American Dollar",
                Symbol = "$",
                Decimals = 2
            };
            return c;
        }

        public static Currency EMPTY = new Currency();

        static Currency() {
            EMPTY.Id = 0;
            EMPTY.Name = "";
            EMPTY.Title = "Default";
            EMPTY.Symbol = "";
            EMPTY.SymbolFormat = "RS";
            EMPTY.Decimals = 2;
            EMPTY.DecimalSeparator = "'.'";
            EMPTY.GroupSeparator = "','";
        }
    }
}
