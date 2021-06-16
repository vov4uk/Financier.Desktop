using Financier.DataAccess.View;
using Prism.Commands;
using Prism.Events;
using System;

namespace Financier.Desktop.ViewModel
{
    public class BlotterVM : EntityBaseVM<BlotterTransactions>
    {
        public event EventHandler<TransactionsView> EditRaised;
        public event EventHandler AddTransferRaised;
        public event EventHandler AddTransactionRaised;
        public event EventHandler AddTemplateRaised;
        public event EventHandler<TransactionsView> DuplicateRaised;
        public event EventHandler<TransactionsView> DeleteRaised;
        public event EventHandler<TransactionsView> InfoRaised;

        public BlotterTransactions _selectedValue;
        public BlotterTransactions SelectedValue
        {
            get { return _selectedValue; }
            set { 
                SetProperty(ref _selectedValue, value);
                EditCommand.RaiseCanExecuteChanged();
                DuplicateCommand.RaiseCanExecuteChanged();
                DeleteCommand.RaiseCanExecuteChanged();
                InfoCommand.RaiseCanExecuteChanged();
            }
        }

        private DelegateCommand _editCommand;
        public DelegateCommand EditCommand
        {
            get
            {
                return _editCommand ??= new DelegateCommand(()=> EditRaised?.Invoke(this, SelectedValue), () => SelectedValue != null);
            }
        }

        private DelegateCommand _addTransferCommand;
        public DelegateCommand AddTransferCommand
        {
            get
            {
                return _addTransferCommand ??= new DelegateCommand(() => AddTransferRaised?.Invoke(this, EventArgs.Empty));
            }
        }

        private DelegateCommand _addTransactionCommand;
        public DelegateCommand AddTransactionCommand
        {
            get
            {
                return _addTransactionCommand ??= new DelegateCommand(() => AddTransactionRaised?.Invoke(this, EventArgs.Empty));
            }
        }

        private DelegateCommand _addTemplateCommand;
        public DelegateCommand AddTemplateCommand
        {
            get
            {
                return _addTemplateCommand ??= new DelegateCommand(() => AddTemplateRaised?.Invoke(this, EventArgs.Empty), () => false);
            }
        }

        private DelegateCommand _duplicateCommand;
        public DelegateCommand DuplicateCommand
        {
            get
            {
                return _duplicateCommand ??= new DelegateCommand(() => DuplicateRaised?.Invoke(this, SelectedValue), () => SelectedValue != null);
            }
        }

        private DelegateCommand _infoCommand;
        public DelegateCommand InfoCommand
        {
            get
            {
                return _infoCommand ??= new DelegateCommand(() => InfoRaised?.Invoke(this, SelectedValue), () => false);
            }
        }

        private DelegateCommand _deleteCommand;
        public DelegateCommand DeleteCommand
        {
            get
            {
                return _deleteCommand ??= new DelegateCommand(() => DeleteRaised?.Invoke(this, SelectedValue), () => SelectedValue != null);
            }
        }
    }
}
