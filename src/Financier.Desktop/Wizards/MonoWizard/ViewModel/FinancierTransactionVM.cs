using Prism.Mvvm;

namespace Financier.Desktop.MonoWizard.ViewModel
{
    public class FinancierTransactionVM : BindableBase
    {
        private int toAccountId;
        private int categoryId;
        private int locationId;
        private string note;

        public int FromAccountId { get; set; }
        public int MonoAccountId { get; set; }
        public long FromAmount { get; set; }

        public int ToAccountId
        {
            get => toAccountId;
            set
            {
                toAccountId = value;
                RaisePropertyChanged(nameof(ToAccountId));
            }
        }

        public long ToAmount { get; set; }
        public long? OriginalFromAmount { get; set; }
        public int OriginalCurrencyId { get; set; }

        public int CategoryId
        {
            get => categoryId;
            set
            {
                categoryId = value;
                RaisePropertyChanged(nameof(CategoryId));
            }
        }

        public int LocationId
        {
            get => locationId;
            set
            {
                locationId = value;
                RaisePropertyChanged(nameof(LocationId));
            }
        }

        public string Note
        {
            get => note;
            set
            {
                note = value;
                RaisePropertyChanged(nameof(Note));
            }
        }

        public long DateTime { get; set; }
    }
}
