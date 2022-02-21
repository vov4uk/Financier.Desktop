using System.Windows;

namespace Financier.Converters
{
    public sealed class InvertedBooleanToVisibilityConverter : BooleanConverter<Visibility>
    {
        public InvertedBooleanToVisibilityConverter() :
            base(Visibility.Collapsed, Visibility.Visible)
        { }
    }
}
