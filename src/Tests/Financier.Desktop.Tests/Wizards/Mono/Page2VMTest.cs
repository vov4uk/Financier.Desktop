namespace Financier.Desktop.Tests.Wizards.Mono
{
    using System;
    using System.Collections.Generic;
    using Financier.Common.Model;
    using Financier.Desktop.Wizards;
    using Financier.Desktop.Wizards.MonoWizard.ViewModel;
    using Financier.Tests.Common;
    using Xunit;

    public class Page2VMTest
    {
        [Theory]
        [AutoMoqData]
        public void Constructor_ReceiveTransactions_AllTransactionsSeted(List<BankTransaction> transactions, Dictionary<int, BlotterModel> lastTransactions)
        {
            var vm = new Page2VM(transactions, lastTransactions);

            Assert.Equal(transactions, vm.AllTransactions);
            Assert.Equal("Please select transaction", vm.Title);
            Assert.True(vm.IsValid());
        }

        [Fact]
        public void MonoAccount_BalanceMatch_StartTransactionSetted()
        {
            var account = new AccountFilterModel() { TotalAmount = 10000 };
            var startTr = new BankTransaction() { Balance = 100.0 };
            var transactions = new List<BankTransaction>()
            {
                new BankTransaction() { Balance = 99.0 },
                startTr,
            };

            var vm = new Page2VM(transactions, new Dictionary<int, BlotterModel>());
            vm.MonoAccount = account;

            Assert.Equal(startTr, vm.StartTransaction);
        }

        [Fact]
        public void MonoAccount_BalanceNotMatch_StartTransactionNotSetted()
        {
            var account = new AccountFilterModel() { TotalAmount = 10001 };
            var transactions = new List<BankTransaction>()
            {
                new BankTransaction() { Balance = 99.0 },
                new BankTransaction() { Balance = 100.0 },
            };

            var vm = new Page2VM(transactions, new Dictionary<int, BlotterModel>());
            vm.MonoAccount = account;

            Assert.Equal(default, vm.StartTransaction);
        }

        [Fact]
        public void MonoTransactions_StartTransactionNotSet_MonoTransactionsReturnNotAllTansactions()
        {
            var transactions = new List<BankTransaction>()
            {
                new BankTransaction() { Balance = 99.0, Date = new DateTime(2017, 11, 18) },
                new BankTransaction() { Balance = 100.0, Date = new DateTime(2017, 11, 16) },
            };

            var vm = new Page2VM(transactions, new Dictionary<int, BlotterModel>());

            Assert.Single(vm.GetMonoTransactions());
        }

        [Fact]
        public void MonoTransactions_StartTransactionSeted_MonoTransactionsReturn3Trsnsactions()
        {
            var startTr = new BankTransaction() { Balance = 100.0, Date = new DateTime(2019, 11, 17) };
            var transactions = new List<BankTransaction>()
            {
                new BankTransaction() { Balance = 199.0, Date = new DateTime(2019, 11, 18) },
                new BankTransaction() { Balance = 102.0, Date = new DateTime(2019, 11, 18) },
                new BankTransaction() { Balance = 101.0, Date = new DateTime(2019, 11, 18) },
                startTr,
                new BankTransaction() { Balance = 102.0, Date = new DateTime(2019, 11, 16) },
                new BankTransaction() { Balance = 103.0, Date = new DateTime(2019, 11, 16) },
                new BankTransaction() { Balance = 104.0, Date = new DateTime(2019, 11, 16) },
            };

            var vm = new Page2VM(transactions, new Dictionary<int, BlotterModel>());
            vm.StartTransaction = startTr;

            Assert.Equal(3, vm.GetMonoTransactions().Count);
        }

        [Theory]
        [AutoMoqData]
        public void DeleteCommand_Execute_TransactionsRemoved(List<BankTransaction> transactions, Dictionary<int, BlotterModel> lastTransactions)
        {
            var trToremove = new BankTransaction();
            transactions.Add(trToremove);
            var vm = new Page2VM(transactions, lastTransactions);
            vm.DeleteCommand.Execute(trToremove);

            Assert.DoesNotContain(trToremove, vm.AllTransactions);
        }
    }
}
