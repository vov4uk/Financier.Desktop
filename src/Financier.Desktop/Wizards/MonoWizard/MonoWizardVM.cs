using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Financier.Common.Model;
using Financier.DataAccess.Data;

namespace Financier.Desktop.Wizards.MonoWizard.ViewModel
{
    public class MonoWizardVM : WizardBaseVM
    {
        private readonly List<BankTransaction> monoTransactions;
        private readonly Dictionary<int, BlotterModel> lastTransactions;
        private readonly string Bank;

        public MonoWizardVM(string bank, IEnumerable<BankTransaction> monoTransactions, Dictionary<int, BlotterModel> lastTransactions)
        {
            this.monoTransactions = new(monoTransactions);
            this.Bank = bank;
            this.lastTransactions = lastTransactions;
            CreatePages();
            CurrentPage = Pages[0];
        }

        public override void AfterCurrentPageUpdated(WizardPageBaseVM newValue)
        {
            if (newValue != null)
            {
                newValue.IsCurrentPage = true;
                Logger.Info($"Current page -> {_currentPage.GetType().FullName}");
            }
        }

        public override void BeforeCurrentPageUpdated(WizardPageBaseVM old, WizardPageBaseVM newValue)
        {
            if (old != null)
                old.IsCurrentPage = false;

            if (old is Page1VM page1 && newValue is Page2VM)
            {
                var monoAccount = page1.MonoAccount;
                ((Page2VM)newValue).MonoAccount = monoAccount;
                Logger.Info($"MonoBankAccount -> {JsonSerializer.Serialize(monoAccount)}");
            }

            if (old is Page2VM page2 && newValue is Page3VM)
            {
                ((Page3VM)newValue).MonoAccount = ((Page2VM)old).MonoAccount;
                ((Page3VM)newValue).SetMonoTransactions(page2.GetMonoTransactions());
                Logger.Info($"MonoTransactions count -> {page2.GetMonoTransactions().Count}");
            }
        }

        public override void CreatePages()
        {
            _pages = new List<WizardPageBaseVM>
                {
                    new Page1VM(Bank),
                    new Page2VM(monoTransactions, lastTransactions),
                    new Page3VM()
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

        private Transaction TransformMonoTransaction(FinancierTransactionDto x)
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
                result.FromAccountId = x.MonoAccountId ?? 0;
                result.ToAccountId = x.ToAccountId;
                result.ToAmount = Math.Abs(x.OriginalFromAmount ?? x.FromAmount);
            }
            else if (x.FromAccountId > 0) // Transfer To Mono
            {
                result.FromAccountId = x.FromAccountId;
                result.ToAccountId = x.MonoAccountId ?? 0;
                result.ToAmount = Math.Abs(x.OriginalFromAmount ?? x.FromAmount);
                result.FromAmount = -1 * Math.Abs(x.OriginalFromAmount ?? x.FromAmount);
            }
            else // Expanse
            {
                result.FromAccountId = x.MonoAccountId ?? 0;
                result.CategoryId = x.CategoryId;
                result.ToAccountId = 0;
                result.ToAccount = default;
                result.ToAmount = 0;
            }

            return result;
        }
    }
}
