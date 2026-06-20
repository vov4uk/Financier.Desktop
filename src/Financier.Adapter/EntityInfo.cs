using Financier.DataAccess.Data;
using System;
using System.Collections.Generic;

namespace Financier.Adapter
{
    public class EntityInfo
    {
        public EntityInfo()
        {
            Properties = new Dictionary<string, EntityPropertyInfo>();
        }

        public Type EntityType { get; set; }
        public Func<Entity> Factory { get; set; }
        public IDictionary<string, EntityPropertyInfo> Properties { get; private set; }
    }
}
