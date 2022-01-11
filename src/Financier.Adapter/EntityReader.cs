using Financier.DataAccess.Data;
using Financier.Adapter.Converters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;

namespace Financier.Adapter
{
    public static class EntityReader
    {
        public static Dictionary<string, List<string>> EntityColumnsOrder = new Dictionary<string, List<string>>();
        public static BackupVersion BackupVersion { get; private set; } = new BackupVersion();

        private static IReadOnlyDictionary<string, EntityInfo> GetEntityTypes()
        {
            Type entityType = typeof(Entity);
            Dictionary<string, EntityInfo> entities = new Dictionary<string, EntityInfo>();
            IEnumerable<Type> types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(entityType.IsAssignableFrom);
            foreach (Type t in types)
            {
                TableAttribute attr = t.GetCustomAttributes(typeof(TableAttribute), true).Cast<TableAttribute>().FirstOrDefault();
                if (attr != null)
                {
                    EntityInfo info = new EntityInfo() { EntityType = t };
                    entities[attr.Name] = info;
                    foreach (PropertyInfo p in t.GetProperties())
                    {
                        ColumnAttribute pattr = (ColumnAttribute)p.GetCustomAttribute(typeof(ColumnAttribute));
                        if (pattr != null)
                        {
                            EntityPropertyInfo pInfo = new EntityPropertyInfo(p)
                            {
                                Converter = (IPropertyConverter)Activator.CreateInstance(typeof(DefaultConverter))
                            };
                            pInfo.Converter.PropertyType = p.PropertyType;
                            info.Properties[pattr.Name] = pInfo;
                        }
                    }
                }
            }

            return new ReadOnlyDictionary<string, EntityInfo>(entities);
        }

        public static IEnumerable<Entity> ParseBackupFile(string fileName)
        {
            using var reader = new BackupReader(fileName);
            List<Entity> entities = new List<Entity>();

            var entityTypes = GetEntityTypes();
            Entity entity = null;
            EntityInfo entityInfo = null;
            string prevField = string.Empty;
            string entityType = string.Empty;

            var lines = reader.GetLines().Select(s => new Line(s));
            foreach (Line line in lines)
            {
                if (line.Key == Entity.ENTITY)
                {
                    prevField = string.Empty;
                    entityType = line.Value;
                    if (!string.IsNullOrEmpty(line.Value) && entityTypes.TryGetValue(line.Value, out entityInfo))
                    {
                        entity = (Entity)Activator.CreateInstance(entityInfo.EntityType);
                    }
                    if (!EntityColumnsOrder.ContainsKey(entityType))
                    {
                        EntityColumnsOrder.Add(entityType, new List<string>());
                    }
                }
                else if (line.Key == Entity.END && entity != null)
                {
                    entities.Add(entity);
                    entity = null;
                    entityType = string.Empty;
                }
                else if (entity != null && line.Value != null)
                {
                    if (entityInfo != null && entityInfo.Properties.TryGetValue(line.Key, out var property))
                    {
                        property.SetValue(entity, line.Value);
                    }

                    var order = EntityColumnsOrder[entityType];
                    if (!order.Contains(line.Key))
                    {
                        var newOrder = order.IndexOf(prevField);
                        order.Insert(newOrder + 1, line.Key);
                    }
                    prevField = line.Key;
                }
            }

            return entities;
        }

    }
}