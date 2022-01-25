using System.Windows;

namespace Financier.Reports.Converters
{
    public sealed class InvertedBooleanToVisibilityConverter : BooleanConverter<Visibility>
    {
        public InvertedBooleanToVisibilityConverter() :
            base(Visibility.Collapsed, Visibility.Visible)
        { }
    }
}
