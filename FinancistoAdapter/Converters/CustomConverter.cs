using System;

namespace FinancistoAdapter.Converters
{
    public abstract class CustomConverter : IPropertyConverter
    {
        public Type PropertyType { get; set; }

        protected abstract object PerformConvertion(string value);

        protected abstract string PerformReverseConvertion(object value);

        public object Convert(object value)
        {
            string s = value as string;
            if (s == null) throw new NotSupportedException("Only string values are supported by this convertor.");
            return PerformConvertion(s);
        }

        public string ConvertBack(object value)
        {
            if (value == null) throw new NotSupportedException("Only string values are supported by this convertor.");
            return PerformReverseConvertion(value);
        }
    }
}
