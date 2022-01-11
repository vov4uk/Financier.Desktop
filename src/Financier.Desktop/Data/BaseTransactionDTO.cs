using Prism.Mvvm;
using System;

namespace Financier.Desktop.Data
{
    public class BaseTransactionDto : BindableBase
    {
        private DateTime date;
        private DateTime time;
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

        public DateTime Time
        {
            get => time;
            set
            {
                time = value;
                RaisePropertyChanged(nameof(Time));
            }
        }

        public DateTime DateTime
        {
            get { return new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, time.Second); }
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
