using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Financier.DataAccess.Data
{
    [Keyless]
    [Table(Backup.CCARD_CLOSING_DATE_TABLE)]
    public class CCardClosingDate : Entity
    {
        [ForeignKey("Account")]
        [Column("account_id")]
        public int AccountId { get; set; }

        [Column("period")]
        public int Period { get; set; }

        [Column("closing_day")]
        public int ClosingDay { get; set; }

        public Account Account { get; set; }
    }
}