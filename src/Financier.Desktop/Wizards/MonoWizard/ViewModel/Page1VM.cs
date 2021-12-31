using Financier.DataAccess.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Financier.Desktop.Wizards.MonoWizard.ViewModel
{
    public class Page1VM : WizardPageBaseVM
    {

        private ObservableCollection<Account> _accounts;

        private Account _monoAccount;

        public Page1VM(List<Account> records)
        {
            Accounts = new ObservableCollection<Account>(records);
            MonoAccount = Accounts.FirstOrDefault(x => x.IsActive && x.Title.Contains("mono", System.StringComparison.OrdinalIgnoreCase)) ?? Accounts.FirstOrDefault();
        }
        public ObservableCollection<Account> Accounts
        {
            get => _accounts;
            private set
            {
                _accounts = value;
                RaisePropertyChanged(nameof(Accounts));
            }
        }
        public Account MonoAccount
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
