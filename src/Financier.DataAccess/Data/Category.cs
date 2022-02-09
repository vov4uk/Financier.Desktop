using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace Financier.DataAccess.Data
{
    [DebuggerDisplay("{Id}-{Title} -- Left:{Left} - Right:{Right}")]
    [Table(Backup.CATEGORY_TABLE)]
    public class Category : Entity, IIdentity
    {
        [Column(Backup.IdColumn)]
        public int Id { get; set; } = -1;

        [Column(Backup.TitleColumn)]
        public string Title { get; set; }

        [Column(Backup.IsActiveColumn)]
        public bool IsActive { get; set; } = true;

        [Column("left")]
        public int Left { get; set; } = 1;

        [Column("right")]
        public int Right { get; set; } = 2;

        [Column("type")]
        public int Type { get; set; }

        [Column("last_location_id")]
        public int? LastLocationId { get; set; }

        [Column("last_project_id")]
        public int? LastProjectId { get; set; }

        [Column(Backup.UpdatedOnColumn)]
        public long UpdatedOn { get; set; }

        [Column(Backup.SortOrderColumn)]
        public int SortOrder { get; set; }

        [NotMapped]
        public static Category None => new Category()
        {
            Id = 0,
            Left = 1,
            Right = 2,
            Title = "<NO_CATEGORY>"
        };

        [NotMapped]
        public static Category Split => new Category()
        {
            Id = -1,
            Left = 0,
            Right = 0,
            Title = "<SPLIT_CATEGORY>"
        };
    }
}