using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Financier.Common.Entities;
using Financier.Common.Localization;
using Financier.Common.Model;
using Financier.DataAccess.Abstractions;
using Financier.DataAccess.Data;
using Financier.Desktop.Data;
using Financier.Desktop.Helpers;
using Financier.Desktop.ViewModel.Dialog;
using Financier.Desktop.Views.Controls;

namespace Financier.Desktop.ViewModel
{
    [ExcludeFromCodeCoverage]
    public class AccountsVM : EntityBaseVM<AccountModel>
    {
        public AccountsVM(IFinancierDatabase db, IDialogWrapper dialogWrapper)
            : base(db, dialogWrapper)
        {
        }

        protected override Task OnAdd() => OpenAccountDialogAsync(0);

        protected override Task OnEdit(AccountModel item) => OpenAccountDialogAsync(item.Id ?? 0);

        protected override async Task OnDelete(AccountModel item)
        {
            if (!dialogWrapper.ShowMessageBox(
                    LocalizationService.Instance.confirm_delete_account,
                    LocalizationService.Instance.delete,
                    yesNoButtons: true))
                return;

            var account = await db.GetOrCreateAsync<Account>(item.Id ?? 0);
            account.IsActive = false;
            await db.InsertOrUpdateAsync(new[] { account });

            DbManual.ResetManuals(nameof(DbManual.Account));
            await DbManual.SetupAsync(db);
            await RefreshData();
        }

        protected override async Task RefreshData()
        {
            using var uow = db.CreateUnitOfWork();
            var accountRepo = uow.GetRepository<Account>();
            var items = await accountRepo.FindManyAndProjectAsync(
                predicate: x => true,
                projection: acc => new AccountModel(acc),
                includes: x => x.Currency);

            Entities = new ObservableCollection<AccountModel>(
                items.OrderByDescending(x => x.IsActive).ThenBy(x => x.SortOrder));
        }

        private async Task OpenAccountDialogAsync(int id)
        {
            var isNew = id == 0;
            Account account = await db.GetOrCreateAsync<Account>(id);

            AccountDto dto;
            if (isNew)
            {
                dto = new AccountDto
                {
                    Type = "CASH",
                    IsActive = true,
                    IsIncludeIntoTotals = true,
                };
            }
            else
            {
                dto = new AccountDto(account);
            }

            var vm = new AccountControlVM(dto, isNew);
            var result = dialogWrapper.ShowDialog<AccountControl>(
                vm, 580, 560, LocalizationService.Instance["account"]);

            if (result is not AccountDto updated)
                return;

            ApplyDto(account, updated);
            await db.InsertOrUpdateAsync(new[] { account });

            if (isNew && updated.OpeningAmount != 0)
            {
                var t = new Transaction
                {
                    FromAccountId = account.Id,
                    CategoryId = 0,
                    FromAmount = updated.OpeningAmount,
                    DateTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                };
                await db.InsertOrUpdateAsync(new[] { t });
            }

            DbManual.ResetManuals(nameof(DbManual.Account));
            await DbManual.SetupAsync(db);
            await RefreshData();
        }

        private static void ApplyDto(Account account, AccountDto dto)
        {
            account.Title = dto.Title;
            account.IsActive = dto.IsActive;
            account.Type = dto.Type;
            account.CurrencyId = dto.CurrencyId;
            account.CardIssuer = dto.CardIssuer;
            account.Issuer = dto.Issuer;
            account.Number = dto.Number;
            account.LimitAmount = dto.LimitAmount;
            account.SortOrder = dto.SortOrder;
            account.IsIncludeIntoTotals = dto.IsIncludeIntoTotals;
            account.Note = dto.Note;
            account.ClosingDay = dto.ClosingDay;
            account.PaymentDay = dto.PaymentDay;
        }
    }
}
