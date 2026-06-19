using Financier.DataAccess.Data;
using Financier.Adapter.Converters;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using Financier.DataAccess.Utils;

namespace Financier.Adapter
{
    public static class EntityExtensions
    {
        private record struct ColumnInfo(string Col, PropertyInfo Prop, IPropertyConverter Conv);
        private record struct TypeInfo(string TableName, ColumnInfo[] Columns);

        private static readonly ConcurrentDictionary<Type, TypeInfo> _typeCache = new();

        public static string ToBackupLines(this Entity entity, Dictionary<string, List<string>> entityColumnsOrder)
        {
            Type type = entity.GetType();
            TypeInfo info = _typeCache.GetOrAdd(type, BuildTypeInfo);

            if (info.TableName == string.Empty)
                return string.Empty;

            List<string> columnsOrder = entityColumnsOrder[info.TableName];
            var dict = new List<KeyValuePair<int, string>>(info.Columns.Length);

            foreach (ColumnInfo col in info.Columns)
            {
                object val = col.Prop.GetValue(entity);
                if (val != null)
                {
                    dict.Add(new KeyValuePair<int, string>(columnsOrder.IndexOf(col.Col), $"{col.Col}:{col.Conv.ConvertBack(val)}"));
                }
            }

            var sb = new StringBuilder();
            sb.AppendLine($"{Backup.ENTITY}:{info.TableName}");
            foreach (var pair in dict.OrderBy(x => x.Key))
            {
                sb.AppendLine(pair.Value);
            }
            sb.AppendLine(Backup.ENTITY_END);
            return sb.ToString();
        }

        private static TypeInfo BuildTypeInfo(Type type)
        {
            string tableName = type.GetCustomAttributes().OfType<TableAttribute>().FirstOrDefault()?.Name;
            if (tableName == null)
                return new TypeInfo(string.Empty, Array.Empty<ColumnInfo>());

            ColumnInfo[] columns = type
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.GetCustomAttribute<IgnoreAttribute>() == null)
                .Select(p => (Attr: p.GetCustomAttribute<ColumnAttribute>(), Prop: p))
                .Where(x => x.Attr != null)
                .Select(x => new ColumnInfo(
                    x.Attr!.Name!,
                    x.Prop,
                    new DefaultConverter { PropertyType = x.Prop.PropertyType }))
                .ToArray();

            return new TypeInfo(tableName, columns);
        }
    }
}
