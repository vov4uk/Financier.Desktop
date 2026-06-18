using System.Collections.Generic;
using System.Linq;
using Financier.Common.Entities;
using Financier.Common.Model;
using Financier.Converters;
using Financier.Desktop.Data;
using Financier.Desktop.ViewModel.Dialog;

namespace Financier.Desktop.Pages.Dialogs
{

    public class RuleControlVM : DialogBaseVM
    {
        public List<string> MccTitles { get; private set; }

        private readonly string _noneMccTitle;
        private RuleConditionType _selectedConditionType;
        private string _selectedMccTitle;
        public RuleConditionType SelectedConditionType
        {
            get => _selectedConditionType;
            set
            {
                if (_selectedConditionType != value)
                {
                    _selectedConditionType = value;
                    RaisePropertyChanged(nameof(SelectedConditionType));
                    RaisePropertyChanged(nameof(IsMCCSelected));
                    SaveCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public string SelectedMccTitle
        {
            get => _selectedMccTitle;
            
            set
            {
                if (_selectedMccTitle != value)
                {
                    _selectedMccTitle = value;
                    RaisePropertyChanged(nameof(SelectedMccTitle));
                    SaveCommand.RaiseCanExecuteChanged();
                }
            }
        }


        public bool IsMCCSelected
        {
            get => SelectedConditionType == RuleConditionType.MCC;
        }

        public RuleControlVM(RuleDto entity)
        {
            this.Entity = entity;
            SelectedConditionType = entity.Condition;
            this.Entity.PropertyChanged += Transaction_PropertyChanged;

            MccTitles = DbManual.MCCTitles.Keys.OrderBy(x => x).ToList();
            SelectedMccTitle = entity.MCCCategory.GetEnumLocalizedMccDescription();
            _noneMccTitle = Mcc.none.GetEnumLocalizedMccDescription();
        }

        public RuleDto Entity { get; }

        public override object OnRequestSave()
        {
            Entity.Condition = SelectedConditionType;
            if (!IsMCCSelected)
            {
                Entity.MCCCategory = Mcc.none;
            }
            else
            {
                Entity.Description = null;
                Entity.MCCCategory = DbManual.MCCTitles[SelectedMccTitle];
            }
            return Entity;
        }

        protected override bool CanSaveCommandExecute()
        {
            bool conditionMeets = (IsMCCSelected && !string.IsNullOrEmpty(SelectedMccTitle) && SelectedMccTitle != _noneMccTitle && DbManual.MCCTitles.ContainsKey(SelectedMccTitle)) || (!IsMCCSelected && !string.IsNullOrEmpty(Entity.Description));
            bool accountMeets = Entity.PayeeId != null || Entity.LocationId != null || Entity.CategoryId != null || Entity.ProjectId != null;
            return conditionMeets && accountMeets;
        }

        private void Transaction_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            SaveCommand.RaiseCanExecuteChanged();
        }
    }
}
