using Financier.DataAccess.Data;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Financier.Common.Model
{
    [ExcludeFromCodeCoverage]
    public class RuleModel : BaseModel, IActive
    {
        [DisplayName("Id")]
        public int? Id { get; set; }

        [DisplayName("Created")]
        public DateTime Created { get; set; }
        [DisplayName("Condition")]
        public string Condition { get; set; }

        [DisplayName("Condition")]
        public string Description { get; set; }

        [DisplayName("Title")]
        public string Title { get; set; }

        public bool IsActive { get; set; }
        public int? PayeeId { get; set; }
        public int? ProjectId { get; set; }
        public int? CategoryId { get; set; }
        public int? LocationId { get; set; }
        public string MCCCategory { get; set; }


        public RuleModel() { }
    }
}