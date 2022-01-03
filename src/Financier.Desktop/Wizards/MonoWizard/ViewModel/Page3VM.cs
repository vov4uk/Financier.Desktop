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
        private readonly List<Account> originalAccounts;
        private DelegateCommand<FinancierTransactionDTO> _deleteCommand;
        private Account _monoAccount;
        private List<Account> accounts;
        private List<Category> categories;
        private List<Currency> currencies;
        private ObservableCollection<FinancierTransactionDTO> financierTransactions;
        private List<Location> locations;
        private List<Project> projects;
        public Page3VM(List<Account> accounts, List<Currency> currencies, List<Location> locations, List<Category> categories, List<Project> projects)
        {
            Accounts = accounts.OrderByDescending(x => x.IsActive).ThenBy(x => x.SortOrder).ToList();
            originalAccounts = new List<Account>(accounts);
            Currencies = currencies;
            Locations = locations.DefaultOrder().ToList();
            Categories = categories;
            Projects = projects.DefaultOrder().ToList();
            Categories.Insert(0, Category.None);
        }

        public List<Account> Accounts
        {
            get => accounts;
            private set
            {
                accounts = value;
                RaisePropertyChanged(nameof(Accounts));
            }
        }

        public List<Category> Categories
        {
            get => categories;
            private set
            {
                categories = value;
                RaisePropertyChanged(nameof(Categories));
            }
        }

        public List<Currency> Currencies
        {
            get => currencies;
            private set
            {
                currencies = value;
                RaisePropertyChanged(nameof(Currencies));
            }
        }

        public DelegateCommand<FinancierTransactionDTO> DeleteCommand
        {
            get
            {
                return _deleteCommand ??= new DelegateCommand<FinancierTransactionDTO>(tr => { financierTransactions.Remove(tr); });
            }
        }

        public ObservableCollection<FinancierTransactionDTO> FinancierTransactions
        {
            get => financierTransactions;
            private set
            {
                financierTransactions = value;
                RaisePropertyChanged(nameof(FinancierTransactions));
            }
        }

        public List<Location> Locations
        {
            get => locations;
            private set
            {
                locations = value;
                RaisePropertyChanged(nameof(Locations));
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
                    Accounts = new List<Account>(
                        originalAccounts.Where(x => x.Id != _monoAccount.Id).OrderByDescending(x => x.IsActive).ThenBy(x => x.SortOrder));
                }
            }
        }

        public List<Project> Projects
        {
            get => projects;
            private set
            {
                projects = value;
                RaisePropertyChanged(nameof(Projects));
            }
        }

        public override string Title => "Please select categories";
        public override bool IsValid()
        {
            return true;
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
    }
}
