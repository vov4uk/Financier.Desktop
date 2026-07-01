using System;
using System.ComponentModel;
using System.Text;
using Financier.Converters;

namespace Financier.Common.Entities
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum SymbolFormat
    {
        [Description("100.00 $")]
        RS, // Right with space:  "100.00 $"
        [Description("100.00$")]
        R,  // Right no space:    "100.00$"
        [Description("$ 100.00")]
        LS, // Left with space:   "$ 100.00"
        [Description("$100.00")]
        L   // Left no space:     "$100.00"
    }

    public static class SymbolFormatExtensions
    {
        public static void AppendSymbol(this SymbolFormat format, StringBuilder sb, string symbol)
        {
            switch (format)
            {
                case SymbolFormat.RS:
                    sb.Append(' ').Append(symbol);
                    break;

                case SymbolFormat.R:
                    sb.Append(symbol);
                    break;

                case SymbolFormat.LS:
                    int offsetLs = GetInsertIndex(sb);
                    sb.Insert(offsetLs, ' ').Insert(offsetLs, symbol);
                    break;

                case SymbolFormat.L:
                    sb.Insert(GetInsertIndex(sb), symbol);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(format), format, null);
            }
        }

        /// <summary>
        /// Mirrors Java's getInsertIndex — skips any leading minus sign so the
        /// symbol is inserted before the digits but after the sign: "-$100.00"
        /// </summary>
        private static int GetInsertIndex(StringBuilder sb)
        {
            return sb.Length > 0 && sb[0] == '-' ? 1 : 0;
        }
    }
}
