using Financier.Common.Entities;
using Financier.Common.Model;
using Financier.DataAccess.Data;
using Prism.Mvvm;
using System;

namespace Financier.Desktop.Data
{
    public class RuleDTO : BindableBase
    {
        private bool isActive;
        private string description;
        private string condition;
        private int? payeeId;
        private int? projectId;
        private CategoryModel category;
        private int? categoryId;
        private int? locationId;

        public RuleDTO()
        {
        }

        public RuleDTO(RuleModel rulesModel)
        {
            Description = rulesModel.Description;
            Condition = rulesModel.Condition;
            IsActive = rulesModel.IsActive;
            PayeeId = rulesModel.PayeeId;
            ProjectId = rulesModel.ProjectId;
            CategoryId = rulesModel.CategoryId;
            LocationId = rulesModel.LocationId;
            Created = rulesModel.Created;
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

        public string Condition
        {
            get => condition;
            set
            {
                condition = value;
                RaisePropertyChanged(nameof(Condition));
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
    }
}