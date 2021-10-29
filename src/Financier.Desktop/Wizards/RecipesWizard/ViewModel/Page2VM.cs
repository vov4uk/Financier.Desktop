using Financier.DataAccess.Data;
using Financier.Desktop.MonoWizard.ViewModel;
using Financier.Desktop.Wizards.MonoWizard.ViewModel;
using Prism.Commands;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Financier.Desktop.Wizards.RecipesWizard.ViewModel
{
    public class Page2VM : WizardPageBaseVM
    {
        public override string Title => "Transactions";

        public override bool IsValid() => true;

        private RangeObservableCollection<Category> categories;

        public Page2VM(List<Category> categories)
        {
            this.categories = new RangeObservableCollection<Category>(categories);
        }

        private RangeObservableCollection<FinancierTransactionVM> financierTransactions;
        private DelegateCommand<FinancierTransactionVM> _deleteCommand;
        private DelegateCommand _addRowCommand;
        private DelegateCommand _totalCommand;
        private double calculatedAmount;
        private double totalAmount;

        public void SetMonoTransactions(List<FinancierTransactionVM> list)
        {
            FinancierTransactions = new RangeObservableCollection<FinancierTransactionVM>(list);
            CalculateFromAmounts();
        }

        private void CalculateFromAmounts()
        {
            this.CalculatedAmount =
                FinancierTransactions.Sum(x => x.FromAmount) / 100.0;
        }

        public double TotalAmount
        {
            get => totalAmount;
            set
            {
                totalAmount = value;
                this.RaisePropertyChanged(nameof(this.TotalAmount));
                this.RaisePropertyChanged(nameof(this.Diff));
            }
        }

        public double CalculatedAmount
        {
            get => calculatedAmount;
            set
            {
                calculatedAmount = value;
                this.RaisePropertyChanged(nameof(this.CalculatedAmount));
                this.RaisePropertyChanged(nameof(this.Diff));
            }
        }

        public double Diff => TotalAmount - CalculatedAmount;

        public DelegateCommand<FinancierTransactionVM> DeleteCommand
        {
            get
            {
                return _deleteCommand ??= new DelegateCommand<FinancierTransactionVM>(tr => { financierTransactions.Remove(tr); });
            }
        }

        public DelegateCommand AddRowCommand
        {
            get
            {
                return _addRowCommand ??= new DelegateCommand(() => { financierTransactions.Add(new FinancierTransactionVM()); });
            }
        }

        public DelegateCommand TotalCommand
        {
            get
            {
                return _totalCommand ??= new DelegateCommand(CalculateFromAmounts);
            }
        }

        public RangeObservableCollection<FinancierTransactionVM> FinancierTransactions
        {
            get => financierTransactions;
            set
            {
                financierTransactions = value;
                RaisePropertyChanged(nameof(FinancierTransactions));
            }
        }

        public RangeObservableCollection<Category> Categories
        {
            get => categories;
            set
            {
                categories = value;
                RaisePropertyChanged(nameof(Categories));
            }
        }
    }
}
