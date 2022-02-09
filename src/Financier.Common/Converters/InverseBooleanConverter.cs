using System.Windows.Data;

namespace Financier.Converters
{
    [ValueConversion(typeof(bool), typeof(bool))]
    public class InverseBooleanConverter : BooleanConverter<bool>
    {
        public InverseBooleanConverter() :
            base(false, true)
        { }
    }
}
