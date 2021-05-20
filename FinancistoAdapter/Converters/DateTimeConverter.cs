using System;

namespace FinancistoAdapter.Converters
{
    public class DateTimeConverter : CustomConverter
    {
        private DateTime StartDate = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        protected override object PerformConvertion(string value)
        {
            double timestamp = double.Parse(value);
            return StartDate.AddMilliseconds(timestamp);
        }

        protected override string PerformReverseConvertion(object value)
        {
            var date = (DateTime)value;
            return (date - StartDate).TotalMilliseconds.ToString();
        }
    }
}
