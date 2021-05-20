using FinancistoAdapter.Converters;
using System;
using System.Diagnostics;

namespace FinancistoAdapter.Entities
{
    [DebuggerDisplay("{Title}")]
    [Entity(Backup.LOCATIONS_TABLE)]
    public class Location : Entity, IIdentity
    {
        [EntityProperty(IdColumn)]
        public int Id { get; set; } = -1;

        [EntityProperty(IsActiveColumn )]
        public bool IsActive { get; set; } = true;

        [EntityProperty(TitleColumn)]
        public string Title { get; set; }

        [EntityProperty("name")]
        public string Name { get; set; }

        [EntityProperty("datetime", Converter = typeof(DateTimeConverter))]
        public DateTime? Date { get; set; }

        [EntityProperty("provider")]
        public string Provider { get; set; }

        [EntityProperty("accuracy")]
        public string Accuracy { get; set; }

        [EntityProperty("latitude")]
        public string Latitude { get; set; }

        [EntityProperty("longitude")]
        public string Longitude { get; set; }

        [EntityProperty("is_payee")]
        public bool IsPayee { get; set; }

        [EntityProperty("resolved_address")]
        public string Address { get; set; }

        [EntityProperty("count")]
        public int Count { get; set; }

        [EntityProperty(UpdatedOnColumn)]
        public long UpdatedOn { get; set; }
    }
}