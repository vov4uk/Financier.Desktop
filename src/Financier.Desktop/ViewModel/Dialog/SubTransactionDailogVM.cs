using Financier.DataAccess.Data;
using Prism.Commands;
using System.Collections.ObjectModel;

namespace Financier.Desktop.ViewModel.Dialog
{
    public class SubTransactionDailogVM : DialogBaseVM
    {
        private DelegateCommand _changeFromAmountSignCommand;

        private DelegateCommand _clearOriginalFromAmountCommand;

        private DelegateCommand _clearFromAmountCommand;

        private DelegateCommand<int?> _clearCategoryCommand;

        private DelegateCommand _clearProjectCommand;

        private DelegateCommand _clearNotesCommand;

        private TransactionDTO transaction;

        public ObservableCollection<Category> Categories { get; set; }

        public ObservableCollection<Project> Projects { get; set; }

        public TransactionDTO Transaction
        {
            get => transaction;
            set
            {
                if (transaction != null)
                {
                    transaction.PropertyChanged -= Transaction_PropertyChanged;
                }
                transaction = value;
                if (transaction != null)
                {
                    transaction.PropertyChanged += Transaction_PropertyChanged;
                }
                RaisePropertyChanged(nameof(Transaction));
            }
        }

        private void Transaction_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TransactionDTO.FromAmount))
            {
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        public DelegateCommand ChangeFromAmountSignCommand
        {
            get
            {
                return _changeFromAmountSignCommand ??=
                    new DelegateCommand(() => { Transaction.IsAmountNegative = !Transaction.IsAmountNegative; });
            }
        }

        public DelegateCommand<int?> ClearCategoryCommand
        {
            get
            {
                return _clearCategoryCommand ??= new DelegateCommand<int?>( i => { Transaction.CategoryId = i; });
            }
        }

        public DelegateCommand ClearFromAmountCommand
        {
            get { return _clearFromAmountCommand ??= new DelegateCommand(() => { Transaction.FromAmount = 0; }); }
        }

        public DelegateCommand ClearNotesCommand
        {
            get { return _clearNotesCommand ??= new DelegateCommand(() => { Transaction.Note = default; }); }
        }

        public DelegateCommand ClearOriginalFromAmountCommand
        {
            get { return _clearOriginalFromAmountCommand ??= new DelegateCommand(() => { Transaction.OriginalFromAmount = 0; }); }
        }

        public DelegateCommand ClearProjectCommand
        {
            get { return _clearProjectCommand ??= new DelegateCommand(() => { Transaction.ProjectId = default; }); }
        }

        protected override bool CanSaveCommandExecute()
        {
            if (Transaction.IsSplitCategory) return Transaction.UnsplitAmount == 0;
            return true;
        }
    }
}
