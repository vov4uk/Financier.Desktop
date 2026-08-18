using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Financier.Common.Entities;
using Financier.Common.Localization;
using Financier.Common.Model;
using Financier.Converters;
using Financier.Desktop.Data;
using Financier.Desktop.Helpers;
using Financier.Desktop.Pages.Dialogs;
using Financier.Desktop.ViewModel.Dialog;
using Financier.Desktop.Views;
using Prism.Commands;

namespace Financier.Desktop.Wizards.MonoWizard.ViewModel
{
    public class Page3VM : WizardPageBaseVM
    {
        private readonly IDialogWrapper _dialogWrapper;
        private DelegateCommand _clearAllNotesCommand;
        private DelegateCommand<FinancierTransactionDto> _deleteCommand;
        private AsyncDelegateCommand<FinancierTransactionDto> _addRuleCommand;
        private AsyncDelegateCommand<FinancierTransactionDto> _transferCommand;
        List<AccountFilterModel> accounts;
        private AccountFilterModel _monoAccount;
        private ObservableCollection<FinancierTransactionDto> financierTransactions;
        private static readonly Regex CardNumberRegex = new Regex(@"(\*)([0-9]{4})", RegexOptions.None, TimeSpan.FromMilliseconds(1000));

        public Page3VM(IDialogWrapper dialogWrapper)
        {
            this._dialogWrapper = dialogWrapper;
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

        public AsyncDelegateCommand<FinancierTransactionDto> AddRuleCommand
        {
            get
            {
                return _addRuleCommand ??= new AsyncDelegateCommand<FinancierTransactionDto>(tr => OpenRulesDialogAsync(tr?.Note, tr?.MCC ?? 0));
            }
        }

        public AsyncDelegateCommand<FinancierTransactionDto> TransferCommand
        {
            get
            {
                return _transferCommand ??= new AsyncDelegateCommand<FinancierTransactionDto>(OpenTransferDialogAsync);
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

        public override string Title => LocalizationService.Instance.please_select_categories;
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
                    MCC = mcc,
                    IsAmountNegative = amount < 0
                };

                ApplyRules(newTr);

                transToAdd.Add(newTr);
            }

            FinancierTransactions = new ObservableCollection<FinancierTransactionDto>(transToAdd);
        }

        private static void ApplyRules(FinancierTransactionDto transaction)
        {
            foreach (var rule in DbManual.Rules.Where(r => r.IsActive))
            {
                HashSet<int> mccCodes = new HashSet<int>();
                if (rule.Condition == RuleConditionType.MCC && DbManual.MCCEnums.ContainsKey(rule.MCCCategory))
                {
                    var list = DbManual.MCCEnums[rule.MCCCategory];
                    mccCodes = [.. list];
                }

                bool meetsCondition = false;
                if (rule.Condition == RuleConditionType.DescriptionContains && !string.IsNullOrEmpty(transaction.Note))
                {
                    if (transaction.Note.Contains(rule.Description, StringComparison.OrdinalIgnoreCase))
                    {
                        meetsCondition = true;
                    }
                }
                else if (rule.Condition == RuleConditionType.DescriptionMatches && !string.IsNullOrEmpty(transaction.Note))
                {
                    if (transaction.Note.Equals(rule.Description, StringComparison.OrdinalIgnoreCase))
                    {
                        meetsCondition = true;
                    }
                }
                else if (rule.Condition == RuleConditionType.MCC && transaction.MCC > 0 && mccCodes.Contains(transaction.MCC))
                {
                    meetsCondition = true;
                }

                if (meetsCondition)
                {
                    if (rule.CategoryId.HasValue)
                    {
                        transaction.CategoryId = rule.CategoryId.Value;
                    }
                    if (rule.LocationId.HasValue)
                    {
                        transaction.LocationId = rule.LocationId.Value;
                    }
                    if (rule.PayeeId.HasValue)
                    {
                        transaction.PayeeId = rule.PayeeId.Value;
                    }
                    if (rule.ProjectId.HasValue)
                    {
                        transaction.ProjectId = rule.ProjectId.Value;
                    }
                }
            }
        }

        private void ClearAllNotes()
        {
            foreach (var item in FinancierTransactions)
            {
                item.Note = null;
            }
        }

