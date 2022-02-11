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
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    [ExcludeFromCodeCoverage]
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

        public IAsyncCommand DuplicateCommand => _duplicateCommand ??= new AsyncCommand(() => OnDuplicate(SelectedValue), () => SelectedValue != null);

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
                return OpenTransferDialogAsync(item.Id, false);
            }
            else
            {
                return OpenTransactionDialogAsync(item.Id, false);
            }
        }

        private Task AddTransfer()
        {
            return OpenTransferDialogAsync(0, false);
        }

        private Task OnDuplicate(BlotterModel item)
        {
            if (item.Type == "Transfer")
            {
                return OpenTransferDialogAsync(item.Id, true);
            }
            else
            {
                return OpenTransactionDialogAsync(item.Id, true);
            }
        }

        protected override void OnSelectedValueChanged()
        {
            DuplicateCommand.RaiseCanExecuteChanged();
            base.OnSelectedValueChanged();
        }

        protected override Task OnAdd()
        {
            return OpenTransactionDialogAsync(0, false);
        }

        private async Task OpenTransferDialogAsync(int id, bool isDuplicate)
        {
            Transaction transfer = await db.GetOrCreateTransactionAsync(id);
            if (isDuplicate)
            {
                transfer.Id = 0;
                transfer.DateTime = UnixTimeConverter.ConvertBack(DateTime.Now);
            }

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
            }
        }

        private async Task OpenTransactionDialogAsync(int id, bool isDuplicate)
        {
            Transaction transaction = await db.GetOrCreateTransactionAsync(id);
            IEnumerable<Transaction> subTransactions = await db.GetSubTransactionsAsync(id);

            if (isDuplicate)
            {
                transaction.Id = 0;
                transaction.DateTime = UnixTimeConverter.ConvertBack(DateTime.Now);
            }

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

            Expression<Func<BlotterTransactions, bool>> predicate = x => x.DateTime >= fromUnix && x.DateTime <= toUnix;

            if (Account?.Id != null)
            {
                predicate = predicate.And(x => x.FromAccountId == _account.Id || x.ToAccountId == _account.Id);
            }

            if (Category?.Id != null)
            {
                predicate = predicate.And(x => x.CategoryId == _category.Id && x.ToAccountId == null);
            }

            if (Project?.Id != null)
            {
                predicate = predicate.And(x => x.ProjectId == _project.Id);
            }

            if (Payee?.Id != null)
            {
                predicate = predicate.And(x => x.PayeeId == _payee.Id);
            }

            if (Location?.Id != null)
            {
                predicate = predicate.And(x => x.LocationId == _location.Id);
            }

            var items = await repo.FindManyAsync(
                predicate: predicate,
                projection: x => new BlotterModel
                {
                    Id = x.Id,
                    FromAccountId  = x.FromAccountId,
                    FromAccountTitle = x.FromAccountTitle,
                    ToAccountId = x.ToAccountId,
                    ToAccountTitle = x.ToAccountTitle,
                    FromAccountCurrencyId = x.FromAccountCurrencyId,
                    CategoryId = x.CategoryId,
                    CategoryTitle = x.CategoryTitle,
                    LocationId = x.LocationId,
                    Location = x.Location,
                    Payee = x.Payee,
                    Note = x.Note,
                    FromAmount = x.FromAmount,
                    ToAmount = x.ToAmount,
                    Datetime = x.DateTime,
                    OriginalCurrencyId = x.OriginalCurrencyId,
                    OriginalFromAmount = x.OriginalFromAmount,
                    FromAccountBalance = x.FromAccountBalance,
                    ToAccountBalance = x.ToAccountBalance,
                    FromAccountCurrency = new CurrencyModel
                    {
                        Id = x.FromAccountCurrency.Id,
                        Name = x.FromAccountCurrency.Name,
                        Symbol = x.FromAccountCurrency.Symbol,
                    },
                    ToAccountCurrency = x.ToAccountCurrency == null ? default : new CurrencyModel
                    {
                        Id = x.ToAccountCurrency.Id,
                        Name = x.ToAccountCurrency.Name,
                        Symbol = x.ToAccountCurrency.Symbol,
                    },
                    OriginalCurrency = x.OriginalCurrency == null ? default : new CurrencyModel
                    {
                        Id = x.OriginalCurrency.Id,
                        Name = x.OriginalCurrency.Name,
                        Symbol = x.OriginalCurrency.Symbol,
                    }
                },
                includes: new Expression<Func<BlotterTransactions, object>>[] { x => x.FromAccountCurrency, x => x.ToAccountCurrency, x => x.OriginalCurrency });

            if (items != null)
            {
                Entities = new System.Collections.ObjectModel.ObservableCollection<BlotterModel>(items.OrderByDescending(x => x.Datetime));
            }
        }
    }
}
