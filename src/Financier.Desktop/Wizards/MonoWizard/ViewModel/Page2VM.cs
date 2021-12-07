using Financier.DataAccess.Data;
using Financier.DataAccess.Monobank;
using Financier.Desktop.MonoWizard.ViewModel;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Financier.Desktop.Wizards.MonoWizard.ViewModel
{
    public class Page2VM : WizardPageBaseVM
    {
        private Account _monoAccount;

        private MonoTransaction _startTransaction;

        private ObservableCollection<MonoTransaction> allTransactions;
        private DelegateCommand<MonoTransaction> _deleteCommand;

        public Page2VM(List<MonoTransaction> records)
        {
            AllTransactions = new ObservableCollection<MonoTransaction>(records);
        }

        public DelegateCommand<MonoTransaction> DeleteCommand
        {
            get
            {
                return _deleteCommand ??= new DelegateCommand<MonoTransaction>(tr => { allTransactions.Remove(tr); });
            }
        }

        public Account MonoAccount
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

        public MonoTransaction StartTransaction
        {
            get => _startTransaction;
            set
            {
                _startTransaction = value;
                RaisePropertyChanged(nameof(StartTransaction));
            }
        }

        public override string Title => "Please select transaction";

        public ObservableCollection<MonoTransaction> AllTransactions
        {
            get => allTransactions;
            private set
            {
                allTransactions = value;
                RaisePropertyChanged(nameof(AllTransactions));
            }
        }

        public List<MonoTransaction> MonoTransactions
        {
            get
            {
                var startDate = _startTransaction?.Date ?? new DateTime(2017, 11, 17); // Monobank launched
                return allTransactions.OrderByDescending(x => x.Date).Where(x => x.Date > startDate).ToList();
            }
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
