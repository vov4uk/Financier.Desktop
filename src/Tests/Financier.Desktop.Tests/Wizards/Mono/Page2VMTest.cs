namespace Financier.Desktop.Tests.Wizards.Mono
{
    using System;
    using System.Collections.Generic;
    using Financier.DataAccess.Data;
    using Financier.DataAccess.Monobank;
    using Financier.Desktop.Wizards.MonoWizard.ViewModel;
    using Financier.Tests.Common;
    using Xunit;

    public class Page2VMTest
    {
        [Theory]
        [AutoMoqData]
        public void Constructor_ReceiveTransactions_AllTransactionsSeted(List<MonoTransaction> transactions)
        {
            var vm = new Page2VM(transactions);

            Assert.Equal(transactions, vm.AllTransactions);
            Assert.Equal("Please select transaction", vm.Title);
            Assert.True(vm.IsValid());
        }

        [Fact]
        public void MonoAccount_BalanceMatch_StartTransactionSetted()
        {
            var account = new Account() { TotalAmount = 10000 };
            var startTr = new MonoTransaction() { Balance = 100.0 };
            var transactions = new List<MonoTransaction>()
            {
                new MonoTransaction() { Balance = 99.0 },
                startTr,
            };

            var vm = new Page2VM(transactions);
            vm.MonoAccount = account;

            Assert.Equal(startTr, vm.StartTransaction);
        }

        [Fact]
        public void MonoAccount_BalanceNotMatch_StartTransactionNotSetted()
        {
            var account = new Account() { TotalAmount = 10001 };
            var transactions = new List<MonoTransaction>()
            {
                new MonoTransaction() { Balance = 99.0 },
                new MonoTransaction() { Balance = 100.0 },
            };

            var vm = new Page2VM(transactions);
            vm.MonoAccount = account;

            Assert.Equal(default, vm.StartTransaction);
        }

        [Fact]
        public void MonoTransactions_StartTransactionNotSet_MonoTransactionsReturnNotAllTansactions()
        {
            var transactions = new List<MonoTransaction>()
            {
                new MonoTransaction() { Balance = 99.0, Date = new DateTime(2017, 11, 18) },
                new MonoTransaction() { Balance = 100.0, Date = new DateTime(2017, 11, 16) },
            };

            var vm = new Page2VM(transactions);

            Assert.Single(vm.MonoTransactions);
        }

        [Fact]
        public void MonoTransactions_StartTransactionSeted_MonoTransactionsReturn3Trsnsactions()
        {
            var startTr = new MonoTransaction() { Balance = 100.0, Date = new DateTime(2019, 11, 17) };
            var transactions = new List<MonoTransaction>()
            {
                new MonoTransaction() { Balance = 199.0, Date = new DateTime(2019, 11, 18) },
                new MonoTransaction() { Balance = 102.0, Date = new DateTime(2019, 11, 18) },
                new MonoTransaction() { Balance = 101.0, Date = new DateTime(2019, 11, 18) },
                startTr,
                new MonoTransaction() { Balance = 102.0, Date = new DateTime(2019, 11, 16) },
                new MonoTransaction() { Balance = 103.0, Date = new DateTime(2019, 11, 16) },
                new MonoTransaction() { Balance = 104.0, Date = new DateTime(2019, 11, 16) },
            };

            var vm = new Page2VM(transactions);
            vm.StartTransaction = startTr;

            Assert.Equal(3, vm.MonoTransactions.Count);
        }

        [Theory]
        [AutoMoqData]
        public void DeleteCommand_Execute_TransactionsRemoved(List<MonoTransaction> transactions)
        {
            var trToremove = new MonoTransaction();
            transactions.Add(trToremove);
            var vm = new Page2VM(transactions);
            vm.DeleteCommand.Execute(trToremove);

            Assert.DoesNotContain(trToremove, vm.AllTransactions);
        }
    }
}
