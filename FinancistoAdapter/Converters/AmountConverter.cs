namespace FinancistoAdapter.Converters
{
    public class AmountConverter : CustomConverter
    {
        protected override object PerformConvertion(string value)
        {
            double d = double.Parse(value);
            return d / 100;
        }

        protected override string PerformReverseConvertion(object value)
        {
            double d = (double)value;
            return ((long)(d * 100)).ToString();
        }
    }
}
