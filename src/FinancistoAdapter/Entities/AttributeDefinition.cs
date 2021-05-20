using System.Diagnostics;

namespace FinancistoAdapter.Entities
{
    [DebuggerDisplay("{Title}")]
    [Entity(Backup.ATTRIBUTES_TABLE)]
    public class AttributeDefinition : Entity
    {

        public const int TYPE_TEXT = 1;
        public const int TYPE_NUMBER = 2;
        public const int TYPE_LIST = 3;
        public const int TYPE_CHECKBOX = 4;

        [EntityProperty(IdColumn)]
        public int Id { get; set; } = -1;

        [EntityProperty("type")]
        public int Type { get; set; }

        [EntityProperty(IsActiveColumn )]
        public bool IsActive { get; set; } = true;

        [EntityProperty(TitleColumn)]
        public string Title { get; set; }

        [EntityProperty("list_values")]
        public string ListValues { get; set; }

        [EntityProperty("default_value")]
        public string DefaultValue { get; set; }
    }
}
