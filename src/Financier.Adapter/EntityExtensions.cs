using Financier.DataAccess.Data;
using Financier.Adapter.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Financier.Adapter
{
    public static class EntityExtensions
    {
        public static string ToBackupLines(this Entity entity, Dictionary<string, List<string>> entityColumnsOrder)
        {
            var sb = new StringBuilder();

            Type type = entity.GetType();
            TableAttribute classArttr = type.GetCustomAttributes().OfType<TableAttribute>().FirstOrDefault();
            if (classArttr == null)
            {
                return string.Empty;
            }

            sb.AppendLine($"{Backup.ENTITY}:{classArttr.Name}");

            var dict = new List<KeyValuePair<int, string>>();
            List<string> columnsOrder = entityColumnsOrder[classArttr.Name];
            foreach (var propertyInfo in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                ColumnAttribute pattr = (ColumnAttribute)propertyInfo.GetCustomAttribute(typeof(ColumnAttribute));
                if (pattr != null)
                {
                    EntityPropertyInfo pInfo = new EntityPropertyInfo(propertyInfo)
                    {
                        Converter = (IPropertyConverter)Activator.CreateInstance(typeof(DefaultConverter))
                    };
                    pInfo.Converter.PropertyType = propertyInfo.PropertyType;

                    object val = propertyInfo.GetValue(entity);
                    if (val != null)
                    {
                        dict.Add(new KeyValuePair<int, string>(columnsOrder.IndexOf(pattr.Name), $"{pattr.Name}:{pInfo.Converter.ConvertBack(val)}"));
                    }
                }
            }

            foreach (var pair in dict.OrderBy(x => x.Key))
            {
                sb.AppendLine(pair.Value);
            }
            sb.AppendLine(Backup.ENTITY_END);
            return sb.ToString();
        }
    }
}