using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Financier.Common.Entities;

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

        private string BuildTitle()
        {
            string title = string.Empty;
            if (PayeeId.HasValue)
            {
                var pe = DbManual.Payee.FirstOrDefault(p => p.Id == PayeeId.Value);
                title += $"Payee: {pe?.Title} ";
            }
            if (ProjectId.HasValue)
            {
                var p = DbManual.Project.FirstOrDefault(p => p.Id == ProjectId.Value);
                title += $"Project: {p?.Title} ";
            }
            if (CategoryId.HasValue)
            {
                var c = DbManual.Category.FirstOrDefault(c => c.Id == CategoryId.Value);
                title += $"Category: {c?.Title} ";
            }
            if (LocationId.HasValue)
            {
                var l = DbManual.Location.FirstOrDefault(l => l.Id == LocationId.Value);
                title += $"Location: {l?.Title} ";
            }
            if (!string.IsNullOrEmpty(MCCCategory))
            {
                title += $"MCC: {MCCCategory} ";
            }
            return title.Trim();
        }
        public void UpdateTitle()
        {
            Title = BuildTitle();
        }
    }
}