using System.Diagnostics;

namespace FinancistoAdapter.Entities
{
    [DebuggerDisplay("{Title}")]
    [Entity(Backup.CURRENCY_TABLE)]
    public class Currency : Entity, IIdentity
    {
        [EntityProperty(IdColumn)]
        public int Id { get; set; } = -1;

        [EntityProperty(IsActiveColumn )]
        public bool IsActive { get; set; } = true;

        [EntityProperty(TitleColumn)]
        public string Title { get; set; }

        [EntityProperty("name")]
        public string Name { get; set; }

        [EntityProperty("symbol")]
        public string Symbol { get; set; }

        [EntityProperty("symbol_format")]

        public string SymbolFormat { get; set; }// = SymbolFormat.RS;

        [EntityProperty("is_default")]
        public bool IsDefault { get; set; }

        [EntityProperty("decimals")]
        public int Decimals { get; set; } = 2;

        [EntityProperty("decimal_separator")]
        public string DecimalSeparator { get; set; }

        [EntityProperty("group_separator")]
        public string GroupSeparator { get; set; }

        [EntityProperty(UpdatedOnColumn)]
        public long UpdatedOn { get; set; }
    }
}