        private static (int categoryId, int locationId, int accountId) ParseDescription(string description)
        {
            int accountId, locationId, categoryId;
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

        private static void TryParseLocation(string desc, out int locationId)
        {
            locationId = 0;
            var location = DbManual.Location
                .Where(x => x.Id > 0 && x.IsActive)
                .FirstOrDefault(l => ContainsString(l.Title, desc) || ContainsString(l.Address, desc));
            if (location != null)
            {
                locationId = location.Id.Value;
            }
        }

        private static void TryParseCategory(string desc, out int categoryId)
        {
            categoryId = 0;
            var category = DbManual.Category
                    .Where(x => x.Id > 0)
                    .FirstOrDefault(l => ContainsString(l.Title, desc));
            if (category != null)
            {
                categoryId = category.Id.Value;
            }
        }

        private static void TryParseAccount(string desc, out int accountId)
        {
            accountId = 0;
            var res = CardNumberRegex.Match(desc);

            if (res.Success && res.Groups.Count > 2)
            {
                string cardNumber = res.Groups[2].Value;
                var acc = DbManual.Account
                    .Find(y => !string.IsNullOrWhiteSpace(y.Number) && string.Equals(y.Number, cardNumber, StringComparison.InvariantCultureIgnoreCase));

                if (acc?.Id != null)
                {
                    accountId = acc.Id.Value;
                }
            }
        }

        private AccountFilterModel GetOtherAccount(FinancierTransactionDto tr, out bool transferToMono)
        {
            transferToMono = false;
            if (tr == null)
            {
                return null;
            }

            if (tr.FromAccountId > 0) // Transfer To Mono
            {
                transferToMono = true;
                return Accounts.FirstOrDefault(a => a.Id == tr.FromAccountId);
            }

            if (tr.ToAccountId > 0) // Transfer From Mono
            {
                return Accounts.FirstOrDefault(a => a.Id == tr.ToAccountId);
            }

            return null;
        }

        private Task OpenTransferDialogAsync(FinancierTransactionDto tr)
        {
            var otherAccount = GetOtherAccount(tr, out var transferToMono);
            if (otherAccount == null || MonoAccount == null)
            {
                return Task.CompletedTask;
            }

            var fromAccount = transferToMono ? otherAccount : MonoAccount;
            var toAccount = transferToMono ? MonoAccount : otherAccount;
            var monoAmount = Math.Abs(tr.FromAmount);

            var transferDto = new TransferDto
            {
                FromAccountId = fromAccount.Id ?? 0,
                FromAccount = fromAccount,
                ToAccountId = toAccount.Id ?? 0,
                ToAccount = toAccount,
                Note = tr.Note,
                Date = UnixTimeConverter.Convert(tr.DateTime).Date,
                Time = UnixTimeConverter.Convert(tr.DateTime)
            };

            if (transferToMono)
            {
                transferDto.ToAmount = monoAmount;
            }
            else
            {
                transferDto.FromAmount = monoAmount;
            }

            TransferControlVM dialogVm = new TransferControlVM(transferDto);
            var result = _dialogWrapper.ShowDialog<TransferControl>(dialogVm, 385, 340, LocalizationService.Instance.transfer);

            if (result is TransferDto output)
            {
                tr.OriginalCurrencyId = otherAccount.CurrencyId;
                tr.OriginalFromAmount = transferToMono ? Math.Abs(output.FromAmount) : Math.Abs(output.ToAmount);
                tr.Note = output.Note;
            }

            return Task.CompletedTask;
        }

        private async Task OpenRulesDialogAsync(string description, int mccCode)
        {
            Mcc mcc = Mcc.none;
            if (DbManual.MCCCodes.TryGetValue(mccCode, out var mccValue))
            {
                mcc = mccValue;
            }

            RuleDto rule = new RuleDto()
            {
                Description = description,
                Condition = RuleConditionType.DescriptionContains,
                Created = DateTime.Now,
                IsActive = true,
                MCCCategory = mcc
            };

            RuleControlVM ruleVm = new RuleControlVM(rule);

            var result = _dialogWrapper.ShowDialog<RuleControl>(ruleVm, 380, 400, LocalizationService.Instance.rule);

            var updatedItem = result as RuleDto;
            if (updatedItem != null)
            {
                DbManual.Rules.Add(new RuleModel
                {
                    Description = updatedItem.Description,
                    CategoryId = updatedItem.CategoryId,
                    Condition = updatedItem.Condition,
                    Created = updatedItem.Created,
                    Id = DbManual.Rules.Count > 0 ? DbManual.Rules.Max(r => r.Id) + 1 : 1,
                    IsActive = updatedItem.IsActive,
                    LocationId = updatedItem.LocationId,
                    PayeeId = updatedItem.PayeeId,
                    ProjectId = updatedItem.ProjectId,
                    MCCCategory = updatedItem.MCCCategory
                });

                await DbManual.SaveRulesAsync();
                await DbManual.LoadRulesAsync();

                foreach (var transaction in FinancierTransactions)
                {
                    ApplyRules(transaction);
                }
            }
        }
    }
}
