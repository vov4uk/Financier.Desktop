using FinancistoAdapter.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FinancistoAdapter.Entities
{
    public abstract class Entity
    {
        public const string IdColumn = "_id";
        public const string TitleColumn = "title";
        public const string SortOrderColumn = "sort_order";
        public const string IsActiveColumn = "is_active";
        public const string UpdatedOnColumn = "updated_on";
        public const string END = "$$";
        public const string ENTITY = "$ENTITY";

        public string ToBackupLines()
        {
            var sb = new StringBuilder();

            Type type = this.GetType();
            EntityAttribute classArttr = type.GetCustomAttributes().OfType<EntityAttribute>().FirstOrDefault();
            if (classArttr == null)
            {
                return string.Empty;
            }

            sb.AppendLine($"{ENTITY}:{classArttr.EntityName}");

            var dict = new List<KeyValuePair<int, string>>();
            List<string> columnsOrder = EntityReader.EntityColumnsOrder[classArttr.EntityName];
            foreach (var propertyInfo in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                EntityPropertyAttribute pattr = (EntityPropertyAttribute)propertyInfo.GetCustomAttribute(typeof(EntityPropertyAttribute));
                if (pattr != null)
                {
                    EntityPropertyInfo pInfo = new EntityPropertyInfo(propertyInfo)
                    {
                        Converter = (IPropertyConverter) Activator.CreateInstance(pattr.Converter)
                    };
                    pInfo.Converter.PropertyType = propertyInfo.PropertyType;

                    object val = propertyInfo.GetValue(this);
                    if (val != null)
                    {
                        dict.Add(new KeyValuePair<int, string>(columnsOrder.IndexOf(pattr.Key), $"{pattr.Key}:{pInfo.Converter.ConvertBack(val)}"));
                    }
                }
            }

            foreach (var pair in dict.OrderBy(x => x.Key))
            {
                sb.AppendLine(pair.Value);
            }
            sb.AppendLine(END);
            return sb.ToString();
        }
    }
}
