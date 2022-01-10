using System.ComponentModel.DataAnnotations.Schema;

namespace Financier.DataAccess.Data
{
    [Table(Backup.SMS_TEMPLATES_TABLE)]
    public class SmsTemplate : Entity, IActive
    {
        [Column(IdColumn)]
        public int Id { get; set; } = -1;

        [Column(IsActiveColumn)]
        public bool IsActive { get; set; } = true;

        [Column(TitleColumn)]
        public string Title { get; set; }

        [ForeignKey("Category")]
        [Column("category_id")]
        public int CategoryId { get; set; }

        [Column("template")]
        public string Template { get; set; }

        [ForeignKey("Account")]
        [Column("account_id")]
        public int AccountId { get; set; }

        [Column("is_income")]
        public bool IsIncome { get; set; }

        [Column(UpdatedOnColumn)]
        public long UpdatedOn { get; set; }

        public Account Account { get; set; }

        public Category Category { get; set; }
    }
}