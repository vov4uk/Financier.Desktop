using System.ComponentModel;
using Financier.Common.Attribute;
using Financier.Converters;

namespace Financier.Common.Localization;

[TypeConverter(typeof(EnumDescriptionTypeConverter))]
public enum Language
{
    [LocalizedDescription("language_english")]
    English,
    [LocalizedDescription("language_ukrainian")]
    Ukrainian,
    [LocalizedDescription("language_polish")]
    Polish,
}
