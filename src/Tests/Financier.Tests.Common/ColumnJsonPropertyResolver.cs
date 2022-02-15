using AutoMapper.Internal;
using Financier.DataAccess.Data;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Financier.Tests.Common
{
    public class ColumnJsonPropertyResolver<T> : DefaultContractResolver
        where T : Entity, new()
    {
        private Dictionary<string, string> PropertyMappings { get; set; }

        public ColumnJsonPropertyResolver()
        {
            this.PropertyMappings = new Dictionary<string, string>();
            foreach (PropertyInfo property in typeof(T).GetProperties())
            {
                ColumnAttribute column = Attribute.GetCustomAttribute(property, typeof(ColumnAttribute)) as ColumnAttribute;
                if (column != null)
                {
                    PropertyMappings.Add(property.Name, column.Name);
                }
            }
        }

        protected override string ResolvePropertyName(string propertyName)
        {
            string resolvedName = null;
            var resolved = this.PropertyMappings.TryGetValue(propertyName, out resolvedName);
            return (resolved) ? resolvedName : base.ResolvePropertyName(propertyName);
        }
    }
}
