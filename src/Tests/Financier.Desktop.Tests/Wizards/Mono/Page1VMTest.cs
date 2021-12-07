namespace Financier.Desktop.Tests.Wizards.Mono
{
    using System;
    using System.Collections.Generic;
    using Financier.DataAccess.Data;
    using Financier.Desktop.Wizards.MonoWizard.ViewModel;
    using Financier.Tests.Common;
    using Xunit;

    public class Page1VMTest
    {
        [Theory]
        [AutoMoqData]
        public void Constructor_HasMonoAccount_SelectedMonoAccount(List<Account> accounts)
        {
            var monoAcc = new Account { Title = "Monobank", IsActive = true };
            accounts.Add(monoAcc);
            var vm = new Page1VM(accounts);

            Assert.Equal(monoAcc, vm.MonoAccount);
            Assert.Equal("Please select account", vm.Title);
            Assert.True(vm.IsValid());
            Assert.Equal(accounts.Count, vm.Accounts.Count);
        }

        [Theory]
        [AutoMoqData]
        public void Constructor_MonoAccountInactive_MonoAccountNotSelected(List<Account> accounts)
        {
            var monoAcc = new Account { Title = "Monobank", IsActive = false };
            accounts.Add(monoAcc);
            var vm = new Page1VM(accounts);

            Assert.Equal(default, vm.MonoAccount);
        }

        [Theory]
        [AutoMoqData]
        public void Constructor_NoMono_MonoAccountNotSelected(List<Account> accounts)
        {
            foreach (var item in accounts)
            {
                item.Title = Guid.NewGuid().ToString();
            }

            var vm = new Page1VM(accounts);

            Assert.Equal(default, vm.MonoAccount);
        }

        [Fact]
        public void Constructor_NullAccounts_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => new Page1VM(null));
        }
    }
}
