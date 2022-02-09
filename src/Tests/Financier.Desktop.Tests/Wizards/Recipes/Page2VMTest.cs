﻿namespace Financier.Desktop.Tests.Wizards.Recipes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Financier.Desktop.Wizards;
    using Financier.Desktop.Wizards.RecipesWizard.ViewModel;
    using Financier.Tests.Common;
    using Xunit;

    public class Page2VMTest
    {
        [Theory]
        [AutoMoqData]
        public void Constructor_ReceiveParameters_AllPropertiesSeted(double totalAmount)
        {
            var vm = new Page2VM(totalAmount);

            Assert.Equal(totalAmount, vm.TotalAmount);
            Assert.Equal("Transactions", vm.Title);
            Assert.True(vm.IsValid());
        }

        [Fact]
        public void SetTransactions_ReceiveTransactions_TotalCalculated()
        {
            var vm = new Page2VM(-100.0);

            var transactions = new List<FinancierTransactionDto>
            {
            new () { FromAmount = -5000, Note = Guid.NewGuid().ToString(), Order = 1 },
            new () { FromAmount = -2500, Note = Guid.NewGuid().ToString(), Order = 2 },
            new () { FromAmount = -3000, Note = Guid.NewGuid().ToString(), Order = 3 },
            new () { FromAmount = 500, Note = Guid.NewGuid().ToString(), Order = 4 },
            };

            vm.SetTransactions(transactions);

            Assert.Equal(-100.0, vm.TotalAmount);
            Assert.Equal(-100.0, vm.CalculatedAmount);
            Assert.True(vm.FinancierTransactions.SequenceEqual(transactions));
        }

        [Fact]
        public void AddRow_Execute_CorrectOrderSeted()
        {
            var vm = new Page2VM(0);

            vm.AddRowCommand.Execute();

            Assert.Single(vm.FinancierTransactions);
            Assert.Equal(1, vm.FinancierTransactions[0].Order);
        }

        [Fact]
        public void Total_Execute_TotalCalculated()
        {
            var vm = new Page2VM(-100);

            Assert.Equal(0, vm.CalculatedAmount);
            vm.FinancierTransactions.Add(new () { FromAmount = -10000, Order = 1 });
            vm.TotalCommand.Execute();
            Assert.Equal(-100, vm.CalculatedAmount);
            Assert.Equal(0, vm.Diff);
        }

        [Fact]
        public void Delete_Execute_OrderUpdated()
        {
            var vm = new Page2VM(-100);

            Assert.Equal(0, vm.CalculatedAmount);
            FinancierTransactionDto toDelete = new () { FromAmount = -10000, Order = 2 };
            FinancierTransactionDto lastOne = new () { FromAmount = -10000, Order = 3 };

            vm.FinancierTransactions.Add(new () { FromAmount = -10000, Order = 1 });
            vm.FinancierTransactions.Add(toDelete);
            vm.FinancierTransactions.Add(lastOne);
            vm.DeleteRowCommand.Execute(toDelete);

            Assert.Equal(2, vm.FinancierTransactions.Last().Order);
        }

        [Theory]
        [AutoMoqData]
        public void ClearAllNotes_Execute_NotesEmpty(List<FinancierTransactionDto> transactions)
        {
            var vm = new Page2VM(-100);

            Assert.Equal(0, vm.CalculatedAmount);

            foreach (var item in transactions)
            {
                vm.FinancierTransactions.Add(item);
            }

            vm.ClearAllNotesCommand.Execute();

            foreach (var item in vm.FinancierTransactions)
            {
                Assert.Null(item.Note);
            }
        }
    }
}
