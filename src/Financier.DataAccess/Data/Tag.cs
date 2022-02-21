using System.ComponentModel.DataAnnotations.Schema;

namespace Financier.DataAccess.Data
{
    public abstract class Tag : Entity, IIdentity
    {
        [Column(Backup.IdColumn)]
        public int Id { get; set; } = -1;

        [Column(Backup.IsActiveColumn)]
        public bool IsActive { get; set; } = true;

        [Column(Backup.TitleColumn)]
        public string Title { get; set; }

        [Column(Backup.UpdatedOnColumn)]
        public long UpdatedOn { get; set; }
    }
}
