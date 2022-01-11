using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Financier.DataAccess.Data;
using Financier.DataAccess.Monobank;

namespace Financier.Desktop.Wizards.MonoWizard.ViewModel
{
    public class MonoWizardVM : WizardBaseVM
    {
        private readonly List<Account> accounts;
        private readonly List<Category> categories;
        private readonly List<Currency> currencies;
        private readonly List<Location> locations;
        private readonly List<MonoTransaction> monoTransactions = new();
        private readonly List<Project> projects;
        public MonoWizardVM(
            IEnumerable<MonoTransaction> monoTransactions,
            IEnumerable<Account> accounts,
            IEnumerable<Currency> currencies,
            IEnumerable<Location> locations,
            IEnumerable<Category> categories,
            IEnumerable<Project> projects)
        {
            this.monoTransactions = new(monoTransactions);
            this.accounts = new(accounts);
            this.currencies = new(currencies);
            this.locations = new(locations);
            this.categories = new(categories);
            this.projects = new(projects);

            CreatePages();
            CurrentPage = Pages[0];
        }

        //TODO - remove
        public Account MonoBankAccount { get; set; }

        public override void AfterCurrentPageUpdated(WizardPageBaseVM currentPage)
        {
            if (currentPage != null)
            {
                currentPage.IsCurrentPage = true;
                Logger.Info($"Current page -> {_currentPage.GetType().FullName}");
            }
        }

        public override void BeforeCurrentPageUpdated(WizardPageBaseVM currentPage, WizardPageBaseVM value)
        {
            if (currentPage != null)
                currentPage.IsCurrentPage = false;

            if (currentPage is Page1VM page1 && value is Page2VM)
            {
                var monoAccount = page1.MonoAccount;
                ((Page2VM)value).MonoAccount = monoAccount;
                MonoBankAccount = monoAccount;
                Logger.Info($"MonoBankAccount -> {JsonSerializer.Serialize(monoAccount)}");
            }

            if (currentPage is Page2VM page2 && value is Page3VM)
            {
                ((Page3VM)value).MonoAccount = MonoBankAccount;
                ((Page3VM)value).SetMonoTransactions(page2.GetMonoTransactions());
                Logger.Info($"MonoTransactions count -> {page2.GetMonoTransactions().Count}");
            }
        }

        public override void CreatePages()
        {
            _pages = new List<WizardPageBaseVM>
                {
                    new Page1VM(accounts),
                    new Page2VM(monoTransactions),
                    new Page3VM(accounts, currencies, locations, categories, projects)
                }.AsReadOnly();
        }


        public override object OnRequestClose(bool save)
        {
            if (save)
            {
                return _pages.OfType<Page3VM>()
                    .Single()
                    .FinancierTransactions
                    .Select(TransformMonoTransaction)
                    .ToList();
            }
            return null;
        }

        private Transaction TransformMonoTransaction(FinancierTransactionDTO x)
        {
            var result = new Transaction
            {
                Id = 0,
                FromAmount = x.FromAmount,
                OriginalFromAmount = x.OriginalFromAmount ?? 0,
                OriginalCurrencyId = x.OriginalCurrencyId,
                Note = x.Note,
                LocationId = x.LocationId,
                ProjectId = x.ProjectId,
                CategoryId = 0,
                Category = default,
                DateTime = x.DateTime,
                ToAmount = 0
            };

            if (x.ToAccountId > 0) // Transfer From Mono
            {
                result.FromAccountId = x.MonoAccountId;
                result.ToAccountId = x.ToAccountId;
                result.ToAmount = Math.Abs(x.OriginalFromAmount ?? x.FromAmount);
            }
            else if (x.FromAccountId > 0) // Transfer To Mono
            {
                result.FromAccountId = x.FromAccountId;
                result.ToAccountId = x.MonoAccountId;
                result.ToAmount = Math.Abs(x.OriginalFromAmount ?? x.FromAmount);
                result.FromAmount = -1 * Math.Abs(x.OriginalFromAmount ?? x.FromAmount);
            }
            else // Expanse
            {
                result.FromAccountId = x.MonoAccountId;
                result.CategoryId = x.CategoryId;
                result.ToAccountId = 0;
                result.ToAccount = default;
                result.ToAmount = 0;
            }

            return result;
        }
    }
}
