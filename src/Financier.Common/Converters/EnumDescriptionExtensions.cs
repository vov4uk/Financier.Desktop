using System;
using System.ComponentModel;
using System.Reflection;
using Financier.Common.Attribute;

namespace Financier.Converters
{
    public static class EnumDescriptionExtensions
    {
        public static string GetEnumDescription(this Enum enumObj)
        {
            FieldInfo fieldInfo = enumObj.GetType().GetField(enumObj.ToString());
            DescriptionAttribute attrib = fieldInfo.GetCustomAttribute<DescriptionAttribute>();
            return attrib?.Description ?? enumObj.ToString();
        }

        public static string GetEnumLocalizedDescription(this Enum enumObj)
        {
            FieldInfo fieldInfo = enumObj.GetType().GetField(enumObj.ToString());
            LocalizedMccDescriptionAttribute attrib = fieldInfo.GetCustomAttribute<LocalizedMccDescriptionAttribute>();
            return attrib?.Description ?? enumObj.ToString();
        }
    }
}
