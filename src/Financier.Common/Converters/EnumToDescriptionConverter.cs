using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows.Data;

namespace Financier.Converters
{
    public class EnumToDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            if (value is Enum e)
            {
                MemberInfo mi = e.GetType().GetTypeInfo().GetMember(e.ToString()).FirstOrDefault();
                if (mi != null)
                {
                    DescriptionAttribute attribute = mi.GetCustomAttribute<DescriptionAttribute>(false);
                    if (attribute != null)
                    {
                        return attribute.Description;
                    }
                }

                return e.ToString();
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
                return null;

            if (!targetType.IsEnum)
                throw new ArgumentException("Target type must be an enum type.");

            string stringValue = value.ToString();

            // Try to find enum value by description
            var fields = targetType.GetFields(BindingFlags.Public | BindingFlags.Static);
            foreach (var field in fields)
            {
                DescriptionAttribute attribute = field.GetCustomAttribute<DescriptionAttribute>(false);
                if (attribute?.Description == stringValue)
                {
                    return field.GetValue(null);     
                }
            }

            // If no description matches, try to find by name
            try
            {
                return Enum.Parse(targetType, stringValue, ignoreCase: true);
            }
            catch
            {
                return null;
            }
        }
    }
}
