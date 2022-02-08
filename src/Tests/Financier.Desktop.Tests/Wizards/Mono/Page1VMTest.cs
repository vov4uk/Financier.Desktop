namespace Financier.Desktop.Tests.Wizards.Mono
{
    using System;
    using System.Collections.Generic;
    using Financier.Common.Entities;
    using Financier.Common.Model;
    using Financier.Desktop.Wizards.MonoWizard.ViewModel;
    using Financier.Tests.Common;
    using Xunit;

    public class Page1VMTest
    {
        [Theory]
        [AutoMoqData]
        public void Constructor_HasMonoAccount_SelectedMonoAccount(List<AccountFilterModel> accounts)
        {
            var monoAcc = new AccountFilterModel { Title = "Monobank", Is_Active = 1 };
            accounts.Add(monoAcc);
            DbManual.SetupTests(accounts);
            var vm = new Page1VM();

            Assert.Equal(monoAcc, vm.MonoAccount);
            Assert.Equal("Please select account", vm.Title);
            Assert.True(vm.IsValid());
            DbManual.ResetAllManuals();
        }

        [Theory]
        [AutoMoqData]
        public void Constructor_MonoAccountInactive_MonoAccountNotSelected(List<AccountFilterModel> accounts)
        {
            var monoAcc = new AccountFilterModel { Title = "Monobank", Is_Active = 0 };
            accounts.Add(monoAcc);
            DbManual.SetupTests(accounts);
            var vm = new Page1VM();

            Assert.Equal(accounts[0], vm.MonoAccount);
            DbManual.ResetAllManuals();
        }

        [Theory]
        [AutoMoqData]
        public void Constructor_NoMono_FirstAccountSelected(List<AccountFilterModel> accounts)
        {
            foreach (var item in accounts)
            {
                item.Title = Guid.NewGuid().ToString();
            }
            DbManual.SetupTests(accounts);
            var vm = new Page1VM();

            Assert.NotNull(vm.MonoAccount);
            DbManual.ResetAllManuals();
        }
    }
}
