using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Financier.Common.Attribute;

namespace Financier.Converters
{
    public static class EnumDescriptionExtentions
    {
        public static string GetEnumDescription(this Enum enumObj)
        {
            FieldInfo fieldInfo = enumObj.GetType().GetField(enumObj.ToString());

            object[] attribArray = fieldInfo.GetCustomAttributes(false);

            if (attribArray.Length == 0)
            {
                return enumObj.ToString();
            }
            else
            {
                DescriptionAttribute attrib = attribArray[0] as DescriptionAttribute;
                return attrib.Description;
            }
        }
        public static string GetEnumLocalizedDescription(this Enum enumObj)
        {
            FieldInfo fieldInfo = enumObj.GetType().GetField(enumObj.ToString());

            object[] attribArray = fieldInfo.GetCustomAttributes(false);

            if (attribArray.Length == 0)
            {
                return enumObj.ToString();
            }
            else
            {
                LocalizedMccDescriptionAttribute attrib = attribArray.FirstOrDefault() as LocalizedMccDescriptionAttribute;
                return attrib?.Description ?? enumObj.ToString();
            }
        }
    }
}
