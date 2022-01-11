using Prism.Mvvm;
using System;

namespace Financier.Desktop.ViewModel.Dialog
{
    public class BaseTransactionDto : BindableBase
    {
        private DateTime date;
        private int id;
        private string note;
        private double rate;

        public DateTime Date
        {
            get => date;
            set
            {
                date = value;
                RaisePropertyChanged(nameof(Date));
            }
        }

        public int Id
        {
            get => id;
            set
            {
                id = value;
                RaisePropertyChanged(nameof(Id));
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

        public double Rate
        {
            get => rate;
            set
            {
                rate = value;
                RaisePropertyChanged(nameof(Rate));
                RaisePropertyChanged("RateString");
            }
        }
    }
}
