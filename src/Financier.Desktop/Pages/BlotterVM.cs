namespace Financier.Desktop.ViewModel
{
    using Financier.Common.Entities;
    using Financier.Common.Model;
    using Financier.Converters;
    using Financier.DataAccess.Abstractions;
    using Financier.DataAccess.Data;
    using Financier.DataAccess.Utils;
    using Financier.DataAccess.View;
    using Financier.Desktop.Data;
    using Financier.Desktop.Helpers;
    using Financier.Desktop.ViewModel.Dialog;
    using Financier.Desktop.Views;
    using Mvvm.Async;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    public class BlotterVM : EntityBaseVM<BlotterModel>
    {
        private IAsyncCommand _addTemplateCommand;
        private IAsyncCommand _addTransferCommand;
        private IAsyncCommand _duplicateCommand;
        private IAsyncCommand _clearFiltersCommand;
        private IAsyncCommand _infoCommand;
        private DateTime? _from;
        private DateTime? _to;
        private PeriodType _periodType;
        private AccountFilterModel _account;
        private CategoryModel _category;
        private PayeeModel _payee;
        private ProjectModel _project;
        private LocationModel _location;

        public BlotterVM(IFinancierDatabase db, IDialogWrapper dialogWrapper)
            : base(db, dialogWrapper)
        {
        }

        public DateTime? From
        {
            get => _from;
            set
            {
                if (SetProperty(ref _from, value))
                {
                    RaisePropertyChanged(nameof(From));
                }
            }
        }

        public DateTime? To
        {
            get => _to;
            set
            {
                if (SetProperty(ref _to, value))
                {
                    RaisePropertyChanged(nameof(To));
                }
            }
        }

        public PeriodType PeriodType
        {
            get => _periodType;
            set
            {
                if (SetProperty(ref _periodType, value))
                {
                    RaisePropertyChanged(nameof(PeriodType));
                }
            }
        }

        public AccountFilterModel Account
        {
            get => _account ??= DbManual.Account.FirstOrDefault(p => !p.Id.HasValue);
            set
            {
                _account = value;
                RaisePropertyChanged(nameof(Account));
            }
        }

        public CategoryModel Category
        {
            get => _category ??= DbManual.Category.FirstOrDefault(p => !p.Id.HasValue);
            set
            {
                _category = value;
                RaisePropertyChanged(nameof(Category));
            }
        }

        public PayeeModel Payee
        {
            get => _payee ??= DbManual.Payee.FirstOrDefault(p => !p.Id.HasValue);
            set
            {
                _payee = value;
                RaisePropertyChanged(nameof(Payee));
            }
        }

        public ProjectModel Project
        {
            get => _project ??= DbManual.Project.FirstOrDefault(p => !p.Id.HasValue);
            set
            {
                _project = value;
                RaisePropertyChanged(nameof(Project));
            }
        }

        public LocationModel Location
        {
            get => _location ??= DbManual.Location.FirstOrDefault(p => !p.Id.HasValue);
            set
            {
                _location = value;
                RaisePropertyChanged(nameof(Location));
            }
        }

        public IAsyncCommand AddTemplateCommand => _addTemplateCommand ??= new AsyncCommand(() => Task.CompletedTask, () => false);

        public IAsyncCommand AddTransferCommand => _addTransferCommand ??= new AsyncCommand(AddTransfer);

        public IAsyncCommand DuplicateCommand => _duplicateCommand ??= new AsyncCommand(() => Task.CompletedTask, () => false);

        public IAsyncCommand ClearFiltersCommand => _clearFiltersCommand ??= new AsyncCommand(ClearFilters);

        public IAsyncCommand InfoCommand => _infoCommand ??= new AsyncCommand(() => Task.CompletedTask, () => false);

        private async Task ClearFilters()
        {
            PeriodType = PeriodType.AllTime;
            Account = default;
            Category = default;
            Payee = default;
            Project = default;
            Location = default;
            await RefreshDataCommand.ExecuteAsync();
        }

        protected override async Task OnDelete(BlotterModel item)
        {
            if (this.dialogWrapper.ShowMessageBox("Are you sure you want to delete transaction?", "Delete", true))
            {
                using (var uow = db.CreateUnitOfWork())
                {
                    var repo = uow.GetRepository<Transaction>();
                    var transaction = await repo.FindByAsync(x => x.Id == item.Id);

                    await repo.DeleteAsync(transaction);
                    await uow.SaveChangesAsync();
                }
                await db.RebuildAccountBalanceAsync(item.FromAccountId);
                await db.RebuildAccountBalanceAsync(item.ToAccountId ?? 0);
                await RefreshData();
            }
        }

        protected override Task OnEdit(BlotterModel item)
        {
            if (item.Type == "Transfer")
            {
                return OpenTransferDialogAsync(item.Id);
            }
            else
            {
                return OpenTransactionDialogAsync(item.Id);
            }
        }

        private Task AddTransfer()
        {
            return OpenTransferDialogAsync(0);
        }

        protected override Task OnAdd()
        {
            return OpenTransactionDialogAsync(0);
        }

        private async Task OpenTransferDialogAsync(int id)
        {
            Transaction transfer = await db.GetOrCreateTransactionAsync(id);

            TransferControlVM dialogVm = new TransferControlVM(new TransferDto(transfer));

            var result = dialogWrapper.ShowDialog<TransferControl>(dialogVm, 385, 340, "Transfer");

            var output = result as TransferDto;
            if (output != null)
            {
                MapperHelper.MapTransfer(result as TransferDto, transfer);
                await db.InsertOrUpdateAsync(new[] { transfer });

                await db.RebuildAccountBalanceAsync(transfer.FromAccountId);
                await db.RebuildAccountBalanceAsync(transfer.ToAccountId);
                await RefreshData();

                //await RefreshViewModelAsync<AccountModel>();
            }
        }

        private async Task OpenTransactionDialogAsync(int id)
        {
            Transaction transaction = await db.GetOrCreateTransactionAsync(id);
            IEnumerable<Transaction> subTransactions = await db.GetSubTransactionsAsync(id);
            var transactionDto = new TransactionDto(transaction, subTransactions);

            // if transaction not in home currency, replace FromAmount with OriginalFromAmount to show correct values
            if (transactionDto.IsOriginalFromAmountVisible)
            {
                foreach (var item in transactionDto.SubTransactions)
                {
                    item.FromAmount = item.OriginalFromAmount ?? 0;
                }
            }

            using var uow = db.CreateUnitOfWork();

            TransactionControlVM dialogVm = new TransactionControlVM(transactionDto, dialogWrapper);

            var result = dialogWrapper.ShowDialog<TransactionControl>(dialogVm, 640, 340, nameof(Transaction));

            var resultVm = result as TransactionDto;
            if (resultVm != null)
            {
                var resultTransactions = new List<Transaction>();

                MapperHelper.MapTransaction(resultVm, transaction);
                long fromAmount = transaction.FromAmount;
                resultTransactions.Add(transaction);
                if (resultVm?.SubTransactions?.Any() == true)
                {
                    // TODO : Add Unit Test for code below
                    foreach (var subTransactionDto in resultVm.SubTransactions)
                    {
                        var subTransaction = await db.GetOrCreateAsync<Transaction>(subTransactionDto.Id);
                        subTransactionDto.Date = resultVm.Date;
                        subTransactionDto.Time = resultVm.Time;
                        MapperHelper.MapTransaction(subTransactionDto, subTransaction);
                        subTransaction.Parent = transaction;
                        subTransaction.FromAccountId = transaction.FromAccountId;
                        subTransaction.OriginalCurrencyId = transaction.OriginalCurrencyId ?? transaction.FromAccount.CurrencyId;
                        subTransaction.Category = default;

                        //Set FromAmount in home currency
                        if (resultVm.IsOriginalFromAmountVisible)
                        {
                            var originalFromAmount = subTransactionDto.FromAmount;
                            subTransaction.FromAmount = (long)(originalFromAmount * resultVm.Rate);
                            subTransaction.OriginalFromAmount = originalFromAmount;
                            fromAmount -= subTransaction.FromAmount;
                        }

                        resultTransactions.Add(subTransaction);
                    }

                    // check if sum of all subtransaction == parentTransaction.FromAmount
                    // if not - add diference to last transaction
                    if (resultVm.IsOriginalFromAmountVisible && fromAmount != 0)
                    {
                        resultTransactions.Last().FromAmount += fromAmount;
                    }
                }

                await db.InsertOrUpdateAsync(resultTransactions);
                await db.RebuildAccountBalanceAsync(transaction.FromAccountId);
                await RefreshData();
            }
        }

        protected override async Task RefreshData()
        {
            using var uow = db.CreateUnitOfWork();
            var repo = uow.GetRepository<BlotterTransactions>();
            var fromUnix = UnixTimeConverter.ConvertBack(From ?? DateTime.MinValue.ToLocalTime());
            var toUnix = UnixTimeConverter.ConvertBack(To ?? DateTime.MaxValue.ToLocalTime());

            Expression<Func<BlotterTransactions, bool>> predicate = x => x.datetime >= fromUnix && x.datetime <= toUnix;

            if (Account?.Id != null)
            {
                predicate = predicate.And(x => x.from_account_id == _account.Id || x.to_account_id == _account.Id);
            }

            if (Category?.Id != null)
            {
                predicate = predicate.And(x => x.category_id == _category.Id && x.to_account_id == null);
            }

            if (Project?.Id != null)
            {
                predicate = predicate.And(x => x.project_id == _project.Id);
            }

            if (Payee?.Id != null)
            {
                predicate = predicate.And(x => x.payee_id == _payee.Id);
            }

            if (Location?.Id != null)
            {
                predicate = predicate.And(x => x.location_id == _location.Id);
            }

            var items = await repo.FindManyAsync(
                predicate: predicate,
                projection: x => new BlotterModel
                {
                    Id = x._id,
                    FromAccountId  = x.from_account_id,
                    FromAccountTitle = x.from_account_title,
                    ToAccountId = x.to_account_id,
                    ToAccountTitle = x.to_account_title,
                    FromAccountCurrencyId = x.from_account_currency_id,
                    CategoryId = x.category_id,
                    CategoryTitle = x.category_title,
                    LocationId = x.location_id,
                    Location = x.location,
                    Payee = x.payee,
                    Note = x.note,
                    FromAmount = x.from_amount,
                    ToAmount = x.to_amount,
                    Datetime = x.datetime,
                    OriginalCurrencyId = x.original_currency_id,
                    OriginalFromAmount = x.original_from_amount,
                    FromAccountBalance = x.from_account_balance,
                    ToAccountBalance = x.to_account_balance,
                    FromAccountCurrency = new CurrencyModel
                    {
                        Id = x.from_account_currency.Id,
                        Name = x.from_account_currency.Name,
                        Symbol = x.from_account_currency.Symbol,
                    },
                    ToAccountCurrency = x.to_account_currency == null ? default : new CurrencyModel
                    {
                        Id = x.to_account_currency.Id,
                        Name = x.to_account_currency.Name,
                        Symbol = x.to_account_currency.Symbol,
                    },
                    OriginalCurrency = x.original_currency == null ? default : new CurrencyModel
                    {
                        Id = x.original_currency.Id,
                        Name = x.original_currency.Name,
                        Symbol = x.original_currency.Symbol,
                    }
                },
                includes: new Expression<Func<BlotterTransactions, object>>[] { x => x.from_account_currency, x => x.to_account_currency, x => x.original_currency });
            if (items != null)
            {
                Entities = new System.Collections.ObjectModel.ObservableCollection<BlotterModel>(items.OrderByDescending(x => x.Datetime));
            }
        }
    }
}
