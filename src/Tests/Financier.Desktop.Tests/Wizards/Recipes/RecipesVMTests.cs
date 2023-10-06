namespace Financier.Desktop.Tests.Wizards.Recipes
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Financier.Common.Entities;
    using Financier.Desktop.Data;
    using Financier.Desktop.Wizards.RecipesWizard.ViewModel;
    using Xunit;

    public class RecipesVMTests
    {
        [Fact]
        public void MoveNextCommand_Executed2Times_ReceiveParsedTransactions()
        {
            DbManual.SetupTests(new List<Common.Model.AccountFilterModel>());
            DbManual.SetupTests(new List<Common.Model.CurrencyModel>());
            DbManual.SetupTests(new List<Common.Model.CategoryModel>());
            double total = -100.0;
            List<TransactionDto> actual = default;
            List<TransactionDto> expected = new List<TransactionDto>
            {
                new TransactionDto { FromAmount = -5000, IsAmountNegative = true,  Note = "Beer Baltyka" },
                new TransactionDto { FromAmount = -2500, IsAmountNegative = true,  Note = "Milk Molokia 25%" },
                new TransactionDto { FromAmount = -2550, IsAmountNegative = true,  Note = string.Empty },
                new TransactionDto { FromAmount = 50,    IsAmountNegative = false, Note = "Discount" },
            };
            foreach (var item in expected)
            {
                item.CategoryId = 0;
                item.LocationId = 0;
                item.OriginalCurrencyId = 0;
                item.OriginalFromAmount = 0;
                item.ProjectId = 0;
            }

            var vm = new RecipesVM(total);
            vm.RequestClose += (o, args) =>
            {
                if (args)
                {
                    actual = o as List<TransactionDto>;
                }
            };

            var page1 = vm.CurrentPage as Page1VM;
            page1.Text = @"
Beer Baltyka 0 50,0 A
Milk Molokia 2.5% 25.0 A
25.50 A
Discount -0.5 A";
            vm.MoveNextCommand.Execute();

            var page2 = vm.CurrentPage as Page2VM;

            page2.TotalCommand.Execute();

            vm.MoveNextCommand.Execute();

            Assert.Equal(total, page2.CalculatedAmount);
            Assert.Equal(total, page2.TotalAmount);
            Assert.Equal(0, page2.Diff);
            Assert.Equal(expected, actual, new Comparer());
        }

        [Fact]
        public void CancelCommand_Execute_TransactionsEmpty()
        {
            List<TransactionDto> output = new ();
            double total = -100.0;
            List<TransactionDto> actual = default;
            bool save = true;
            var vm = new RecipesVM(total);
            vm.RequestClose += (o, args) =>
            {
                save = args;
                output = o as List<TransactionDto>;
            };

            var page1 = vm.CurrentPage as Page1VM;
            page1.Text = "50,0 A";
            vm.MoveNextCommand.Execute();

            var page2 = vm.CurrentPage as Page2VM;

            vm.CancelCommand.Execute();

            Assert.Equal(output, actual, new Comparer());
        }

        [Fact]
        public void TotalCommand_Execute_CorrectTotalAmount()
        {
            double total = -100.0;
            var vm = new RecipesVM(total);
            var page1 = vm.CurrentPage as Page1VM;
            page1.Text = @"
Beer Baltyka 0 50,0 A
Milk Molokia 2.5% 25.0 A
25.50 A";
            vm.MoveNextCommand.Execute();

            var page2 = vm.CurrentPage as Page2VM;
            page2.TotalCommand.Execute();

            Assert.Equal(-100.5, page2.CalculatedAmount);
            Assert.Equal(.5, page2.Diff);
            Assert.Equal(total, page2.TotalAmount);
        }

        private class Comparer : IEqualityComparer<TransactionDto>
        {
            public bool Equals(TransactionDto x, TransactionDto y)
            {
                var res = x.Category == y.Category
                   && x.CategoryId == y.CategoryId
                   && x.DateTime == y.DateTime
                   && x.FromAmount == y.FromAmount
                   && x.Id == y.Id
                   && x.LocationId == y.LocationId
                   && x.Note == y.Note
                   && x.OriginalCurrency == y.OriginalCurrency
                   && x.OriginalCurrencyId == y.OriginalCurrencyId
                   && x.OriginalFromAmount == y.OriginalFromAmount
                   && x.PayeeId == y.PayeeId
                   && x.ProjectId == y.ProjectId;

                return res;
            }

            public int GetHashCode([DisallowNull] TransactionDto obj)
            {
                return obj.Id.GetHashCode();
            }
        }
    }
}
