using System;
using Financier.Common.Entities;
using Financier.Common.Model;
using Financier.Desktop.Data;
using Financier.Desktop.ViewModel.Dialog;

namespace Financier.Desktop.Pages.Dialogs
{

    public class RuleControlVM : DialogBaseVM
    {
        private RuleConditionType _selectedConditionType;
        private Mcc _selectedMCC;
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
        public Mcc SelectedMCC
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
            get => SelectedConditionType == RuleConditionType.MCC;
        }

        public RuleControlVM(RuleDTO entity)
        {
            this.Entity = entity;
            SelectedConditionType = entity.Condition;
            if (IsMCCSelected && Enum.TryParse<Mcc>(Entity.Description, out var mcc))
            {
                SelectedMCC = mcc;
            }
        }

        public RuleDTO Entity { get; }

        public override object OnRequestSave()
        {
            Entity.Condition = SelectedConditionType;
            if (IsMCCSelected)
            {
                Entity.Description = SelectedMCC.ToString();
            }
            return Entity;
        }
    }
}
