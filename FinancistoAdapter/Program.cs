using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using FinancistoAdapter.Entities;

namespace FinancistoAdapter
{
	public class EntityInfo
	{
		public EntityInfo()
		{
			Properties = new Dictionary<string, EntityPropertyInfo>();
		}

		public Type EntityType { get; set; }
		public IDictionary<string, EntityPropertyInfo> Properties { get; private set; }
	}

	public class EntityPropertyInfo
	{
		private delegate void SetValueDelegate(object entity, object value);

		private SetValueDelegate _delegate;

		public EntityPropertyInfo(PropertyInfo info)
		{
			PropertyName = info.Name;
			PropertyType = info.PropertyType;
			_delegate = info.SetValue;
		}

		public string PropertyName { get; private set; }

		public Type PropertyType { get; private set; }

		public Type ForeignKey { get; set; }

		public void SetValue(Entity entity, string value)
		{
			object v = Converter.Convert(value);
			_delegate(entity, v);
		}

		public void SetValue(Entity entity, object pureValue)
		{
			_delegate(entity, pureValue);
		}

		public IPropertyConverter Converter { get; set; }
	}

	class Program
	{
		public static IReadOnlyDictionary<string, EntityInfo> GetEntityTypes()
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
							pInfo.ForeignKey = pattr.ForeignKey;
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
				//var keysMap = new Dictionary<Tuple<Type, string, int>, Tuple<EntityPropertyInfo, int>>();
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
							if (property.ForeignKey != null)
								//keysMap[Tuple.Create(entityInfo.EntityType, property.PropertyName, entity.Id)] = Tuple.Create(property, int.Parse(line.Value));
							{
								Entity cEntity = entity;
								foreignKeys.Add(Tuple.Create(property, new Action<object>(v => property.SetValue(cEntity, v)), int.Parse(line.Value)));
							}
							else
								property.SetValue(entity, line.Value);
						}
					}
				}

				//foreach (var link in keysMap)
				foreach (Tuple<EntityPropertyInfo, Action<object>, int> link in foreignKeys)
				{
					Dictionary<int, Entity> mapById;
					Entity linkedEntity;
					if (map.TryGetValue(link.Item1.ForeignKey, out mapById) && mapById.TryGetValue(link.Item3, out linkedEntity))
						link.Item2(linkedEntity);
				}

				return entities;
			}
		}

		static void Main(string[] args)
		{
			var entites = GetEntities("test.backup").ToArray();
		}
	}
}
