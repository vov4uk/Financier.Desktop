using Financier.DataAccess.Data;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Financier.Common.Model
{
    [ExcludeFromCodeCoverage]
    public class CurrencyModel : BaseModel
    {
        [Column("_id")]
        public long? Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("title")]
        public string Title { get; set; }

        [Column("symbol")]
        public string Symbol { get; set; }

        [Column("is_default")]
        public long Is_Default { get; set; }

        public bool IsDefault => Is_Default == 1;

        public CurrencyModel() { }

        public CurrencyModel(Currency currency)
        {
            Id = currency.Id;
            Is_Default = currency.IsDefault ? 1 : 0;
            Name = currency.Name;
            Title = currency.Title;
            Symbol = currency.Symbol;
        }
    }
}