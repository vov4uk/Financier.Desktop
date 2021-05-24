using System.Diagnostics;

namespace FinancistoAdapter.Entities
{
    [DebuggerDisplay("{Id}-{Title} -- Left:{Left} - Right:{Right}")]
    [Entity(Backup.CATEGORY_TABLE)]
    public class Category : Entity, IIdentity
    {
        [EntityProperty(IdColumn)]
        public int Id { get; set; } = -1;

        [EntityProperty(TitleColumn)]
        public string Title { get; set; }

        [EntityProperty(IsActiveColumn )]
        public bool IsActive { get; set; } = true;

        [EntityProperty("left")]
        public int Left { get; set; } = 1;

        [EntityProperty("right")]
        public int Right { get; set; } = 2;

        [EntityProperty("type")]
        public string Type { get; set; }

        [EntityProperty("last_location_id")]
        public int LastLocationId { get; set; }

        [EntityProperty("last_project_id")]
        public int LastProjectId { get; set; }
        
        [EntityProperty(UpdatedOnColumn)]
        public long UpdatedOn { get; set; }


        private class SplitCategory : Category
        {
            public new int Id
            {
                get { return -1; }
                set { }
            }

            public new string Title
            {
                get { return "Split"; }
                set { }
            }
        }

        private static readonly Category _split = new SplitCategory();
        public static Category Split
        {
            get { return _split; }
        }
    }
}
