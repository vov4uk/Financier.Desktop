namespace Financier.Desktop.Tests.Pages.Dialog
{
    using System.Collections.Generic;
    using Financier.Common.Entities;
    using Financier.Common.Model;
    using Financier.Desktop.Data;
    using Financier.Desktop.ViewModel.Dialog;
    using Xunit;

    public class AccountControlVMTest
    {
        // ── helpers ──────────────────────────────────────────────────────────

        private static AccountDto CreateEntity(
            string type = "CASH",
            int currencyId = 0,
            string cardIssuer = null,
            string title = null) =>
            new AccountDto
            {
                Type = type,
                CurrencyId = currencyId,
                CardIssuer = cardIssuer,
                Title = title,
            };

        private static CurrencyModel MakeCurrency(int id, string name = "USD") =>
            new CurrencyModel { Id = id, Name = name };

        // ── Constructor ──────────────────────────────────────────────────────

        [Fact]
        public void Constructor_SetsEntityProperty()
        {
            var entity = CreateEntity();
            var vm = new AccountControlVM(entity, isNew: true);
            Assert.Same(entity, vm.Entity);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Constructor_SetsIsNew(bool isNew)
        {
            var vm = new AccountControlVM(CreateEntity(), isNew);
            Assert.Equal(isNew, vm.IsNew);
        }

        [Fact]
        public void Constructor_SetsCurrencies_FromDbManual()
        {
            DbManual.SetupTests(new List<CurrencyModel>());
            var vm = new AccountControlVM(CreateEntity(), isNew: true);
            Assert.Same(DbManual.Currencies, vm.Currencies);
        }

        [Theory]
        [InlineData("CASH", AccountType.CASH)]
        [InlineData("BANK", AccountType.BANK)]
        [InlineData("CREDIT_CARD", AccountType.CREDIT_CARD)]
        [InlineData("DEBIT_CARD", AccountType.DEBIT_CARD)]
        [InlineData("ASSET", AccountType.ASSET)]
        [InlineData("ELECTRONIC", AccountType.ELECTRONIC)]
        public void Constructor_ValidType_SetsSelectedAccountType(string type, AccountType expected)
        {
            var vm = new AccountControlVM(CreateEntity(type: type), isNew: true);
            Assert.Equal(expected, vm.SelectedAccountType);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("NOT_A_TYPE")]
        public void Constructor_InvalidType_DefaultsSelectedAccountTypeToCash(string type)
        {
            var vm = new AccountControlVM(CreateEntity(type: type), isNew: true);
            Assert.Equal(AccountType.CASH, vm.SelectedAccountType);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("NOT_A_TYPE")]
        public void Constructor_InvalidType_NormalisesEntityTypeToCash(string type)
        {
            var entity = CreateEntity(type: type);
            _ = new AccountControlVM(entity, isNew: true);
            Assert.Equal(AccountType.CASH.ToString(), entity.Type);
        }

        [Fact]
        public void Constructor_WithCardIssuer_SetsSelectedCardIssuer()
        {
            var vm = new AccountControlVM(CreateEntity(cardIssuer: "VISA"), isNew: true);
            Assert.Equal(CardIssuer.VISA, vm.SelectedCardIssuer);
        }

        [Fact]
        public void Constructor_WithElectronicType_SetsSelectedElectronicType()
        {
            var vm = new AccountControlVM(CreateEntity(cardIssuer: "PAYPAL"), isNew: true);
            Assert.Equal(ElectronicType.PAYPAL, vm.SelectedElectronicType);
        }

        [Fact]
        public void Constructor_WithCurrencyId_SetsSelectedCurrency()
        {
            var currency = MakeCurrency(id: 3);
            DbManual.SetupTests(new List<CurrencyModel> { currency });

            var vm = new AccountControlVM(CreateEntity(currencyId: 3), isNew: true);

            Assert.Same(currency, vm.SelectedCurrency);
        }

        [Fact]
        public void Constructor_WithCurrencyIdNotInList_LeavesSelectedCurrencyNull()
        {
            DbManual.SetupTests(new List<CurrencyModel> { MakeCurrency(id: 1) });

            var vm = new AccountControlVM(CreateEntity(currencyId: 99), isNew: true);

            Assert.Null(vm.SelectedCurrency);
        }

        [Fact]
        public void Constructor_WithZeroCurrencyId_LeavesSelectedCurrencyNull()
        {
            DbManual.SetupTests(new List<CurrencyModel>());

            var vm = new AccountControlVM(CreateEntity(currencyId: 0), isNew: true);

            Assert.Null(vm.SelectedCurrency);
        }

        // ── ClearTitleCommand ────────────────────────────────────────────────

        [Fact]
        public void ClearTitleCommand_Execute_SetsEntityTitleToNull()
        {
            var entity = CreateEntity(title: "My Account");
            var vm = new AccountControlVM(entity, isNew: true);

            vm.ClearTitleCommand.Execute();

            Assert.Null(entity.Title);
        }

        [Fact]
        public void ClearTitleCommand_Execute_DisablesSaveCommand()
        {
            var entity = CreateEntity(currencyId: 1, title: "My Account");
            var vm = new AccountControlVM(entity, isNew: true);

            vm.ClearTitleCommand.Execute();

            Assert.False(vm.SaveCommand.CanExecute());
        }

        // ── SelectedAccountType setter ────────────────────────────────────────

        [Fact]
        public void SelectedAccountType_WhenChanged_SetsEntityType()
        {
            var entity = CreateEntity();
            var vm = new AccountControlVM(entity, isNew: true);

            vm.SelectedAccountType = AccountType.BANK;

            Assert.Equal(AccountType.BANK.ToString(), entity.Type);
        }

        [Fact]
        public void SelectedAccountType_WhenChanged_RaisesPropertyChangedForAllRelatedProperties()
        {
            var vm = new AccountControlVM(CreateEntity(), isNew: true);
            var raised = new List<string>();
            vm.PropertyChanged += (_, e) => raised.Add(e.PropertyName);

            vm.SelectedAccountType = AccountType.CREDIT_CARD;

            Assert.Contains(nameof(vm.SelectedAccountType), raised);
            Assert.Contains(nameof(vm.ShowCardIssuer), raised);
            Assert.Contains(nameof(vm.ShowElectronicType), raised);
            Assert.Contains(nameof(vm.ShowIssuer), raised);
            Assert.Contains(nameof(vm.ShowNumber), raised);
            Assert.Contains(nameof(vm.ShowCreditCardFields), raised);
        }

        // ── SelectedCardIssuer setter ─────────────────────────────────────────

        [Fact]
        public void SelectedCardIssuer_WhenChanged_SetsEntityCardIssuer()
        {
            var entity = CreateEntity();
            var vm = new AccountControlVM(entity, isNew: true);

            vm.SelectedCardIssuer = CardIssuer.MASTERCARD;

            Assert.Equal(CardIssuer.MASTERCARD.ToString(), entity.CardIssuer);
        }

        [Fact]
        public void SelectedCardIssuer_WhenChanged_RaisesPropertyChanged()
        {
            var vm = new AccountControlVM(CreateEntity(), isNew: true);
            var raised = new List<string>();
            vm.PropertyChanged += (_, e) => raised.Add(e.PropertyName);

            vm.SelectedCardIssuer = CardIssuer.AMEX;

            Assert.Contains(nameof(vm.SelectedCardIssuer), raised);
        }

        // ── SelectedElectronicType setter ─────────────────────────────────────

        [Fact]
        public void SelectedElectronicType_WhenChanged_SetsEntityCardIssuer()
        {
            var entity = CreateEntity();
            var vm = new AccountControlVM(entity, isNew: true);

            vm.SelectedElectronicType = ElectronicType.PAYPAL;

            Assert.Equal(ElectronicType.PAYPAL.ToString(), entity.CardIssuer);
        }

        [Fact]
        public void SelectedElectronicType_WhenChanged_RaisesPropertyChanged()
        {
            var vm = new AccountControlVM(CreateEntity(), isNew: true);
            var raised = new List<string>();
            vm.PropertyChanged += (_, e) => raised.Add(e.PropertyName);

            vm.SelectedElectronicType = ElectronicType.BITCOIN;

            Assert.Contains(nameof(vm.SelectedElectronicType), raised);
        }

        // ── SelectedCurrency setter ───────────────────────────────────────────

        [Fact]
        public void SelectedCurrency_WithValidId_SetsEntityCurrencyId()
        {
            var entity = CreateEntity();
            var vm = new AccountControlVM(entity, isNew: true);

            vm.SelectedCurrency = MakeCurrency(id: 7);

            Assert.Equal(7, entity.CurrencyId);
        }

        [Fact]
        public void SelectedCurrency_WhenNullValue_DoesNotUpdateEntityCurrencyId()
        {
            var entity = CreateEntity(currencyId: 5);
            var vm = new AccountControlVM(entity, isNew: true);

            vm.SelectedCurrency = null;

            Assert.Equal(5, entity.CurrencyId);
        }

        [Fact]
        public void SelectedCurrency_WhenCurrencyHasNullId_DoesNotUpdateEntityCurrencyId()
        {
            var entity = CreateEntity(currencyId: 5);
            var vm = new AccountControlVM(entity, isNew: true);

            vm.SelectedCurrency = new CurrencyModel { Id = null };

            Assert.Equal(5, entity.CurrencyId);
        }

        [Fact]
        public void SelectedCurrency_WhenChanged_RaisesPropertyChanged()
        {
            var vm = new AccountControlVM(CreateEntity(), isNew: true);
            var raised = new List<string>();
            vm.PropertyChanged += (_, e) => raised.Add(e.PropertyName);

            vm.SelectedCurrency = MakeCurrency(id: 1);

            Assert.Contains(nameof(vm.SelectedCurrency), raised);
        }

        [Fact]
        public void SelectedCurrency_WhenChanged_RaisesCanExecuteChanged()
        {
            var vm = new AccountControlVM(CreateEntity(), isNew: true);
            bool canExecuteChangedRaised = false;
            vm.SaveCommand.CanExecuteChanged += (_, _) => canExecuteChangedRaised = true;

            vm.SelectedCurrency = MakeCurrency(id: 1);

            Assert.True(canExecuteChangedRaised);
        }

        // ── Show* computed properties ─────────────────────────────────────────

        [Theory]
        [InlineData(AccountType.DEBIT_CARD, true)]
        [InlineData(AccountType.CREDIT_CARD, true)]
        [InlineData(AccountType.CASH, false)]
        [InlineData(AccountType.BANK, false)]
        [InlineData(AccountType.ELECTRONIC, false)]
        [InlineData(AccountType.ASSET, false)]
        public void ShowCardIssuer_ReturnsExpectedValue(AccountType accountType, bool expected)
        {
            var vm = new AccountControlVM(CreateEntity(), isNew: true);
            vm.SelectedAccountType = accountType;
            Assert.Equal(expected, vm.ShowCardIssuer);
        }

        [Theory]
        [InlineData(AccountType.ELECTRONIC, true)]
        [InlineData(AccountType.CASH, false)]
        [InlineData(AccountType.BANK, false)]
        [InlineData(AccountType.DEBIT_CARD, false)]
        [InlineData(AccountType.CREDIT_CARD, false)]
        public void ShowElectronicType_ReturnsExpectedValue(AccountType accountType, bool expected)
        {
            var vm = new AccountControlVM(CreateEntity(), isNew: true);
            vm.SelectedAccountType = accountType;
            Assert.Equal(expected, vm.ShowElectronicType);
        }

        [Theory]
        [InlineData(AccountType.DEBIT_CARD, true)]
        [InlineData(AccountType.CREDIT_CARD, true)]
        [InlineData(AccountType.ELECTRONIC, true)]
        [InlineData(AccountType.CASH, false)]
        [InlineData(AccountType.BANK, false)]
        [InlineData(AccountType.ASSET, false)]
        public void ShowIssuer_ReturnsExpectedValue(AccountType accountType, bool expected)
        {
            var vm = new AccountControlVM(CreateEntity(), isNew: true);
            vm.SelectedAccountType = accountType;
            Assert.Equal(expected, vm.ShowIssuer);
        }

        [Theory]
        [InlineData(AccountType.DEBIT_CARD, true)]
        [InlineData(AccountType.CREDIT_CARD, true)]
        [InlineData(AccountType.CASH, false)]
        [InlineData(AccountType.BANK, false)]
        [InlineData(AccountType.ELECTRONIC, false)]
        public void ShowNumber_ReturnsExpectedValue(AccountType accountType, bool expected)
        {
            var vm = new AccountControlVM(CreateEntity(), isNew: true);
            vm.SelectedAccountType = accountType;
            Assert.Equal(expected, vm.ShowNumber);
        }

        [Theory]
        [InlineData(AccountType.CREDIT_CARD, true)]
        [InlineData(AccountType.DEBIT_CARD, false)]
        [InlineData(AccountType.CASH, false)]
        [InlineData(AccountType.ELECTRONIC, false)]
        [InlineData(AccountType.BANK, false)]
        public void ShowCreditCardFields_ReturnsExpectedValue(AccountType accountType, bool expected)
        {
            var vm = new AccountControlVM(CreateEntity(), isNew: true);
            vm.SelectedAccountType = accountType;
            Assert.Equal(expected, vm.ShowCreditCardFields);
        }

        // ── OnRequestSave ─────────────────────────────────────────────────────

        [Fact]
        public void OnRequestSave_ReturnsEntity()
        {
            var entity = CreateEntity();
            var vm = new AccountControlVM(entity, isNew: true);
            Assert.Same(entity, vm.OnRequestSave());
        }

        // ── CanSaveCommandExecute ─────────────────────────────────────────────

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void SaveCommand_WhenTitleIsNullOrWhitespace_CannotExecute(string title)
        {
            var entity = CreateEntity(currencyId: 1, title: title);
            var vm = new AccountControlVM(entity, isNew: true);
            Assert.False(vm.SaveCommand.CanExecute());
        }

        [Fact]
        public void SaveCommand_WhenCurrencyIdIsZero_CannotExecute()
        {
            var entity = CreateEntity(currencyId: 0, title: "My Account");
            var vm = new AccountControlVM(entity, isNew: true);
            Assert.False(vm.SaveCommand.CanExecute());
        }

        [Fact]
        public void SaveCommand_WhenTitleAndCurrencyIdAreValid_CanExecute()
        {
            var entity = CreateEntity(currencyId: 1, title: "My Account");
            var vm = new AccountControlVM(entity, isNew: true);
            Assert.True(vm.SaveCommand.CanExecute());
        }

        // ── entity.PropertyChanged subscription ───────────────────────────────

        [Fact]
        public void EntityTitleChanged_RaisesCanExecuteChanged()
        {
            var entity = CreateEntity();
            var vm = new AccountControlVM(entity, isNew: true);
            bool raised = false;
            vm.SaveCommand.CanExecuteChanged += (_, _) => raised = true;

            entity.Title = "Updated";

            Assert.True(raised);
        }

        [Fact]
        public void EntityCurrencyIdChanged_RaisesCanExecuteChanged()
        {
            var entity = CreateEntity();
            var vm = new AccountControlVM(entity, isNew: true);
            bool raised = false;
            vm.SaveCommand.CanExecuteChanged += (_, _) => raised = true;

            entity.CurrencyId = 5;

            Assert.True(raised);
        }
    }
}
