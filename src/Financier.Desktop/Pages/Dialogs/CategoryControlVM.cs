using System.Collections.Generic;
using Financier.Common.Model;
using Financier.Desktop.Data;
using Prism.Commands;

namespace Financier.Desktop.ViewModel.Dialog
{
    public class CategoryControlVM : DialogBaseVM
    {
        private DelegateCommand _clearTitleCommand;
        private CategoryModel _selectedParent;

        public CategoryControlVM(CategoryDto entity, List<CategoryModel> availableParents)
        {
            Entity = entity;
            AvailableParents = availableParents;
            _selectedParent = entity.ParentId > 0
                ? availableParents.Find(x => x.Id == entity.ParentId)
                : availableParents.Count > 0 ? availableParents[0] : null;
        }

        public CategoryDto Entity { get; }

        public List<CategoryModel> AvailableParents { get; }

        public CategoryModel SelectedParent
        {
            get => _selectedParent;
            set
            {
                SetProperty(ref _selectedParent, value);
                Entity.ParentId = value?.Id ?? 0;
                if (value?.Id > 0)
                {
                    Entity.IsIncome = (value.Type & 1) != 0;
                }
                RaisePropertyChanged(nameof(IsTypeEnabled));
            }
        }

        public bool IsTypeEnabled => Entity.ParentId <= 0;

        public DelegateCommand ClearTitleCommand => _clearTitleCommand ??= new DelegateCommand(() => Entity.Title = default!);

        public override object OnRequestSave() => Entity;
    }
}
