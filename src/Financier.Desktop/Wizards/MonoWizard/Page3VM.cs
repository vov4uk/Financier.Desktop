using Financier.Common.Entities;
using Financier.Common.Model;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace Financier.Desktop.Wizards.MonoWizard.ViewModel
{
    public class Page3VM : WizardPageBaseVM
    {
        private DelegateCommand _clearAllNotesCommand;
        private DelegateCommand<FinancierTransactionDto> _deleteCommand;
        List<AccountFilterModel> accounts;
        private AccountFilterModel _monoAccount;
        private ObservableCollection<FinancierTransactionDto> financierTransactions;
        private readonly Regex CardNumberRegex = new Regex(@"(\*\*\*\*)([0-9]{4})");

        public Page3VM()
        {
            Accounts = DbManual.Account.OrderByDescending(x => x.IsActive).ThenBy(x => x.SortOrder).ToList();
        }

        public DelegateCommand<FinancierTransactionDto> DeleteCommand
        {
            get
            {
                return _deleteCommand ??= new DelegateCommand<FinancierTransactionDto>(tr => { financierTransactions.Remove(tr); });
            }
        }

        public DelegateCommand ClearAllNotesCommand
        {
            get
            {
                return _clearAllNotesCommand ??= new DelegateCommand(ClearAllNotes);
            }
        }

        public ObservableCollection<FinancierTransactionDto> FinancierTransactions
        {
            get => financierTransactions;
            private set
            {
                financierTransactions = value;
                RaisePropertyChanged(nameof(FinancierTransactions));
            }
        }

        public List<AccountFilterModel> Accounts
        {
            get => accounts;
            private set
            {
                accounts = value;
                RaisePropertyChanged(nameof(Accounts));
            }
        }

        public AccountFilterModel MonoAccount
        {
            get => _monoAccount;
            set
            {
                _monoAccount = value;
                RaisePropertyChanged(nameof(MonoAccount));
                if (_monoAccount != null)
                {
                    Accounts = new List<AccountFilterModel>(
                        DbManual.Account.Where(x => x.Id != _monoAccount.Id).OrderByDescending(x => x.IsActive).ThenBy(x => x.SortOrder));
                }
            }
        }

        public override string Title => "Please select categories";
        public override bool IsValid()
        {
            return true;
        }

        public void SetMonoTransactions(List<BankTransaction> transactions)
        {
            List<FinancierTransactionDto> transToAdd = new List<FinancierTransactionDto>();
            foreach (var x in transactions)
            {
                var locationId = DbManual.Location.Where(x => x.Id > 0).FirstOrDefault(l => !string.IsNullOrEmpty(l.Title) && l.Title.Contains(x.Description, StringComparison.OrdinalIgnoreCase)
                                                            || !string.IsNullOrEmpty(l.Address) && l.Address.Contains(x.Description, StringComparison.OrdinalIgnoreCase))?.Id ?? 0;
                var categoryId = DbManual.Category.Where(x => x.Id > 0).FirstOrDefault(l => l.Title.Contains(x.Description, StringComparison.OrdinalIgnoreCase))?.Id ?? 0;

                long amount = Convert.ToInt64(x.CardCurrencyAmount * 100.0);
                int toAccountId = 0;
                int fromAccountId = 0;


                if (CardNumberRegex.IsMatch(x.Description))
                {
                    var acc = DbManual.Account.FirstOrDefault(y => !string.IsNullOrWhiteSpace(y.Number) && x.Description.EndsWith(y.Number));
                    if (acc != null)
                    {
                        if (amount < 0)
                        {
                            toAccountId = acc.Id.Value;
                        }
                        else
                        {
                            fromAccountId = acc.Id.Value;
                        }
                    }
                }

                var newTr = new FinancierTransactionDto
                {
                    MonoAccountId = MonoAccount.Id,
                    FromAmount = amount,
                    OriginalFromAmount = x.ExchangeRate == null ? null : Convert.ToInt64(x.OperationAmount * 100.0),
                    OriginalCurrencyId = x.ExchangeRate == null ? 0 : (DbManual.Currencies.FirstOrDefault(c => c.Name == x.OperationCurrency)?.Id ?? 0),
                    CategoryId = categoryId,
                    ToAccountId = toAccountId,
                    FromAccountId = fromAccountId,
                    LocationId = locationId,
                    Note = (locationId > 0 || categoryId > 0 || toAccountId > 0 || fromAccountId > 0) ? null : x.Description,
                    DateTime = new DateTimeOffset(x.Date).ToUnixTimeMilliseconds()
                };
                transToAdd.Add(newTr);
            }

            FinancierTransactions = new ObservableCollection<FinancierTransactionDto>(transToAdd);
        }

        private void ClearAllNotes()
        {
            foreach (var item in FinancierTransactions)
            {
                item.Note = null;
            }
        }
    }
}
