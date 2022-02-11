using Financier.DataAccess.Data;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Financier.Common.Model
{
    [ExcludeFromCodeCoverage]
    public class CurrencyModel : BaseModel
    {
        [Column("_id")]
        public int? Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("title")]
        public string Title { get; set; }

        [Column("symbol")]
        public string Symbol { get; set; }

        [Column("is_default")]
        public bool IsDefault { get; set; }

        public CurrencyModel() { }

        public CurrencyModel(Currency currency)
        {
            Id = currency.Id;
            IsDefault = currency.IsDefault;
            Name = currency.Name;
            Title = currency.Title;
            Symbol = currency.Symbol;
        }
    }
}