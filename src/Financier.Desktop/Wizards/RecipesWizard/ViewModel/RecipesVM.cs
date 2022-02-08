﻿using Financier.Desktop.Data;
using System.Collections.Generic;
using System.Linq;

namespace Financier.Desktop.Wizards.RecipesWizard.ViewModel
{
    public class RecipesVM : WizardBaseVM
    {
        public RecipesVM(double totalAmount)
        {
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
            var page1 = new Page1VM(this.TotalAmount);
            page1.PropertyChanged += (_, e) => 
            {
                if (e.PropertyName == nameof(page1.Text))
                {
                    MoveNextCommand.RaiseCanExecuteChanged();
                }
            };

            _pages = new List<WizardPageBaseVM>()
            {
                page1,
                new Page2VM(this.TotalAmount),
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

        private TransactionDto TransformMonoTransaction(FinancierTransactionDto x)
        {
            var result = new TransactionDto
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
