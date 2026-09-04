using System;
using Financier.Common.Entities;
using Financier.Common.Model;
using Prism.Mvvm;

namespace Financier.Desktop.Data
{
    public class RuleDto : BindableBase
    {
        private CategoryModel category;
        private int? categoryId;
        private RuleConditionType condition;
        private string description;
        private bool isActive;
        private int? locationId;
        private int? payeeId;
        private int? projectId;
        private Mcc mccCategory;
        public RuleDto()
        {
        }

        public RuleDto(RuleModel rulesModel)
        {
            Description = rulesModel.Description;
            Condition = rulesModel.Condition;
            IsActive = rulesModel.IsActive;
            PayeeId = rulesModel.PayeeId;
            ProjectId = rulesModel.ProjectId;
            CategoryId = rulesModel.CategoryId;
            LocationId = rulesModel.LocationId;
            Created = rulesModel.Created;
            MCCCategory = rulesModel.MCCCategory;
        }

        public CategoryModel Category
        {
            get => category ??= DbManual.Category?.Find(x => x.Id == CategoryId);
            set
            {
                if (SetProperty(ref category, value))
                {
                    RaisePropertyChanged(nameof(Category));
                }
            }
        }

        public int? CategoryId
        {
            get => categoryId;
            set
            {
                if (SetProperty(ref categoryId, value))
                {
                    RaisePropertyChanged(nameof(CategoryId));
                }
            }
        }

        public RuleConditionType Condition
        {
            get => condition;
            set
            {
                condition = value;
                RaisePropertyChanged(nameof(Condition));
            }
        }

        public DateTime Created { get; set; }

        public string Description
        {
            get => description;
            set
            {
                description = value;
                RaisePropertyChanged(nameof(Description));
            }
        }
        public bool IsActive
        {
            get => isActive;
            set
            {
                isActive = value;
                RaisePropertyChanged(nameof(IsActive));
            }
        }
        public int? LocationId
        {
            get => locationId;
            set
            {
                if (SetProperty(ref locationId, value))
                {
                    RaisePropertyChanged(nameof(LocationId));
                }
            }
        }

        public int? PayeeId
        {
            get => payeeId;
            set
            {
                if (SetProperty(ref payeeId, value))
                {
                    RaisePropertyChanged(nameof(PayeeId));
                }
            }
        }

        public int? ProjectId
        {
            get => projectId;
            set
            {
                if (SetProperty(ref projectId, value))
                {
                    RaisePropertyChanged(nameof(ProjectId));
                }
            }
        }

        public Mcc MCCCategory
        {
            get => mccCategory;
            set
            {
                if (SetProperty(ref mccCategory, value))
                {
                    RaisePropertyChanged(nameof(MCCCategory));
                }
            }
        }
    }
}
