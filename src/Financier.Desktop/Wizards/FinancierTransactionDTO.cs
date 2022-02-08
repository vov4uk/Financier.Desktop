using Prism.Mvvm;

namespace Financier.Desktop.Wizards
{
    public class FinancierTransactionDto : BindableBase
    {
        private long categoryId;
        private long locationId;
        private string note;
        private int order;
        private long projectId;
        private long toAccountId;

        public long CategoryId
        {
            get => categoryId;
            set
            {
                categoryId = value;
                RaisePropertyChanged(nameof(CategoryId));
            }
        }

        public long DateTime { get; set; }

        public long FromAccountId { get; set; }

        public long FromAmount { get; set; }

        public long LocationId
        {
            get => locationId;
            set
            {
                locationId = value;
                RaisePropertyChanged(nameof(LocationId));
            }
        }

        public long? MonoAccountId { get; set; }

        public string Note
        {
            get => note;
            set
            {
                note = value;
                RaisePropertyChanged(nameof(Note));
            }
        }

        public int Order
        {
            get => order;
            set
            {
                order = value;
                RaisePropertyChanged(nameof(Order));
            }
        }
        public int OriginalCurrencyId { get; set; }

        public long? OriginalFromAmount { get; set; }

        public long ProjectId
        {
            get => projectId;
            set
            {
                projectId = value;
                RaisePropertyChanged(nameof(ProjectId));
            }
        }

        public long ToAccountId
        {
            get => toAccountId;
            set
            {
                toAccountId = value;
                RaisePropertyChanged(nameof(ToAccountId));
            }
        }

        public long ToAmount { get; set; }
    }
}
