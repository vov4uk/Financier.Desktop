using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using Financier.DataAccess.Data;

namespace Financier.DataAccess.View
{
    [DebuggerDisplay("{name} - {from_amount}")]
    public class ByCategoryReportBase : Entity
    {
        public int parent_id { get; set; }

        public string name { get; set; }

        public long datetime { get; set; }

        [ForeignKey("from_account_currency")]
        public int from_account_currency_id { get; set; }

        public Currency from_account_currency { get; set; }

        public long from_amount { get; set; }

        [ForeignKey("to_account_currency")]
        public int? to_account_currency_id { get; set; }

        public Currency to_account_currency { get; set; }
        public long to_amount { get; set; }

        [ForeignKey("original_currency")]
        public int? original_currency_id { get; set; }

        public Currency original_currency { get; set; }

        public long original_from_amount { get; set; }

        public bool is_transfer { get; set; }

        [ForeignKey("from_account")]
        public int from_account_id { get; set; }

        public Account from_account { get; set; }

        [ForeignKey("to_account")]
        public int? to_account_id { get; set; }
        public Account to_account { get; set; }

        [ForeignKey("category")]
        public int? category_id { get; set; }
        public Category category { get; set; }

        public int category_left { get; set; }

        public int category_right { get; set; }

        [ForeignKey("project")]
        public int? project_id { get; set; }
        public Project project { get; set; }

        [ForeignKey("location")]
        public int? location_id { get; set; }
        public Location location { get; set; }

        [ForeignKey("payee")]
        public int? payee_id { get; set; }
        public Payee payee { get; set; }

        public string status { get; set; }

        [NotMapped]
        public long UpdatedOn { get; set; }
    }
}