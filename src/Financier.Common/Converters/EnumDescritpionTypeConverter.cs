using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Financier.Converters
{
    public class EnumDescritpionTypeConverter
         : EnumConverter
    {
        public EnumDescritpionTypeConverter(Type type)
            : base(type)
        { }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value is Enum e && destinationType == typeof(string))
            {
                MemberInfo mi = e.GetType().GetTypeInfo().GetMember(e.ToString()).FirstOrDefault();
                if (mi != null)
                {
                    DescriptionAttribute attribute = mi.GetCustomAttribute<DescriptionAttribute>(false);
                    if (attribute != null)
                    {
                        string result = attribute.Description;
                        return result;
                    }
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
