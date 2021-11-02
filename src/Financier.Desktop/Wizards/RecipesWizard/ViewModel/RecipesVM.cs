using Financier.DataAccess.Data;
using Financier.Desktop.MonoWizard.ViewModel;
using Financier.Desktop.ViewModel.Dialog;
using Financier.Desktop.Wizards.MonoWizard.ViewModel;
using System.Collections.Generic;
using System.Linq;

namespace Financier.Desktop.Wizards.RecipesWizard.ViewModel
{
    public class RecipesVM : WizardBaseVM
    {
        private readonly List<Category> categories;
        private readonly List<Project> projects;

        public RecipesVM(List<Category> categories, List<Project> projects)
        {
            this.categories = categories;
            this.projects = projects;
        }

        public double TotalAmount
        {
            get;
            set;
        }
        public List<TransactionDialogVM> TransactionsToImport { get; private set; }

        public override void AfterCurrentPageUpdated(WizardPageBaseVM newValue)
        {
        }

        public override void BeforeCurrentPageUpdated(WizardPageBaseVM old, WizardPageBaseVM newValue)
        {
            if (old is Page1VM page1 && newValue is Page2VM)
            {
                page1.CalculateCurrentAmount();
                ((Page2VM)newValue).SetMonoTransactions(page1.Amounts);
                Logger.Info($"MonoTransactions count -> {page1.Amounts.Count}");
            }
        }

        public override void CreatePages()
        {
            _pages = new List<WizardPageBaseVM>()
            {
                new Page1VM(){ TotalAmount = this.TotalAmount},
                new Page2VM(categories, projects) { TotalAmount = this.TotalAmount},
            }.AsReadOnly();

            CurrentPage = Pages[0];
        }
        public override void OnRequestClose(bool save)
        {
            if (save)
            {
                TransactionsToImport = _pages.OfType<Page2VM>()
                    .Single()
                    .FinancierTransactions
                    .Select(TransformMonoTransaction)
                    .ToList();
            }
        }

        private TransactionDialogVM TransformMonoTransaction(FinancierTransactionVM x)
        {
            var result = new TransactionDialogVM
            {
                Id = 0,
                FromAmount = x.FromAmount,
                IsAmountNegative = x.FromAmount < 0,
                OriginalFromAmount = x.OriginalFromAmount ?? 0,
                OriginalCurrencyId = x.OriginalCurrencyId,
                Note = x.Note,
                LocationId = x.LocationId,
                ProjectId = x.ProjectId,
                CategoryId = x.CategoryId,
                Category = default,
            };

            return result;
        }
    }
}
