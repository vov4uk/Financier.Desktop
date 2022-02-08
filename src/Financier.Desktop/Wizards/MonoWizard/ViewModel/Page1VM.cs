using Financier.Common.Entities;
using Financier.Common.Model;
using System.Linq;

namespace Financier.Desktop.Wizards.MonoWizard.ViewModel
{
    public class Page1VM : WizardPageBaseVM
    {

        private AccountFilterModel _monoAccount;

        public Page1VM()
        {
            MonoAccount = DbManual.Account.FirstOrDefault(x => x.IsActive && x.Title.Contains("mono", System.StringComparison.OrdinalIgnoreCase)) ?? DbManual.Account.FirstOrDefault(x => x.Id != null);
        }

        public AccountFilterModel MonoAccount
        {
            get => _monoAccount;
            set
            {
                _monoAccount = value;
                RaisePropertyChanged(nameof(MonoAccount));
            }
        }

        public override string Title => "Please select account";

        public override bool IsValid()
        {
            return MonoAccount != null;
        }
    }
}
