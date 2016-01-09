using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FinancistoAdapter.Entities;

namespace FinancistoAdapter
{
	public static class EntityReader
	{
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
				var map = new Dictionary<Type, Dictionary<int, Entity>>();
				var foreignKeys = new List<Tuple<EntityPropertyInfo, Action<object>, int>>();

				var entityTypes = GetEntityTypes();
				Entity entity = null;
				EntityInfo entityInfo = null;

				foreach (Line line in reader.GetLines().Select(s => new Line(s)))
				{
					if (line.Key == "$ENTITY")
					{
						if (!String.IsNullOrEmpty(line.Value) && entityTypes.TryGetValue(line.Value, out entityInfo))
							entity = (Entity)Activator.CreateInstance(entityInfo.EntityType);
					}
					else if (line.Key == "$$" && entity != null)
					{
						if (!map.ContainsKey(entityInfo.EntityType))
							map[entityInfo.EntityType] = new Dictionary<int, Entity>();
						map[entityInfo.EntityType][entity.Id] = entity;

						entities.Add(entity);

						entity = null;
					}
					else if (entity != null && line.Value != null)
					{
						EntityPropertyInfo property;
						if (entityInfo.Properties.TryGetValue(line.Key, out property))
						{
							if (typeof(Entity).IsAssignableFrom(property.PropertyType))
							{
								Entity cEntity = entity;
								foreignKeys.Add(Tuple.Create(property, new Action<object>(v => property.SetValue(cEntity, v)), int.Parse(line.Value)));
							}
							else
								property.SetValue(entity, line.Value);
						}
					}
				}

				foreach (Tuple<EntityPropertyInfo, Action<object>, int> link in foreignKeys)
				{
					Dictionary<int, Entity> mapById;
					Entity linkedEntity;
					if (map.TryGetValue(link.Item1.PropertyType, out mapById) && mapById.TryGetValue(link.Item3, out linkedEntity))
						link.Item2(linkedEntity);
				}

				return entities;
			}
		}
	}
}
