namespace Financier.Desktop.ViewModel
{
    using Financier.DataAccess.View;
    using Prism.Commands;
    using System;

    public class BlotterVM : EntityBaseVM<BlotterTransactions>
    {
        public BlotterTransactions _selectedValue;

        private DelegateCommand _addTemplateCommand;

        private DelegateCommand _addTransactionCommand;

        private DelegateCommand _addTransferCommand;

        private DelegateCommand _deleteCommand;

        private DelegateCommand _duplicateCommand;

        private DelegateCommand _editCommand;

        private DelegateCommand _infoCommand;

        public event EventHandler AddTemplateRaised;

        public event EventHandler AddTransactionRaised;

        public event EventHandler AddTransferRaised;

        public event EventHandler<TransactionsView> DeleteRaised;

        public event EventHandler<TransactionsView> DuplicateRaised;

        public event EventHandler<TransactionsView> EditRaised;
        public event EventHandler<TransactionsView> InfoRaised;
        public DelegateCommand AddTemplateCommand
        {
            get
            {
                return _addTemplateCommand ??= new DelegateCommand(() => AddTemplateRaised?.Invoke(this, EventArgs.Empty), () => false);
            }
        }

        public DelegateCommand AddTransactionCommand
        {
            get
            {
                return _addTransactionCommand ??= new DelegateCommand(() => AddTransactionRaised?.Invoke(this, EventArgs.Empty));
            }
        }

        public DelegateCommand AddTransferCommand
        {
            get
            {
                return _addTransferCommand ??= new DelegateCommand(() => AddTransferRaised?.Invoke(this, EventArgs.Empty));
            }
        }

        public DelegateCommand DeleteCommand
        {
            get
            {
                return _deleteCommand ??= new DelegateCommand(() => DeleteRaised?.Invoke(this, SelectedValue), () => SelectedValue != null);
            }
        }

        public DelegateCommand DuplicateCommand
        {
            get
            {
                return _duplicateCommand ??= new DelegateCommand(() => DuplicateRaised?.Invoke(this, SelectedValue), () => false);
            }
        }

        public DelegateCommand EditCommand
        {
            get
            {
                return _editCommand ??= new DelegateCommand(() => EditRaised?.Invoke(this, SelectedValue), () => SelectedValue != null);
            }
        }

        public DelegateCommand InfoCommand
        {
            get
            {
                return _infoCommand ??= new DelegateCommand(() => InfoRaised?.Invoke(this, SelectedValue), () => false);
            }
        }

        public BlotterTransactions SelectedValue
        {
            get => _selectedValue;
            set
            {
                SetProperty(ref _selectedValue, value);
                EditCommand.RaiseCanExecuteChanged();
                DuplicateCommand.RaiseCanExecuteChanged();
                DeleteCommand.RaiseCanExecuteChanged();
                InfoCommand.RaiseCanExecuteChanged();
            }
        }
    }
}
