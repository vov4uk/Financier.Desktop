using Financier.DataAccess.Data;
using Financier.DataAccess.Monobank;
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
                if (_startTransaction != null)
                {
                    TransactionsToImport = _transactions.OrderByDescending(x => x.Date).Where(x => x.Date > _startTransaction.Date).ToList();
                }
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

        public List<MonoTransaction> TransactionsToImport { get; set; }

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
