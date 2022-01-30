using Financier.DataAccess.Data;
using Prism.Commands;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Financier.Desktop.Wizards.RecipesWizard.ViewModel
{
    public class Page2VM : RecipesWizardPageVMBase
    {
        private DelegateCommand _addRowCommand;
        private DelegateCommand<FinancierTransactionDto> _deleteCommand;
        private DelegateCommand _totalCommand;
        private DelegateCommand _clearAllNotesCommand;

        private ObservableCollection<Category> categories;
        private ObservableCollection<FinancierTransactionDto> financierTransactions;
        private ObservableCollection<Project> projects;
        public Page2VM(List<Category> categories, List<Project> projects, double totalAmount)
        {
            Categories = new ObservableCollection<Category>(categories);
            Projects = new ObservableCollection<Project>(projects.OrderByDescending(x => x.IsActive).ThenBy(x => x.Id));
            TotalAmount = totalAmount;
            financierTransactions = new();
        }

        public DelegateCommand AddRowCommand
        {
            get
            {
                return _addRowCommand ??= new DelegateCommand(() => { financierTransactions.Add(new FinancierTransactionDto() { Order = financierTransactions.Count + 1 }); });
            }
        }

        public ObservableCollection<Category> Categories
        {
            get => categories;
            private set
            {
                categories = value;
                RaisePropertyChanged(nameof(Categories));
            }
        }

        public DelegateCommand<FinancierTransactionDto> DeleteRowCommand
        {
            get
            {
                return _deleteCommand ??= new DelegateCommand<FinancierTransactionDto>(tr =>
                {
                    financierTransactions.Remove(tr);
                    for (int i = 0; i < financierTransactions.Count; i++)
                    {
                        financierTransactions[i].Order = i + 1;
                    }
                });
            }
        }

        public ObservableCollection<FinancierTransactionDto> FinancierTransactions
        {
            get => financierTransactions;
            private set
            {
                financierTransactions = value;
                RaisePropertyChanged(nameof(FinancierTransactions));
            }
        }

        public ObservableCollection<Project> Projects
        {
            get => projects;
            private set
            {
                projects = value;
                RaisePropertyChanged(nameof(Projects));
            }
        }

        public override string Title => "Transactions";

        public DelegateCommand TotalCommand
        {
            get
            {
                return _totalCommand ??= new DelegateCommand(CalculateFromAmounts);
            }
        }

        public DelegateCommand ClearAllNotesCommand
        {
            get
            {
                return _clearAllNotesCommand ??= new DelegateCommand(ClearAllNotes);
            }
        }

        public override bool IsValid() => true;
        public void SetTransactions(List<FinancierTransactionDto> list)
        {
            FinancierTransactions = new ObservableCollection<FinancierTransactionDto>(list);
            CalculateFromAmounts();
        }

        private void CalculateFromAmounts()
        {
            base.CalculatedAmount =
                FinancierTransactions.Sum(x => x.FromAmount) / 100.0;
        }

        private void ClearAllNotes()
        {
            foreach (var item in FinancierTransactions)
            {
                item.Note = null;
            }
        }
    }
}
