using Financier.Common.Model;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Financier.Desktop.Wizards.MonoWizard.ViewModel
{
    public class Page2VM : WizardPageBaseVM
    {
        private DelegateCommand<BankTransaction> _deleteCommand;
        private AccountFilterModel _monoAccount;

        private BankTransaction _startTransaction;

        private ObservableCollection<BankTransaction> allTransactions;
        public Page2VM(List<BankTransaction> records)
        {
            AllTransactions = new ObservableCollection<BankTransaction>(records);
        }

        public ObservableCollection<BankTransaction> AllTransactions
        {
            get => allTransactions;
            private set
            {
                allTransactions = value;
                RaisePropertyChanged(nameof(AllTransactions));
            }
        }

        public DelegateCommand<BankTransaction> DeleteCommand
        {
            get
            {
                return _deleteCommand ??= new DelegateCommand<BankTransaction>(tr => { allTransactions.Remove(tr); });
            }
        }

        public AccountFilterModel MonoAccount
        {
            get => _monoAccount;
            set
            {
                _monoAccount = value;
                RaisePropertyChanged(nameof(MonoAccount));
                double balance = _monoAccount.TotalAmount / 100.0;
                StartTransaction = allTransactions.FirstOrDefault(x => Math.Abs(x.Balance - balance) < 0.01);
            }
        }

        public List<BankTransaction> GetMonoTransactions()
        {
            var startDate = _startTransaction?.Date ?? new DateTime(2017, 11, 17); // Monobank launched
            return allTransactions.OrderByDescending(x => x.Date).Where(x => x.Date > startDate).ToList();
        }

        public BankTransaction StartTransaction
        {
            get => _startTransaction;
            set
            {
                _startTransaction = value;
                RaisePropertyChanged(nameof(StartTransaction));
            }
        }

        public override string Title => "Please select transaction";
        public override bool IsValid()
        {
            return true;
        }
    }
}
