using Financier.DataAccess.Data;
using Financier.DataAccess.Monobank;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Financier.Desktop.MonoWizard.ViewModel
{
    public class Page3ViewModel : WizardBaseViewModel
    {
        private Account _monoAccount;
        private RangeObservableCollection<Account> accounts;
        private readonly List<Account> originalAccounts;
        private RangeObservableCollection<Currency> currencies;
        private RangeObservableCollection<Location> locations;
        private RangeObservableCollection<Category> categories;
        private RangeObservableCollection<FinancierTransactionViewModel> financierTransactions;
        private DelegateCommand<FinancierTransactionViewModel> _deleteCommand;

        public Page3ViewModel(List<Account> accounts, List<Currency> currencies, List<Location> locations, List<Category> categories)
        {
            this.accounts = new RangeObservableCollection<Account>(accounts);
            this.originalAccounts = new List<Account>(accounts);
            this.currencies = new RangeObservableCollection<Currency>(currencies);
            this.locations = new RangeObservableCollection<Location>(locations);
            this.categories = new RangeObservableCollection<Category>(categories);
            this.categories.Insert(0, Category.None);
        }

        public DelegateCommand<FinancierTransactionViewModel> DeleteCommand
        {
            get
            {
                return _deleteCommand ??= new DelegateCommand<FinancierTransactionViewModel>(tr => { financierTransactions.Remove(tr); });
            }
        }

        public Account MonoAccount
        {
            get => _monoAccount;
            set
            {
                _monoAccount = value;
                RaisePropertyChanged(nameof(MonoAccount));
                if (_monoAccount != null)
                {
                    Accounts = new RangeObservableCollection<Account>(
                        originalAccounts.Where(x => x.Id != _monoAccount.Id));
                }
            }
        }

        public override string Title => "Please select categories";

        public RangeObservableCollection<FinancierTransactionViewModel> FinancierTransactions
        {
            get => financierTransactions;
            set
            {
                financierTransactions = value;
                RaisePropertyChanged(nameof(FinancierTransactions));
            }
        }

        public RangeObservableCollection<Account> Accounts
        {
            get => accounts;
            set
            {
                accounts = value;
                RaisePropertyChanged(nameof(Accounts));
            }
        }

        public RangeObservableCollection<Currency> Currencies
        {
            get => currencies;
            set
            {
                currencies = value;
                RaisePropertyChanged(nameof(Currencies));
            }
        }

        public RangeObservableCollection<Category> Categories
        {
            get => categories;
            set
            {
                categories = value;
                RaisePropertyChanged(nameof(Categories));
            }
        }

        public RangeObservableCollection<Location> Locations
        {
            get => locations;
            set
            {
                locations = value;
                RaisePropertyChanged(nameof(Locations));
            }
        }

        public void SetMonoTransactions(List<MonoTransaction> transactions)
        {
            List<FinancierTransactionViewModel> transToAdd = new List<FinancierTransactionViewModel>();
            foreach (var x in transactions)
            {
                var locationId = locations.FirstOrDefault(l => (!string.IsNullOrEmpty(l.Title) && l.Title.Contains(x.Description, StringComparison.OrdinalIgnoreCase))
                                                            || (!string.IsNullOrEmpty(l.Address) && l.Address.Contains(x.Description, StringComparison.OrdinalIgnoreCase)))?.Id ??  0;
                var categoryId = categories.FirstOrDefault(l => l.Title.Contains(x.Description, StringComparison.OrdinalIgnoreCase))?.Id ?? 0;
                var newTr = new FinancierTransactionViewModel
                {
                    MonoAccountId = MonoAccount.Id,
                    FromAmount = Convert.ToInt64(x.CardCurrencyAmount * 100.0),
                    OriginalFromAmount = x.ExchangeRate == null ? null : Convert.ToInt64(x.OperationAmount * 100.0),
                    OriginalCurrencyId = x.ExchangeRate == null ? 0 : currencies.FirstOrDefault(c => c.Name == x.OperationCurrency)?.Id ?? 0,
                    CategoryId = categoryId,
                    ToAccountId = 0,
                    FromAccountId = 0,
                    LocationId = locationId,
                    Note = (locationId > 0 || categoryId > 0) ? null : x.Description,
                    DateTime = new DateTimeOffset(x.Date).ToUnixTimeMilliseconds()
                };
                transToAdd.Add(newTr);
            }

            FinancierTransactions = new RangeObservableCollection<FinancierTransactionViewModel>(transToAdd);
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
