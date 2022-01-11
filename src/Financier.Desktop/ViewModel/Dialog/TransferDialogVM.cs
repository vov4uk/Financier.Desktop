using Financier.DataAccess.Data;
using Prism.Commands;
using System.Collections.ObjectModel;

namespace Financier.Desktop.ViewModel.Dialog
{
    public class TransferDialogVM : DialogBaseVM
    {
        private readonly string[] TrackingProperies = new string[] { nameof(TransferDto.FromAmount), nameof(TransferDto.ToAccount), nameof(TransferDto.FromAccount), };
        private DelegateCommand _clearNotesCommand;

        public TransferDialogVM(TransferDto transfer, List<Account> accounts)

        public ObservableCollection<Account> Accounts { get; set; }

        public DelegateCommand ClearNotesCommand
        {
            get
            {
                return _clearNotesCommand ??= new DelegateCommand(() => { Transfer.Note = default; });
            }
        }

        public TransferDto Transfer { get; }
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
