using System.Collections.Generic;
using Financier.Common.Entities;
using Financier.Common.Localization;
using Financier.Common.Model;
using Financier.Desktop.Data;
using Prism.Commands;

namespace Financier.Desktop.ViewModel.Dialog
{
    public class AccountControlVM : DialogBaseVM
    {
        private AccountTypeItem selectedAccountType;
        private AccountTypeItem selectedCardIssuer;
        private AccountTypeItem selectedElectronicType;
        private CurrencyModel selectedCurrency;
        private DelegateCommand _clearTitleCommand;

        public AccountControlVM(AccountDto entity, bool isNew)
        {
            Entity = entity;
            IsNew = isNew;

            AccountTypes = new List<AccountTypeItem>
            {
                new("CASH",        LocalizationService.Instance["account_type_cash"]),
                new("BANK",        LocalizationService.Instance["account_type_bank"]),
                new("CREDIT_CARD", LocalizationService.Instance["account_type_credit_card"]),
                new("DEBIT_CARD",  LocalizationService.Instance["account_type_debit_card"]),
                new("ASSET",       LocalizationService.Instance["account_type_asset"]),
                new("LIABILITY",   LocalizationService.Instance["account_type_liability"]),
                new("ELECTRONIC",  LocalizationService.Instance["account_type_electronic"]),
                new("OTHER",       LocalizationService.Instance["account_type_other"]),
            };

            CardIssuers = new List<AccountTypeItem>
            {
                new("VISA",          "Visa"),
                new("ELECTRON",      "Visa Electron"),
                new("MASTERCARD",    "Mastercard"),
                new("MAESTRO",       "Maestro"),
                new("CIRRUS",        "Cirrus"),
                new("AMEX",          "AMEX"),
                new("JCB",           "JCB"),
                new("DINERS",        "Diners Club"),
                new("DISCOVER",      "Discover"),
                new("UNIONPAY",      "UnionPay"),
                new("EPS",           "EPS"),
                new("NETS",          "NETS"),
                new("RUPAY",         "RuPay"),
                new("MIR",           "Mir"),
                new("DEFAULT",       "Default"),
            };

            ElectronicTypes = new List<AccountTypeItem>
            {
                new("PAYPAL",        "PayPal"),
                new("BITCOIN",       "Bitcoin"),
                new("AMAZON",        "Amazon"),
                new("EBAY",          "Ebay"),
                new("GOOGLE_WALLET", "Google Wallet"),
                new("WEB_MONEY",     "Web Money"),
                new("YANDEX_MONEY",  "Yandex Money"),
                new("ALIPAY",        "AliPay"),
            };

            Currencies = DbManual.Currencies;

            InitSelections();

            entity.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName is nameof(AccountDto.Title) or nameof(AccountDto.CurrencyId))
                    SaveCommand.RaiseCanExecuteChanged();
            };
        }

        public record AccountTypeItem(string Value, string Display);

        public DelegateCommand ClearTitleCommand =>
            _clearTitleCommand ??= new DelegateCommand(() => { Entity.Title = default!; SaveCommand.RaiseCanExecuteChanged(); });

        public AccountDto Entity { get; }

        public bool IsNew { get; }

        public List<AccountTypeItem> AccountTypes { get; }

        public List<AccountTypeItem> CardIssuers { get; }

        public List<AccountTypeItem> ElectronicTypes { get; }

        public List<CurrencyModel> Currencies { get; }

        public AccountTypeItem SelectedAccountType
        {
            get => selectedAccountType;
            set
            {
                selectedAccountType = value;
                if (value != null)
                    Entity.Type = value.Value;
                RaisePropertyChanged(nameof(SelectedAccountType));
                RaisePropertyChanged(nameof(ShowCardIssuer));
                RaisePropertyChanged(nameof(ShowElectronicType));
                RaisePropertyChanged(nameof(ShowIssuer));
                RaisePropertyChanged(nameof(ShowNumber));
                RaisePropertyChanged(nameof(ShowCreditCardFields));
            }
        }

        public AccountTypeItem SelectedCardIssuer
        {
            get => selectedCardIssuer;
            set
            {
                selectedCardIssuer = value;
                if (value != null)
                    Entity.CardIssuer = value.Value;
                RaisePropertyChanged(nameof(SelectedCardIssuer));
            }
        }

        public AccountTypeItem SelectedElectronicType
        {
            get => selectedElectronicType;
            set
            {
                selectedElectronicType = value;
                if (value != null)
                    Entity.CardIssuer = value.Value;
                RaisePropertyChanged(nameof(SelectedElectronicType));
            }
        }

        public CurrencyModel SelectedCurrency
        {
            get => selectedCurrency;
            set
            {
                selectedCurrency = value;
                if (value?.Id.HasValue == true)
                    Entity.CurrencyId = value.Id.Value;
                RaisePropertyChanged(nameof(SelectedCurrency));
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        public bool ShowCardIssuer =>
            Entity.Type == "DEBIT_CARD" || Entity.Type == "CREDIT_CARD";

        public bool ShowElectronicType =>
            Entity.Type == "ELECTRONIC";

        public bool ShowIssuer =>
            Entity.Type is "DEBIT_CARD" or "CREDIT_CARD" or "ELECTRONIC";

        public bool ShowNumber =>
            Entity.Type == "DEBIT_CARD" || Entity.Type == "CREDIT_CARD";

        public bool ShowCreditCardFields =>
            Entity.Type == "CREDIT_CARD";

        public override object OnRequestSave() => Entity;

        protected override bool CanSaveCommandExecute() =>
            !string.IsNullOrWhiteSpace(Entity?.Title) && Entity?.CurrencyId > 0;

        private void InitSelections()
        {
            selectedAccountType = AccountTypes.Find(x => x.Value == Entity.Type)
                                  ?? AccountTypes[0];
            Entity.Type = selectedAccountType.Value;

            if (!string.IsNullOrEmpty(Entity.CardIssuer))
            {
                selectedCardIssuer = CardIssuers.Find(x => x.Value == Entity.CardIssuer);
                selectedElectronicType = ElectronicTypes.Find(x => x.Value == Entity.CardIssuer);
            }

            selectedCardIssuer ??= CardIssuers[0];
            selectedElectronicType ??= ElectronicTypes[0];

            if (Entity.CurrencyId > 0)
                selectedCurrency = Currencies.Find(x => x.Id == Entity.CurrencyId);
        }
    }
}
