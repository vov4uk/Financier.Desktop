using Financier.Common.Entities;
using Financier.Common.Model;
using Financier.Desktop.Data;
using Financier.Desktop.ViewModel.Dialog;

namespace Financier.Desktop.Pages.Dialogs
{

    public class RuleControlVM : DialogBaseVM
    {
        private RuleConditionType _selectedConditionType;
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
                Entity.Description = null!;
            }
            return Entity;
        }
    }
}
