﻿using Prism.Mvvm;

namespace Financier.Desktop.Wizards
{
    public class FinancierTransactionDto : BindableBase
    {
        private int categoryId;
        private int locationId;
        private string note;
        private int order;
        private int projectId;
        private int toAccountId;

        public int CategoryId
        {
            get => categoryId;
            set
            {
                categoryId = value;
                RaisePropertyChanged(nameof(CategoryId));
            }
        }

        public long DateTime { get; set; }

        public int FromAccountId { get; set; }

        public long FromAmount { get; set; }

        public int MCC { get; set; }

        public int LocationId
        {
            get => locationId;
            set
            {
                locationId = value;
                RaisePropertyChanged(nameof(LocationId));
            }
        }

        public int? MonoAccountId { get; set; }

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

        public int ProjectId
        {
            get => projectId;
            set
            {
                projectId = value;
                RaisePropertyChanged(nameof(ProjectId));
            }
        }

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
    }
}
