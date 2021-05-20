using System.Diagnostics;

namespace FinancistoAdapter.Entities
{
    [DebuggerDisplay("{Title}")]
    [Entity(Backup.PAYEE_TABLE)]
    public class Payee : Entity, IIdentity
    {
        [EntityProperty(IdColumn)]
        public int Id { get; set; } = -1;

        [EntityProperty(IsActiveColumn )]
        public bool IsActive { get; set; } = true;

        [EntityProperty(TitleColumn)]
        public string Title { get; set; }

        [EntityProperty("last_category_id")]
        public long LastCategoryId { get; set; }

        [EntityProperty(UpdatedOnColumn)]
        public long UpdatedOn { get; set; }

    }
}
