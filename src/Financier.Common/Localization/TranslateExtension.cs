using System;
using System.Windows.Data;
using System.Windows.Markup;

namespace Financier.Common.Localization;

/// <summary>
/// WPF MarkupExtension that binds a UI property to a localized string via
/// <see cref="LocalizationService"/>.
/// </summary>
/// <example>
/// <!-- Declare the namespace in a Window/UserControl/App.xaml -->
/// xmlns:loc="clr-namespace:Financier.Common.Localization;assembly=Financier.Common"
///
/// <![CDATA[
/// <Button Content="{loc:Translate Key=Save}" />
/// <TextBlock Text="{loc:Translate Key=AppTitle}" />
/// ]]>
/// </example>
[MarkupExtensionReturnType(typeof(string))]
public class TranslateExtension : MarkupExtension
{
    public string Key { get; set; }

    public override object ProvideValue(IServiceProvider sp)
    {
        // Returns a binding to a localization service
        return new Binding($"[{Key}]") { Source = LocalizationService.Instance }.ProvideValue(sp);
    }
}
