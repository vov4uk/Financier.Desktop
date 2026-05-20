using Financier.Desktop.Data;
using Financier.Desktop.ViewModel.Dialog;
using Prism.Commands;
using System.Collections.ObjectModel;
using System.Data;

namespace Financier.Desktop.Pages.Dialogs
{
    public class RuleControlVM : DialogBaseVM
    {
        private string _selectedConditionType;
        private string _selectedMCC;
        public string SelectedConditionType
        {
            get => _selectedConditionType;
            set
            {
                if (_selectedConditionType != value)
                {
                    _selectedConditionType = value;
                    RaisePropertyChanged(nameof(SelectedConditionType));
                    RaisePropertyChanged(nameof(IsMCCSelected));
                }
            }
        }
        public string SelectedMCC
        {
            get => _selectedMCC;
            set
            {
                if (_selectedMCC != value)
                {
                    _selectedMCC = value;
                    RaisePropertyChanged(nameof(SelectedMCC));
                }
            }
        }

        public bool IsMCCSelected
        {
            get => SelectedConditionType == "MCC";
        }

        public static ObservableCollection<string> ConditionTypes { get; } = new ObservableCollection<string>
        {
           "Description contains",
           "Description matches",
           "MCC"
        };

        public RuleControlVM(RuleDTO entity)
        {
            this.Entity = entity;
            SelectedConditionType = entity.Condition;
            if (IsMCCSelected)
            {
                SelectedMCC = Entity.Description;
            }
        }

        public RuleDTO Entity { get; }

        public override object OnRequestSave()
        {
            Entity.Condition = SelectedConditionType;
            if (IsMCCSelected)
            {
                Entity.Description = SelectedMCC;
            }
            return Entity;
        }
    }
}
