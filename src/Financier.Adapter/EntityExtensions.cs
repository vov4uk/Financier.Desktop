using Financier.DataAccess.Data;
using Financier.Adapter.Converters;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Financier.DataAccess.Utils;

namespace Financier.Adapter
{
    public static class EntityExtensions
    {
        private record struct ColumnInfo(string Col, Func<Entity, object> GetValue, IPropertyConverter Conv);
        private record struct TypeInfo(string TableName, ColumnInfo[] Columns);

        private static readonly ConcurrentDictionary<Type, TypeInfo> _typeCache = new();

        public static void WriteBackupLines(
            this Entity entity,
            TextWriter writer,
            Dictionary<string, (Dictionary<string, int> Index, int Count)> allColumnData)
        {
            Type type = entity.GetType();
            TypeInfo info = _typeCache.GetOrAdd(type, BuildTypeInfo);

            if (info.TableName == string.Empty) return;
            if (!allColumnData.TryGetValue(info.TableName, out var colData)) return;

            var (columnIndex, columnCount) = colData;
            string[] lines = new string[columnCount];

            foreach (ColumnInfo col in info.Columns)
            {
                object val = col.GetValue(entity);
                if (val != null && columnIndex.TryGetValue(col.Col, out int colIdx))
                {
                    lines[colIdx] = $"{col.Col}:{col.Conv.ConvertBack(val)}";
                }
            }

            writer.WriteLine($"{Backup.ENTITY}:{info.TableName}");
            foreach (string line in lines)
            {
                if (line != null)
                    writer.WriteLine(line);
            }
            writer.WriteLine(Backup.ENTITY_END);
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
                    BuildGetter(x.Prop),
                    new DefaultConverter { PropertyType = x.Prop.PropertyType }))
                .ToArray();

            return new TypeInfo(tableName, columns);
        }

        private static Func<Entity, object> BuildGetter(PropertyInfo prop)
        {
            var entityParam = Expression.Parameter(typeof(Entity), "e");
            var castEntity = Expression.Convert(entityParam, prop.DeclaringType!);
            var access = Expression.Property(castEntity, prop);
            var boxed = Expression.Convert(access, typeof(object));
            return Expression.Lambda<Func<Entity, object>>(boxed, entityParam).Compile();
        }
    }
}
