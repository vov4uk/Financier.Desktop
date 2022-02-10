using Financier.Desktop.Data;
using Prism.Commands;
using System.Linq;

namespace Financier.Desktop.ViewModel.Dialog
{
    public class SubTransactionControlVM : DialogBaseVM
    {
        private readonly string[] TrackingProperies = new string[] { nameof(TransactionDto.FromAmount), nameof(TransactionDto.Account) };

        private DelegateCommand _changeFromAmountSignCommand;
        private DelegateCommand<int?> _clearCategoryCommand;
        private DelegateCommand _clearFromAmountCommand;
        private DelegateCommand _clearNotesCommand;
        private DelegateCommand _clearOriginalFromAmountCommand;
        private DelegateCommand _clearProjectCommand;
        public SubTransactionControlVM(TransactionDto transaction)
        {
            Transaction = transaction;
            Transaction.PropertyChanged += Transaction_PropertyChanged;
        }

        public TransactionDto Transaction { get; }

        public DelegateCommand ChangeFromAmountSignCommand
            => _changeFromAmountSignCommand ??= new DelegateCommand(() => { Transaction.IsAmountNegative = !Transaction.IsAmountNegative; });

        public DelegateCommand<int?> ClearCategoryCommand 
            => _clearCategoryCommand ??= new DelegateCommand<int?>(i => { Transaction.CategoryId = i; });

        public DelegateCommand ClearFromAmountCommand
            => _clearFromAmountCommand ??= new DelegateCommand(() => { Transaction.FromAmount = 0; });

        public DelegateCommand ClearNotesCommand
            => _clearNotesCommand ??= new DelegateCommand(() => { Transaction.Note = default; });

        public DelegateCommand ClearOriginalFromAmountCommand
            => _clearOriginalFromAmountCommand ??= new DelegateCommand(() => { Transaction.OriginalFromAmount = 0; });

        public DelegateCommand ClearProjectCommand
            => _clearProjectCommand ??= new DelegateCommand(() => { Transaction.ProjectId = default; });

        public override object OnRequestSave() => Transaction;

        protected override bool CanSaveCommandExecute() => !Transaction.IsSplitCategory || Transaction.UnsplitAmount == 0;

        private void Transaction_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (TrackingProperies.Contains(e.PropertyName))
            {
                SaveCommand.RaiseCanExecuteChanged();
            }
        }
    }
}
