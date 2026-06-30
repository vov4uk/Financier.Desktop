using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using Financier.DataAccess.Data;

namespace Financier.Common.Model
{
    [ExcludeFromCodeCoverage]
    public class CurrencyModel : BaseModel
    {
        private const string DefaultFormat = "#,##0.00";

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

        [Column("is_active")]
        public bool IsActive { get; set; }

        [Column("symbol_format")]
        public string SymbolFormat { get; set; }

        [Column("number_format")]
        public string NumberFormat { get; set; }

        [Column("update_exchange_rate")]
        public bool UpdateExchangeRate { get; set; }

        [Column("decimals")]
        public int Decimals { get; set; }

        [Column("decimal_separator")]
        public string DecimalSeparator { get; set; }

        [Column("group_separator")]
        public string GroupSeparator { get; set; }

        public CurrencyModel() { }

        public CurrencyModel(Currency currency)
        {
            Id = currency.Id;
            IsDefault = currency.IsDefault;
            IsActive = currency.IsActive;
            Name = currency.Name;
            Title = currency.Title;
            Symbol = currency.Symbol;
            SymbolFormat = currency.SymbolFormat;
            NumberFormat = currency.NumberFormat;
            DecimalSeparator = currency.DecimalSeparator;
            GroupSeparator = currency.GroupSeparator;
            Decimals = currency.Decimals;
        }

        [NotMapped]
        private volatile NumberFormatInfo format;

        public NumberFormatInfo getFormat()
        {
            NumberFormatInfo f = format;
            if (f == null)
            {
                f = CreateCurrencyFormat(this);
                format = f;
            }
            return f;
        }

        public static NumberFormatInfo CreateCurrencyFormat(CurrencyModel c)
        {
            string numberFormat = !string.IsNullOrEmpty(c.NumberFormat)
                ? c.NumberFormat
                : DefaultFormat;

            var nfi = new NumberFormatInfo();

            // Decimal separator
            if (!string.IsNullOrEmpty(c.DecimalSeparator))
                nfi.NumberDecimalSeparator = c.DecimalSeparator.Trim('\'');

            // Group (thousands) separator
            if (!string.IsNullOrEmpty(c.GroupSeparator))
                nfi.NumberGroupSeparator = c.GroupSeparator.Trim('\'');
            else
                nfi.NumberGroupSeparator = string.Empty;

            // Grouping used — mirrors df.setGroupingUsed(groupSeparator > 0)
            nfi.NumberGroupSizes = string.IsNullOrEmpty(nfi.NumberGroupSeparator)
                ? new[] { 0 }
                : ParseGroupSizes(numberFormat);

            nfi.NumberDecimalDigits = c.Decimals;
            nfi.CurrencySymbol = c.Symbol;

            return nfi;
        }

        /// <summary>
        /// Parses group sizes from a Java-style decimal pattern, e.g. "#,##,##0.00" → [2, 2]
        /// or "#,##0.00" → [3]. Supports Indian-style two-interval grouping.
        /// Falls back to [3] on any parse failure.
        /// </summary>
        private static int[] ParseGroupSizes(string pattern)
        {
            try
            {
                // Strip suffix (everything after the decimal point)
                int dotIndex = pattern.IndexOf('.');
                string intPart = dotIndex >= 0 ? pattern[..dotIndex] : pattern;

                // Find the integer part between the last and second-last comma
                string[] groups = intPart.Split(',');
                if (groups.Length < 2)
                    return new[] { 3 };

                // Last group = primary group size (rightmost before decimal)
                // Second-to-last = secondary group size (Indian style)
                int primary = groups[^1].Count(ch => ch == '#' || ch == '0');

                if (groups.Length >= 3)
                {
                    int secondary = groups[^2].Count(ch => ch == '#' || ch == '0');
                    return new[] { primary, secondary };
                }

                return new[] { primary };
            }
            catch
            {
                return new[] { 3 };
            }
        }
    }
}
