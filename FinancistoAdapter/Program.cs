using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FinancistoAdapter.Entities;

namespace FinancistoAdapter
{
	public class EntityInfo
	{
		public EntityInfo()
		{
			Properties = new Dictionary<string, PropertyInfo>();
		}

		public Type EntityType { get; set; }
		public IDictionary<string, PropertyInfo> Properties { get; private set; }
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
							info.Properties[pattr.Key] = p;
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
						yield return entity;
						entity = null;
					}
					else if (entity != null && line.Value != null)
					{
						PropertyInfo property;
						if (entityInfo.Properties.TryGetValue(line.Key, out property))
						{
							object value = Convert.ChangeType(line.Value,
								Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
							property.SetValue(entity, value);
						}
					}
				}
			}
		}

		static void Main(string[] args)
		{
			var entites = GetEntities("test.backup").ToArray();
		}
	}
}
