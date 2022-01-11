using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace Financier.DataAccess.Data
{
    [DebuggerDisplay("{Title}")]
    [Table(Backup.CURRENCY_TABLE)]
    public class Currency : Entity, IIdentity
    {
        [Column(IdColumn)]
        public int Id { get; set; } = -1;

        [Column(IsActiveColumn)]
        public bool IsActive { get; set; } = true;

        [Column(TitleColumn)]
        public string Title { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("symbol")]
        public string Symbol { get; set; }

        [Column("symbol_format")]
        public string SymbolFormat { get; set; }// = SymbolFormat.RS;

        [Column("is_default")]
        public bool IsDefault { get; set; }

        [Column("decimals")]
        public int Decimals { get; set; } = 2;

        [Column("decimal_separator")]
        public string DecimalSeparator { get; set; }

        [Column("group_separator")]
        public string GroupSeparator { get; set; }

        [Column(UpdatedOnColumn)]
        public long UpdatedOn { get; set; }

        [NotMapped]
        public static Currency Empty => new Currency
        {
            Id = 0,
            Name = "",
            Title = "Default",
            Symbol = "",
            SymbolFormat = "RS",
            Decimals = 2,
            DecimalSeparator = "'.'",
            GroupSeparator = "','"
        };
    }
}