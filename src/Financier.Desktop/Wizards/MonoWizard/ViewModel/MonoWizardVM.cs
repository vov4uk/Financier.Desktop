using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CsvHelper;
using Financier.DataAccess.Data;
using Financier.DataAccess.Monobank;
using Financier.Desktop.Wizards;
using Financier.Desktop.Wizards.MonoWizard.ViewModel;

namespace Financier.Desktop.MonoWizard.ViewModel
{
    public class MonoWizardVM : WizardBaseVM
    {
        private readonly List<Account> accounts;
        private readonly List<Currency> currencies;
        private readonly List<Location> locations;
        private readonly List<Category> categories;
        private readonly List<Project> projects;
        private readonly string csvFilePath;
        private readonly List<MonoTransaction> monoTransactions = new();

        public MonoWizardVM(List<Account> accounts,
            List<Currency> currencies,
            List<Location> locations,
            List<Category> categories,
            List<Project> projects,
            string csvFilePath)
        {
            this.accounts = accounts;
            this.currencies = currencies;
            this.locations = locations;
            this.categories = categories;
            this.projects = projects;
            this.csvFilePath = csvFilePath;
        }

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
                ((Page3VM)value).SetMonoTransactions(page2.MonoTransactions);
                Logger.Info($"MonoTransactions count -> {page2.MonoTransactions.Count}");
            }
        }

        public Account MonoBankAccount { get; set; }

        public List<Transaction> TransactionsToImport { get; set; }

        public async Task LoadTransactions()
        {
            if (File.Exists(csvFilePath))
            {
                Logger.Info($"csvFilePath -> {csvFilePath}");
                await using FileStream file = File.OpenRead(csvFilePath);
                using StreamReader streamReader = new StreamReader(file, Encoding.UTF8);
                using var csv = new CsvReader(streamReader, CultureInfo.InvariantCulture);
                var records = await csv.GetRecordsAsync<MonoTransaction>().ToListAsync();
                monoTransactions.AddRange(records);
                this.CreatePages();
                CurrentPage = Pages[0];
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


       public override void OnRequestClose(bool save)
        {
            if (save)
            {
                TransactionsToImport = _pages.OfType<Page3VM>()
                    .Single()
                    .FinancierTransactions
                    .Select(TransformMonoTransaction)
                    .ToList();
            }
        }

        private Transaction TransformMonoTransaction(FinancierTransactionVM x)
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
            else
            if (x.FromAccountId > 0) // Transfer To Mono
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
