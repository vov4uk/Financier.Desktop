using Financier.Common.Entities;
using Financier.Common.Model;
using Financier.DataAccess.Data;
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
        private static readonly Regex CardNumberRegex = new Regex(@"(\*)([0-9]{4})", RegexOptions.None, TimeSpan.FromMilliseconds(1000));

        public Page3VM()
        {
            Accounts = DbManual.Account
                .OrderByDescending(x => x.IsActive)
                .ThenBy(x => x.SortOrder)
                .ToList();
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
                var parsedDescription = ParseDescription(x.Description);
                long amount = Convert.ToInt64(x.CardCurrencyAmount * 100.0);
                int toAccountId = amount < 0 ? parsedDescription.accountId : 0;
                int fromAccountId = amount > 0 ? parsedDescription.accountId : 0;
                int.TryParse(x.MCC ?? "0", out var mcc);

                var newTr = new FinancierTransactionDto
                {
                    MonoAccountId = MonoAccount.Id,
                    FromAmount = amount,
                    OriginalFromAmount = x.ExchangeRate == null ? null : Convert.ToInt64(x.OperationAmount * 100.0),
                    OriginalCurrencyId = x.ExchangeRate == null ? 0 : (DbManual.Currencies.FirstOrDefault(c => c.Name == x.OperationCurrency)?.Id ?? 0),
                    CategoryId = parsedDescription.categoryId,
                    ToAccountId = toAccountId,
                    FromAccountId = fromAccountId,
                    LocationId = parsedDescription.locationId,
                    Note = x.Description,
                    DateTime = new DateTimeOffset(x.Date).ToUnixTimeMilliseconds(),
                    MCC = mcc
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

        private (int categoryId, int locationId, int accountId) ParseDescription(string description)
        {
            int accountId = 0, locationId = 0, categoryId = 0;
            TryParseLocation(description, out locationId);
            TryParseCategory(description, out categoryId);
            TryParseAccount(description, out accountId);
            return (categoryId, locationId, accountId);
        }

        private static bool ContainsString(string title, string description)
        {
            if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(description))
            {
                return description.Split(" ").Where(x => !string.IsNullOrEmpty(x) && x.Length > 2).Any(x => title.Contains(x, StringComparison.OrdinalIgnoreCase));
            }
            return false;
        }

        private static bool TryParseLocation(string desc, out int locationId)
        {
              var location = DbManual.Location
                .Where(x => x.Id > 0)
                .FirstOrDefault(l => ContainsString(l.Title, desc) || ContainsString(l.Address, desc));
            if (location != null)
            {
                locationId = location.Id.Value;
                return true;
            }
            locationId = 0;
            return false;
        }

        private static bool TryParseCategory(string desc, out int categoryId)
        {
            var category = DbManual.Category
                    .Where(x => x.Id > 0)
                    .FirstOrDefault(l => ContainsString(l.Title, desc));
            if (category != null)
            {
                categoryId = category.Id.Value;
                return true;
            }
            categoryId = 0;
            return false;
        }

        private static bool TryParseAccount(string desc, out int accountId)
        {
            var res = CardNumberRegex.Match(desc);

            if (res.Success && res.Groups.Count > 2)
            {
                string cardNumber = res.Groups[2].Value;
                var acc = DbManual.Account
                    .FirstOrDefault(y => !string.IsNullOrWhiteSpace(y.Number) && string.Equals(y.Number, cardNumber, StringComparison.InvariantCultureIgnoreCase));

                if (acc != null)
                {
                    accountId = acc.Id.Value;
                    return true;
                }
            }
            accountId = 0;
            return false;
        }
    }
}
