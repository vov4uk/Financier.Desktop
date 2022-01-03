using Financier.DataAccess.Data;
using Financier.Desktop.Data;
using System.Collections.Generic;
using System.Linq;

namespace Financier.Desktop.Wizards.RecipesWizard.ViewModel
{
    public class RecipesVM : WizardBaseVM
    {
        private readonly List<Category> categories;
        private readonly List<Project> projects;

        public RecipesVM(double totalAmount, List<Category> categories, List<Project> projects)
        {
            this.categories = categories;
            this.projects = projects;
            TotalAmount = totalAmount;
            CreatePages();
        }

        public double TotalAmount
        {
            get;
            private set;
        }

        public override void AfterCurrentPageUpdated(WizardPageBaseVM newValue)
        {
        }

        public override void BeforeCurrentPageUpdated(WizardPageBaseVM old, WizardPageBaseVM newValue)
        {
            if (old is Page1VM page1 && newValue is Page2VM)
            {
                page1.CalculateCurrentAmount();
                ((Page2VM)newValue).SetTransactions(page1.Amounts);
                Logger.Info($"MonoTransactions count -> {page1.Amounts.Count}");
            }
        }

        public override void CreatePages()
        {
            _pages = new List<WizardPageBaseVM>()
            {
                new Page1VM(this.TotalAmount),
                new Page2VM(categories, projects, this.TotalAmount),
            }.AsReadOnly();

            CurrentPage = Pages[0];
        }
        public override object OnRequestClose(bool save)
        {
            if (save)
            {
                return _pages.OfType<Page2VM>()
                    .Single()
                    .FinancierTransactions
                    .Select(TransformMonoTransaction)
                    .ToList();
            }
            return null;
        }

        private TransactionDTO TransformMonoTransaction(FinancierTransactionDTO x)
        {
            var result = new TransactionDTO
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
