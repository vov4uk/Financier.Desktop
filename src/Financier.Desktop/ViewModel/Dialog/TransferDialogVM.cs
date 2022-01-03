using Financier.DataAccess.Data;
using Financier.Desktop.Data;
using Prism.Commands;
using System.Collections.Generic;
using System.Linq;

namespace Financier.Desktop.ViewModel.Dialog
{
    public class TransferDialogVM : DialogBaseVM
    {
        private readonly string[] TrackingProperies = new string[] { nameof(TransferDTO.FromAmount), nameof(TransferDTO.ToAccount), nameof(TransferDTO.FromAccount), };
        private DelegateCommand _clearNotesCommand;

        public TransferDialogVM(TransferDTO transfer, List<Account> accounts)
        {
            Accounts = accounts;
            Transfer = transfer;
            Transfer.PropertyChanged += TransferPropertyChanged;
        }

        public List<Account> Accounts { get; }

        public DelegateCommand ClearNotesCommand
        {
            get
            {
                return _clearNotesCommand ??= new DelegateCommand(() => { Transfer.Note = default; });
            }
        }

        public TransferDTO Transfer { get; }
        public override object OnRequestSave()
        {
            return Transfer;
        }

        protected override bool CanSaveCommandExecute()
        {
            return Transfer.FromAccount != null && Transfer.ToAccount != null && Transfer.FromAccountId != Transfer.ToAccountId;
        }

        private void TransferPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (TrackingProperies.Contains(e.PropertyName))
            {
                SaveCommand.RaiseCanExecuteChanged();
            }
        }
    }
}
