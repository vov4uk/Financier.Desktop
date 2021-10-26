using Financier.DataAccess.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Financier.Desktop.MonoWizard.ViewModel
{
    public class Page1VM : WizardBaseVM
    {

        private RangeObservableCollection<Account> _accounts;

        private Account _monoAccount;

        public Page1VM(List<Account> records)
        {
            _accounts = new RangeObservableCollection<Account>(records);
            _monoAccount = _accounts?.FirstOrDefault(x => x.IsActive && x.Title.Contains("mono", System.StringComparison.OrdinalIgnoreCase));
        }
        public RangeObservableCollection<Account> Accounts
        {
            get => _accounts;
            set
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
            return true;
        }
    }
}
