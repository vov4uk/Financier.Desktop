using System;
using System.Reflection;

namespace Financier.Reports.Common
{
    public class BaseReportModel
    {
        public void Init(object reader)
        {
            //foreach (PropertyInfo property in GetType().GetProperties())
            //{
            //    FieldAttribute customAttribute = Attribute.GetCustomAttribute(property, typeof(FieldAttribute)) as FieldAttribute;
            //    if (customAttribute != null)
            //    {
            //        int ordinal = reader.GetOrdinal(customAttribute.Name);
            //        object obj = ordinal != -1 ? reader.GetValue(ordinal) : throw new Exception(string.Format("В классе [{0}] определен атрибут несуществующего поля [{1}] в ридере", GetType(), customAttribute.Name));
            //        if (obj != DBNull.Value)
            //            property.SetValue(this, obj, null);
            //    }
            //}
        }
    }
}