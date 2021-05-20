namespace FinancistoAdapter.Entities
{
    [Entity(Backup.CATEGORY_ATTRIBUTE_TABLE)]
    public class CategoryAttribute : Entity
    {
        [EntityProperty("category_id")]
        public int Category { get; set; }

        [EntityProperty("attribute_id")]
        public int Attribute { get; set; }
    }
}
