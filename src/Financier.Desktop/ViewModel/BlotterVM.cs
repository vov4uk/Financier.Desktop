using Financier.DataAccess.View;
using Prism.Commands;
using System;

namespace Financier.Desktop.ViewModel
{
    public class BlotterVM : EntityBaseVM<BlotterTransactions>
    {
        public event EventHandler<TransactionsView> OpenTransactionRaised;

        public BlotterTransactions _selectedValue;
        public BlotterTransactions SelectedValue
        {
            get { return _selectedValue; }
            set { SetProperty(ref _selectedValue, value); }
        }

        private DelegateCommand<TransactionsView> _openTransactionDialogCommand;
        public DelegateCommand<TransactionsView> OpenTransactionDialogCommand
        {
            get
            {
                if (_openTransactionDialogCommand == null)
                    _openTransactionDialogCommand = new DelegateCommand<TransactionsView>(OpenTransacrionWindow);

                return _openTransactionDialogCommand;
            }
        }

        private void OpenTransacrionWindow(TransactionsView obj)
        {
            if (OpenTransactionRaised != null)
                OpenTransactionRaised(this, obj);
        }
    }
}
