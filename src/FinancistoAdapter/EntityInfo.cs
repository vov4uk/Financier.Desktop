using System;
using System.Collections.Generic;

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
}