using Financier.DataAccess.Data;
using Financier.DataAccess.Monobank;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Financier.Desktop.MonoWizard.ViewModel
{
    public class Page2ViewModel : WizardBaseViewModel
    {
        #region Constructor
        public Page2ViewModel(List<MonoTransaction> records)
        {
            _transactions = new RangeObservableCollection<MonoTransaction>(records);
        }
        #endregion

        private RangeObservableCollection<MonoTransaction> _transactions;
        public RangeObservableCollection<MonoTransaction> Transactions
        {
            get { return _transactions; }
            set
            {
                _transactions = value;
                RaisePropertyChanged(nameof(Transactions));
            }
        }

        private MonoTransaction _startTransaction;
        public MonoTransaction StartTransaction
        {
            get { return _startTransaction; }
            set
            {
                _startTransaction = value;
                RaisePropertyChanged(nameof(StartTransaction));
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
                double balance = _monoAccount.TotalAmount / 100.0;
                StartTransaction = _transactions?.FirstOrDefault(x => x.Balance == balance);
            }
        }

        public List<MonoTransaction> TransactionsToImport {
            get
            {
                var startDate = new DateTime(2017, 11, 17); // Monobank launched
                if (_startTransaction != null)
                {
                    startDate = _startTransaction.Date;
                }
                return _transactions.OrderByDescending(x => x.Date).Where(x => x.Date > startDate).ToList();
            }
        }

        public override string Title
        {
            get
            {
                return "Page 2";
            }
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
