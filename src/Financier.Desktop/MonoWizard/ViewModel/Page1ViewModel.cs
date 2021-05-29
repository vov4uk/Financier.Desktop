using Financier.DataAccess.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Financier.Desktop.MonoWizard.ViewModel
{
    public class Page1ViewModel : WizardBaseViewModel
    {

        public Page1ViewModel(List<Account> records)
        {
            _accounts = new RangeObservableCollection<Account>(records);
            _monoAccount = _accounts?.FirstOrDefault(x => x.IsActive && x.Title.Contains("mono", System.StringComparison.OrdinalIgnoreCase));
        }

        private RangeObservableCollection<Account> _accounts;
        public RangeObservableCollection<Account> Accounts
        {
            get { return _accounts; }
            set
            {
                _accounts = value;
                RaisePropertyChanged(nameof(Accounts));
            }
        }

        private Account _monoAccount;
        public Account MonoAccount
        {
            get { return _monoAccount; }
            set
            {
                _monoAccount = value;
                RaisePropertyChanged(nameof(MonoAccount));
            }
        }

        public override string Title
        {
            get
            {
                return "First Page";
            }
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
