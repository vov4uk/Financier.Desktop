using Financier.Desktop.Data;
using Prism.Commands;
using System.Linq;

namespace Financier.Desktop.ViewModel.Dialog
{
    public class TransferControlVM : DialogBaseVM
    {
        private readonly string[] TrackingProperies = new string[]
        {
            nameof(TransferDto.FromAmount),
            nameof(TransferDto.ToAccount),
            nameof(TransferDto.FromAccount),
        };
        private DelegateCommand _clearNotesCommand;

        public TransferControlVM(TransferDto transfer)
        {
            Transfer = transfer;
            Transfer.PropertyChanged += TransferPropertyChanged;
            transfer.RecalculateRate();
        }

        public TransferDto Transfer { get; }

        public DelegateCommand ClearNotesCommand => _clearNotesCommand ??= new DelegateCommand(() => { Transfer.Note = default; });

        public override object OnRequestSave() => Transfer;

        protected override bool CanSaveCommandExecute()
            => Transfer.FromAccount != null && Transfer.ToAccount != null && Transfer.FromAccountId != Transfer.ToAccountId;

        private void TransferPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (TrackingProperies.Contains(e.PropertyName))
            {
                SaveCommand.RaiseCanExecuteChanged();
            }
        }
    }
}
