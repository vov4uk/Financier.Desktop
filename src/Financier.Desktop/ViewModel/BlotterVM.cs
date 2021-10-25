namespace Financier.Desktop.ViewModel
{
    using Financier.DataAccess.View;
    using Prism.Commands;
    using System;

    public class BlotterVM : EntityBaseVM<BlotterTransactions>
    {
        //private BlotterTransactions _selectedValue;

        private DelegateCommand _addTemplateCommand;

        private DelegateCommand _addTransferCommand;

        private DelegateCommand _duplicateCommand;

        private DelegateCommand _infoCommand;

        public event EventHandler AddTemplateRaised;

        public event EventHandler AddTransferRaised;

        public event EventHandler<TransactionsView> DuplicateRaised;

        public event EventHandler<TransactionsView> InfoRaised;

        public DelegateCommand AddTemplateCommand
        {
            get
            {
                return _addTemplateCommand ??= new DelegateCommand(() => AddTemplateRaised?.Invoke(this, EventArgs.Empty), () => false);
            }
        }

        public DelegateCommand AddTransferCommand
        {
            get
            {
                return _addTransferCommand ??= new DelegateCommand(() => AddTransferRaised?.Invoke(this, EventArgs.Empty));
            }
        }

        public DelegateCommand DuplicateCommand
        {
            get
            {
                return _duplicateCommand ??= new DelegateCommand(() => DuplicateRaised?.Invoke(this, SelectedValue), () => false);
            }
        }

        public DelegateCommand InfoCommand
        {
            get
            {
                return _infoCommand ??= new DelegateCommand(() => InfoRaised?.Invoke(this, SelectedValue), () => false);
            }
        }

        // Need for commands not avaliable in base class
//#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
//        public BlotterTransactions SelectedValue
//#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
//        {
//            get => _selectedValue;
//            set
//            {
//                SetProperty(ref _selectedValue, value);
//                EditCommand.RaiseCanExecuteChanged();
//                DuplicateCommand.RaiseCanExecuteChanged();
//                DeleteCommand.RaiseCanExecuteChanged();
//                InfoCommand.RaiseCanExecuteChanged();
//            }
//        }
    }
}
