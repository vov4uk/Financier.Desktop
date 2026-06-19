using System;
using System.Windows.Markup;

namespace Financier.Common.Localization;

public class EnumBinding : MarkupExtension
{
    public Type EnumType { get; }

    public EnumBinding(Type enumType)
    {
        if (enumType is null || !enumType.IsEnum)
        {
            throw new ArgumentException("Parameter enumType is null or is not enum.");
        }

        EnumType = enumType;
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        Array result = Enum.GetValues(EnumType);
        return result;
    }
}
