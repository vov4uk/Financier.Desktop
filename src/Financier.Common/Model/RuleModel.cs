using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Financier.Common.Attribute;
using Financier.Common.Entities;
using Financier.Common.Localization;
using Financier.Converters;

namespace Financier.Common.Model
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum RuleConditionType
    {
        [LocalizedDescription("rule_condition_type_description_contains")]
        DescriptionContains,
        [LocalizedDescription("rule_condition_type_description_matches")]
        DescriptionMatches,
        [LocalizedDescription("rule_condition_type_mcc")]
        MCC
    }

    [ExcludeFromCodeCoverage]
    public class RuleModel : BaseModel, IActive
    {
        [DisplayName("Id")]
        public int? Id { get; set; }

        [DisplayName("Created")]
        public DateTime Created { get; set; }

        [DisplayName("Condition")]
        public RuleConditionType Condition { get; set; }


        public string Description { get; set; }

        [DisplayName("Description")]
        public string UserFirendlyDescription { get; set; }

        [DisplayName("Title")]
        public string Title { get; set; }

        public bool IsActive { get; set; }
        public int? PayeeId { get; set; }
        public int? ProjectId { get; set; }
        public int? CategoryId { get; set; }
        public int? LocationId { get; set; }
        public Mcc MCCCategory { get; set; }

        public RuleModel() { }

        private string BuildTitle()
        {
            string title = string.Empty;
            List<string> conditions = new List<string>();
            if (PayeeId.HasValue)
            {
                var pe = DbManual.Payee.FirstOrDefault(p => p.Id == PayeeId.Value);
                conditions.Add(string.Format(LocalizationService.Instance.rule_title_payee, pe?.Title));
            }
            if (ProjectId.HasValue)
            {
                var p = DbManual.Project.FirstOrDefault(p => p.Id == ProjectId.Value);
                conditions.Add(string.Format(LocalizationService.Instance.rule_title_project, p?.Title));
            }
            if (CategoryId.HasValue)
            {
                var c = DbManual.Category.FirstOrDefault(c => c.Id == CategoryId.Value);
                conditions.Add(string.Format(LocalizationService.Instance.rule_title_category, c?.Title));
            }
            if (LocationId.HasValue)
            {
                var l = DbManual.Location.FirstOrDefault(l => l.Id == LocationId.Value);
                conditions.Add(string.Format(LocalizationService.Instance.rule_title_location, l?.Title));
            }

            return string.Join(LocalizationService.Instance.rule_title_and, conditions).Trim();
        }

        private string BuildDescription()
        {
            if (Condition == RuleConditionType.MCC)
            {
              return MCCCategory.GetEnumLocalizedMccDescription();
            }
            return Description;
        }

        public void UpdateTitles()
        {
            Title = BuildTitle();
            UserFirendlyDescription = BuildDescription();
        }
    }
}
