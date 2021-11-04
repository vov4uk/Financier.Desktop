using Financier.DataAccess.Data;
using Prism.Commands;
using System.Collections.ObjectModel;

namespace Financier.Desktop.ViewModel.Dialog
{
    public class TransferDialogVM : DialogBaseVM
    {
        private DelegateCommand _clearNotesCommand;

        private TransferDTO _transfer;

        public ObservableCollection<Account> Accounts { get; set; }

        public DelegateCommand ClearNotesCommand
        {
            get
            {
                return _clearNotesCommand ??= new DelegateCommand(() => { Transfer.Note = default; });
            }
        }

        public TransferDTO Transfer
        {
            get => _transfer;
            set
            {
                _transfer = value;
                RaisePropertyChanged(nameof(Transfer));
            }
        }
    }
}
