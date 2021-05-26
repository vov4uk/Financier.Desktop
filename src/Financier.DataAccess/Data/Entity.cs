namespace Financier.DataAccess.Data
{
    public abstract class Entity
    {
        public const string IdColumn = "_id";
        public const string TitleColumn = "title";
        public const string SortOrderColumn = "sort_order";
        public const string IsActiveColumn = "is_active";
        public const string UpdatedOnColumn = "updated_on";
        public const string END = "$$";
        public const string ENTITY = "$ENTITY";
    }
}
