using Financier.DataAccess.Data;
using Financier.DataAccess.Monobank;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Financier.Desktop.Wizards.MonoWizard.ViewModel
{
    public class Page3VM : WizardPageBaseVM
    {
        private Account _monoAccount;
        private ObservableCollection<Account> accounts;
        private readonly List<Account> originalAccounts;
        private ObservableCollection<Currency> currencies;
        private ObservableCollection<Location> locations;
        private ObservableCollection<Category> categories;
        private ObservableCollection<Project> projects;
        private ObservableCollection<FinancierTransactionDTO> financierTransactions;
        private DelegateCommand<FinancierTransactionDTO> _deleteCommand;

        public Page3VM(List<Account> accounts, List<Currency> currencies, List<Location> locations, List<Category> categories, List<Project> projects)
        {
            Accounts = new ObservableCollection<Account>(accounts.OrderByDescending(x => x.IsActive).ThenBy(x => x.SortOrder));
            originalAccounts = new List<Account>(accounts);
            Currencies = new ObservableCollection<Currency>(currencies);
            Locations = new ObservableCollection<Location>(locations.DefaultOrder());
            Categories = new ObservableCollection<Category>(categories);
            Projects = new ObservableCollection<Project>(projects.DefaultOrder());
            Categories.Insert(0, Category.None);
        }

        public DelegateCommand<FinancierTransactionDTO> DeleteCommand
        {
            get
            {
                return _deleteCommand ??= new DelegateCommand<FinancierTransactionDTO>(tr => { financierTransactions.Remove(tr); });
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
                    Accounts = new ObservableCollection<Account>(
                        originalAccounts.Where(x => x.Id != _monoAccount.Id).OrderByDescending(x => x.IsActive).ThenBy(x => x.SortOrder));
                }
            }
        }

        public override string Title => "Please select categories";

        public ObservableCollection<FinancierTransactionDTO> FinancierTransactions
        {
            get => financierTransactions;
            private set
            {
                financierTransactions = value;
                RaisePropertyChanged(nameof(FinancierTransactions));
            }
        }

        public ObservableCollection<Account> Accounts
        {
            get => accounts;
            private set
            {
                accounts = value;
                RaisePropertyChanged(nameof(Accounts));
            }
        }

        public ObservableCollection<Currency> Currencies
        {
            get => currencies;
            private set
            {
                currencies = value;
                RaisePropertyChanged(nameof(Currencies));
            }
        }

        public ObservableCollection<Category> Categories
        {
            get => categories;
            private set
            {
                categories = value;
                RaisePropertyChanged(nameof(Categories));
            }
        }

        public ObservableCollection<Location> Locations
        {
            get => locations;
            private set
            {
                locations = value;
                RaisePropertyChanged(nameof(Locations));
            }
        }

        public ObservableCollection<Project> Projects
        {
            get => projects;
            private set
            {
                projects = value;
                RaisePropertyChanged(nameof(Projects));
            }
        }

        public void SetMonoTransactions(List<MonoTransaction> transactions)
        {
            List<FinancierTransactionDTO> transToAdd = new List<FinancierTransactionDTO>();
            foreach (var x in transactions)
            {
                var locationId = locations.FirstOrDefault(l => !string.IsNullOrEmpty(l.Title) && l.Title.Contains(x.Description, StringComparison.OrdinalIgnoreCase)
                                                            || !string.IsNullOrEmpty(l.Address) && l.Address.Contains(x.Description, StringComparison.OrdinalIgnoreCase))?.Id ?? 0;
                var categoryId = categories.FirstOrDefault(l => l.Title.Contains(x.Description, StringComparison.OrdinalIgnoreCase))?.Id ?? 0;
                var newTr = new FinancierTransactionDTO
                {
                    MonoAccountId = MonoAccount.Id,
                    FromAmount = Convert.ToInt64(x.CardCurrencyAmount * 100.0),
                    OriginalFromAmount = x.ExchangeRate == null ? null : Convert.ToInt64(x.OperationAmount * 100.0),
                    OriginalCurrencyId = x.ExchangeRate == null ? 0 : currencies.FirstOrDefault(c => c.Name == x.OperationCurrency)?.Id ?? 0,
                    CategoryId = categoryId,
                    ToAccountId = 0,
                    FromAccountId = 0,
                    LocationId = locationId,
                    Note = locationId > 0 || categoryId > 0 ? null : x.Description,
                    DateTime = new DateTimeOffset(x.Date).ToUnixTimeMilliseconds()
                };
                transToAdd.Add(newTr);
            }

            FinancierTransactions = new ObservableCollection<FinancierTransactionDTO>(transToAdd);
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
