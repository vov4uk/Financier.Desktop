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
            FieldInfo fieldInfo = enumObj.GetType().GetField(enumObj.ToString())!;

            object[] attribArray = fieldInfo.GetCustomAttributes(false);

            if (attribArray.Length == 0)
            {
                return enumObj.ToString();
            }
            else
            {
                DescriptionAttribute attrib = (DescriptionAttribute)attribArray[0]!;
                return attrib.Description;
            }
        }
        public static string GetEnumLocalizedDescription(this Enum enumObj)
        {
            FieldInfo fieldInfo = enumObj.GetType().GetField(enumObj.ToString())!;

            object[] attribArray = fieldInfo.GetCustomAttributes(false);

            if (attribArray.Length == 0)
            {
                return enumObj.ToString();
            }
            else
            {
                LocalizedMccDescriptionAttribute attrib = (LocalizedMccDescriptionAttribute)attribArray.FirstOrDefault()!;
                return attrib?.Description ?? enumObj.ToString();
            }
        }
    }
}
