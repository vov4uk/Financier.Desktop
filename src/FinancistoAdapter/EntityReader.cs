using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using FinancistoAdapter.Converters;
using FinancistoAdapter.Entities;

namespace FinancistoAdapter
{
    public static class EntityReader
    {
        public static Dictionary<string, List<string>> EntityColumnsOrder = new Dictionary<string, List<string>>();
        private static string _package;
        private static int _versionCode;
        private static Version _version;
        private static int _dbVersion;

        public static string Package { get { return _package; } }
        public static int VersionCode { get { return _versionCode; } }
        public static Version Version { get { return _version; } }
        public static int DatabaseVersion { get { return _dbVersion; } }

        private static IReadOnlyDictionary<string, EntityInfo> GetEntityTypes()
        {
            Type entityType = typeof(Entity);
            Dictionary<string, EntityInfo> entities = new Dictionary<string, EntityInfo>();
            IEnumerable<Type> types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(entityType.IsAssignableFrom);
            foreach (Type t in types)
            {
                EntityAttribute attr = t.GetCustomAttributes(typeof(EntityAttribute), true).Cast<EntityAttribute>().FirstOrDefault();
                if (attr != null)
                {
                    EntityInfo info = new EntityInfo() { EntityType = t };
                    entities[attr.EntityName] = info;
                    foreach (PropertyInfo p in t.GetProperties())
                    {
                        EntityPropertyAttribute pattr = (EntityPropertyAttribute) p.GetCustomAttribute(typeof (EntityPropertyAttribute));
                        if (pattr != null)
                        {
                            EntityPropertyInfo pInfo = new EntityPropertyInfo(p);
                            pInfo.Converter = (IPropertyConverter) Activator.CreateInstance(pattr.Converter);
                            pInfo.Converter.PropertyType = p.PropertyType;
                            info.Properties[pattr.Key] = pInfo;
                        }
                    }
                }
            }

            return new ReadOnlyDictionary<string, EntityInfo>(entities);
        }

        public static IEnumerable<Entity> GetEntities(string fileName)
        {
            using (var reader = new BackupReader(fileName))
            {
                List<Entity> entities = new List<Entity>();

                var entityTypes = GetEntityTypes();
                Entity entity = null;
                EntityInfo entityInfo = null;
                string prevField = string.Empty;
                string entityType = string.Empty;
                foreach (Line line in reader.GetLines().Select(s => new Line(s)))
                {
                    if (line.Key == Entity.ENTITY)
                    {
                        prevField = string.Empty;
                        entityType = line.Value;
                        if (!string.IsNullOrEmpty(line.Value) && entityTypes.TryGetValue(line.Value, out entityInfo))
                            entity = (Entity)Activator.CreateInstance(entityInfo.EntityType);
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
                    _package = reader.Package;
                    _version = reader.Version;
                    _versionCode = reader.VersionCode;
                    _dbVersion = reader.DatabaseVersion;
                }

                return entities;
            }
        }
    }
}
