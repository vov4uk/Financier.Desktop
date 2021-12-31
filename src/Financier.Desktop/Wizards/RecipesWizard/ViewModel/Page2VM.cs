using Financier.DataAccess.Data;
using Prism.Commands;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Financier.Desktop.Wizards.RecipesWizard.ViewModel
{
    public class Page2VM : RecipesWizardPageVMBase
    {
        public override string Title => "Transactions";

        public override bool IsValid() => true;

        private ObservableCollection<Category> categories;
        private ObservableCollection<Project> projects;

        public Page2VM(List<Category> categories, List<Project> projects, double totalAmount)
        {
            Categories = new ObservableCollection<Category>(categories);
            Projects = new ObservableCollection<Project>(projects.OrderByDescending(x => x.IsActive).ThenBy(x => x.Id));
            TotalAmount = totalAmount;
            financierTransactions = new();
        }

        private ObservableCollection<FinancierTransactionDTO> financierTransactions;
        private DelegateCommand<FinancierTransactionDTO> _deleteCommand;
        private DelegateCommand _addRowCommand;
        private DelegateCommand _totalCommand;

        public void SetTransactions(List<FinancierTransactionDTO> list)
        {
            FinancierTransactions = new ObservableCollection<FinancierTransactionDTO>(list);
            CalculateFromAmounts();
        }

        private void CalculateFromAmounts()
        {
            base.CalculatedAmount =
                FinancierTransactions.Sum(x => x.FromAmount) / 100.0;
        }

        public DelegateCommand<FinancierTransactionDTO> DeleteCommand
        {
            get
            {
                return _deleteCommand ??= new DelegateCommand<FinancierTransactionDTO>(tr =>
                {
                    financierTransactions.Remove(tr);
                    for (int i = 0; i < financierTransactions.Count; i++)
                    {
                        financierTransactions[i].Order = i + 1;
                    }
                });
            }
        }

        public DelegateCommand AddRowCommand
        {
            get
            {
                return _addRowCommand ??= new DelegateCommand(() => { financierTransactions.Add(new FinancierTransactionDTO() { Order = financierTransactions.Count + 1}); });
            }
        }

        public DelegateCommand TotalCommand
        {
            get
            {
                return _totalCommand ??= new DelegateCommand(CalculateFromAmounts);
            }
        }

        public ObservableCollection<FinancierTransactionDTO> FinancierTransactions
        {
            get => financierTransactions;
            private set
            {
                financierTransactions = value;
                RaisePropertyChanged(nameof(FinancierTransactions));
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

        public ObservableCollection<Project> Projects
        {
            get => projects;
            private set
            {
                projects = value;
                RaisePropertyChanged(nameof(Projects));
            }
        }
    }
}
