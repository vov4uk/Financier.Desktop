using Financier.DataAccess.Data;
using Financier.Adapter.Converters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Financier.DataAccess.Utils;

namespace Financier.Adapter
{
    public class EntityReader : IEntityReader
    {
        private static readonly Lazy<IReadOnlyDictionary<string, EntityInfo>> _entityTypes =
            new(BuildEntityTypes, System.Threading.LazyThreadSafetyMode.ExecutionAndPublication);

        public async Task<(IEnumerable<Entity> Entities, BackupVersion BackupVersion, Dictionary<string, List<string>> EntityColumnsOrder)> ParseBackupFileAsync(string fileName)
        {
            Dictionary<string, List<string>> EntityColumnsOrder = new Dictionary<string, List<string>>();
            Dictionary<string, HashSet<string>> columnsSeen = new Dictionary<string, HashSet<string>>();

            using var reader = new BackupReader(fileName);
            List<Entity> entities = new List<Entity>();

            var entityTypes = _entityTypes.Value;
            Entity entity = null!;
            EntityInfo entityInfo = null!;
            string prevField = string.Empty;
            string entityType = string.Empty;

            await foreach (var raw in reader.GetLinesAsync())
            {
                Line line = new Line(raw);
                if (line.Key == Backup.ENTITY)
                {
                    prevField = string.Empty;
                    entityType = line.Value!;
                    if (!string.IsNullOrEmpty(line.Value) && entityTypes.TryGetValue(line.Value, out entityInfo!))
                    {
                        entity = entityInfo.Factory();
                    }

                    if (!EntityColumnsOrder.ContainsKey(entityType))
                    {
                        EntityColumnsOrder.Add(entityType, new List<string>());
                        columnsSeen.Add(entityType, new HashSet<string>());
                    }
                }
                else if (line.Key == Backup.ENTITY_END && entity != null)
                {
                    entities.Add(entity);
                    entity = null!;
                    entityType = string.Empty;
                }
                else if (entity != null && line.Value != null)
                {
                    if (entityInfo.Properties.TryGetValue(line.Key!, out var property))
                    {
                        property.SetValue(entity, line.Value);
                    }

                    var order = EntityColumnsOrder[entityType];
                    if (columnsSeen[entityType].Add(line.Key!))
                    {
                        order.Insert(order.IndexOf(prevField) + 1, line.Key!);
                    }
                    prevField = line.Key!;
                }
            }

            return (entities, reader.BackupVersion, EntityColumnsOrder);
        }

        private static IReadOnlyDictionary<string, EntityInfo> BuildEntityTypes()
        {
            Type entityBaseType = typeof(Entity);
            Dictionary<string, EntityInfo> entities = new Dictionary<string, EntityInfo>();
            IEnumerable<Type> types = entityBaseType.Assembly
                .GetTypes()
                .Where(entityBaseType.IsAssignableFrom);
            foreach (Type t in types)
            {
                TableAttribute attr = t.GetCustomAttribute<TableAttribute>();
                if (attr != null)
                {
                    EntityInfo info = new EntityInfo
                    {
                        EntityType = t,
                        Factory = BuildFactory(t)
                    };
                    entities[attr.Name] = info;
                    foreach (PropertyInfo p in t.GetProperties())
                    {
                        var ignore = p.GetCustomAttribute<IgnoreAttribute>();
                        if (ignore == null)
                        {
                            var columnAttr = p.GetCustomAttribute<ColumnAttribute>();
                            if (columnAttr != null)
                            {
                                EntityPropertyInfo pInfo = new EntityPropertyInfo(p)
                                {
                                    Converter = new DefaultConverter { PropertyType = p.PropertyType }
                                };
                                info.Properties[columnAttr.Name] = pInfo;
                            }
                        }
                    }
                }
            }

            return new ReadOnlyDictionary<string, EntityInfo>(entities);
        }

        private static Func<Entity> BuildFactory(Type type)
        {
            var ctor = type.GetConstructor(Type.EmptyTypes)!;
            var newExpr = Expression.New(ctor);
            var cast = Expression.Convert(newExpr, typeof(Entity));
            return Expression.Lambda<Func<Entity>>(cast).Compile();
        }
    }
}
