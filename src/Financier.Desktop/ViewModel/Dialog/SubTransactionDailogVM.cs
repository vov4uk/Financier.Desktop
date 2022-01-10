using Financier.DataAccess.Data;
using Financier.Desktop.Data;
using Prism.Commands;
using System.Collections.Generic;
using System.Linq;

namespace Financier.Desktop.ViewModel.Dialog
{
    public class SubTransactionDailogVM : DialogBaseVM
    {
        private readonly string[] TrackingProperies = new string[] { nameof(TransactionDTO.FromAmount), nameof(TransactionDTO.Account) };

        private DelegateCommand _changeFromAmountSignCommand;
        private DelegateCommand<int?> _clearCategoryCommand;
        private DelegateCommand _clearFromAmountCommand;
        private DelegateCommand _clearNotesCommand;
        private DelegateCommand _clearOriginalFromAmountCommand;
        private DelegateCommand _clearProjectCommand;
        public SubTransactionDailogVM(
            TransactionDTO transaction,
            List<Category> categories,
            List<Project> projects)
        {
            Categories = categories;
            Projects = projects;
            Transaction = transaction;
            Transaction.PropertyChanged += Transaction_PropertyChanged;
        }

        public List<Category> Categories { get; }

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
                return _clearCategoryCommand ??= new DelegateCommand<int?>(i => { Transaction.CategoryId = i; });
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

        public List<Project> Projects { get; }

        public TransactionDTO Transaction { get; }

        public override object OnRequestSave()
        {
            return Transaction;
        }

        protected override bool CanSaveCommandExecute()
        {
            if (Transaction.IsSplitCategory) return Transaction.UnsplitAmount == 0;
            return true;
        }

        private void Transaction_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (TrackingProperies.Contains(e.PropertyName))
            {
                SaveCommand.RaiseCanExecuteChanged();
            }
        }
    }
}
