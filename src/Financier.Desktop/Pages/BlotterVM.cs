namespace Financier.Desktop.ViewModel
{
    using Financier.Common;
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
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text.Json.Serialization;
    using System.Threading.Tasks;

    public class BlotterVM : EntityBaseVM<BlotterModel>
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
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
            get => _account ??= DbManual.Account.Find(p => !p.Id.HasValue);
            set
            {
                _account = value;
                RaisePropertyChanged(nameof(Account));
            }
        }

        public CategoryModel Category
        {
            get => _category ??= DbManual.Category.Find(p => !p.Id.HasValue);
            set
            {
                _category = value;
                RaisePropertyChanged(nameof(Category));
            }
        }

        public PayeeModel Payee
        {
            get => _payee ??= DbManual.Payee.Find(p => !p.Id.HasValue);
            set
            {
                _payee = value;
                RaisePropertyChanged(nameof(Payee));
            }
        }

        public ProjectModel Project
        {
            get => _project ??= DbManual.Project.Find(p => !p.Id.HasValue);
            set
            {
                _project = value;
                RaisePropertyChanged(nameof(Project));
            }
        }

        public LocationModel Location
        {
            get => _location ??= DbManual.Location.Find(p => !p.Id.HasValue);
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
            _from = null;
            _to = null;
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
                await DeleteTransaction(item.Id);
                await db.RebuildAccountBalanceAsync(item.FromAccountId);
                await db.RebuildAccountBalanceAsync(item.ToAccountId ?? 0);
                await RefreshData();
            }
        }

        private async Task DeleteTransaction(int id)
        {
            Logger.Info($"On Transaction delete id : {id}");
            using (var uow = db.CreateUnitOfWork())
            {
                var repo = uow.GetRepository<Transaction>();
                var transaction = await repo.FindByAsync(x => x.Id == id);

                await repo.DeleteAsync(transaction);
                await uow.SaveChangesAsync();
            }
        }

        protected override async Task OnEdit(BlotterModel item)
        {
            if (item.Type == "Transfer")
            {
                var transfer = await GetTransfer(item.Id);
                await OpenTransferDialogAsync(transfer);
            }
            else
            {
                var t = await GetTransaction(item.Id);
                await OpenTransactionDialogAsync(t.transaction, t.subTransactions);
            }
        }

        private async Task AddTransfer()
        {
            Transaction transfer = await db.GetOrCreateTransactionAsync(0);
            if (Account?.Id != null)
            {
                transfer.FromAccountId = (int)Account.Id;
            }
            await OpenTransferDialogAsync(transfer);
        }

        private async Task OnDuplicate(BlotterModel item)
        {
            if (item.Type == "Transfer")
            {
                var transfer = await GetTransfer(item.Id, true);
                await OpenTransferDialogAsync(transfer);
            }
            else
            {
                var t = await GetTransaction(item.Id, true);
                await OpenTransactionDialogAsync(t.transaction, t.subTransactions);
            }
        }

        protected override void OnSelectedValueChanged()
        {
            DuplicateCommand.RaiseCanExecuteChanged();
            base.OnSelectedValueChanged();
        }

        protected override async Task OnAdd()
        {
            Transaction transaction = await db.GetOrCreateTransactionAsync(0);
            IEnumerable<Transaction> subTransactions = await db.GetSubTransactionsAsync(0);

            if (Account?.Id != null)
            {
                transaction.FromAccountId = (int)Account.Id;
            }

            await OpenTransactionDialogAsync(transaction, subTransactions);
        }

        private async Task OpenTransferDialogAsync(Transaction transfer)
        {
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

        private async Task<Transaction> GetTransfer(int id, bool isDuplicate = false)
        {
            Transaction transfer = await db.GetOrCreateTransactionAsync(id);
            if (isDuplicate)
            {
                transfer.Id = 0;
                transfer.DateTime = UnixTimeConverter.ConvertBack(DateTime.Now);
            }

            return transfer;
        }

        private async Task<(Transaction transaction, IEnumerable<Transaction> subTransactions)> GetTransaction(int id, bool isDuplicate = false)
        {
            Transaction transaction = await db.GetOrCreateTransactionAsync(id);
            IEnumerable<Transaction> subTransactions = await db.GetSubTransactionsAsync(id);

            if (isDuplicate)
            {
                transaction.Id = 0;
                transaction.DateTime = UnixTimeConverter.ConvertBack(DateTime.Now);
            }

            return (transaction, subTransactions);
        }

        private async Task OpenTransactionDialogAsync(Transaction transaction, IEnumerable<Transaction> subTransactions)
        {
            var transactionDto = new TransactionDto(transaction, subTransactions);

            TransactionControlVM dialogVm = new TransactionControlVM(transactionDto, dialogWrapper);

            var result = dialogWrapper.ShowDialog<TransactionControl>(dialogVm, 640, 340, nameof(Transaction));

            var resultVm = result as TransactionDto;
            if (resultVm != null)
            {
                await SaveTransactionResult(transaction, subTransactions, resultVm);
            }
        }

        private async Task SaveTransactionResult(Transaction transaction, IEnumerable<Transaction> subTransactions, TransactionDto resultVm)
        {
            var resultTransactions = new List<Transaction>();

            MapperHelper.MapTransaction(resultVm, transaction);
            long totalFromAmountHomeCurrency = transaction.FromAmount;
            resultTransactions.Add(transaction);
            if (resultVm.SubTransactions?.Any() == true)
            {
                // TODO : Add Unit Test for code below
                foreach (var subTransactionDto in resultVm.SubTransactions.OfType<TransactionDto>())
                {
                    Transaction subTransaction = await GetSubTransaction(transaction, resultVm, subTransactionDto);

                    //Set FromAmount in home currency
                    if (resultVm.IsOriginalFromAmountVisible)
                    {
                        var originalFromAmount = (subTransactionDto).RealFromAmount;
                        subTransaction.FromAmount = (long)(originalFromAmount * resultVm.Rate);
                        subTransaction.OriginalFromAmount = originalFromAmount;
                        totalFromAmountHomeCurrency -= subTransaction.FromAmount;
                    }

                    resultTransactions.Add(subTransaction);
                }

                if (!resultVm.IsOriginalFromAmountVisible)
                {
                    foreach (var subTranfer in resultVm.SubTransactions.OfType<TransferDto>())
                    {
                        Transaction subTransaction = await GetSubTransfer(transaction, resultVm, subTranfer);
                        resultTransactions.Add(subTransaction);
                    }
                }

                // check if sum of all subtransaction == parentTransaction.FromAmount
                // if not - add diference to last transaction
                if (resultVm.IsOriginalFromAmountVisible && totalFromAmountHomeCurrency != 0)
                {
                    resultTransactions.Last().FromAmount += totalFromAmountHomeCurrency;
                }
            }

            await db.InsertOrUpdateAsync(resultTransactions);

            await ProcessDeletedTransactions(subTransactions, resultTransactions);

            await db.RebuildAccountBalanceAsync(transaction.FromAccountId);
            var toAccounts = resultTransactions.Select(x => x.ToAccountId).Where(x => x > 0).Distinct().ToList();
            foreach (var account in toAccounts)
            {
                await db.RebuildAccountBalanceAsync(account);
            }
            await RefreshData();
        }

        private async Task ProcessDeletedTransactions(IEnumerable<Transaction> subTransactions, List<Transaction> resultTransactions)
        {
            var transactionsIds = resultTransactions.Select(x => x.Id).Distinct().ToList();
            var deletedSubTransaction = subTransactions.Select(x => x.Id).Where(x => !transactionsIds.Contains(x));
            foreach (var t in deletedSubTransaction)
            {
                await DeleteTransaction(t);
            }
        }

        private async Task<Transaction> GetSubTransfer(Transaction transaction, TransactionDto resultVm, TransferDto subTranfer)
        {
            var subTransaction = await db.GetOrCreateAsync<Transaction>(subTranfer.Id);

            subTranfer.Date = resultVm.Date;
            subTranfer.Time = resultVm.Time;
            MapperHelper.MapTransfer(subTranfer, subTransaction);
            subTransaction.Parent = transaction;
            subTransaction.FromAccountId = transaction.FromAccountId;
            return subTransaction;
        }

        private async Task<Transaction> GetSubTransaction(Transaction transaction, TransactionDto resultVm, TransactionDto subTransactionDto)
        {
            var subTransaction = await db.GetOrCreateAsync<Transaction>(subTransactionDto.Id);
            subTransactionDto.Date = resultVm.Date;
            subTransactionDto.Time = resultVm.Time;
            MapperHelper.MapTransaction(subTransactionDto, subTransaction);
            subTransaction.Parent = transaction;
            subTransaction.FromAccountId = transaction.FromAccountId;
            subTransaction.OriginalCurrencyId = transaction.OriginalCurrencyId ?? transaction.FromAccount.CurrencyId;
            subTransaction.Category = default;
            return subTransaction;
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
                x => x.FromAccountCurrency,
                x => x.ToAccountCurrency,
                x => x.OriginalCurrency);

            if (items != null)
            {
                Entities = new System.Collections.ObjectModel.ObservableCollection<BlotterModel>(items.OrderByDescending(x => x.Datetime));
            }
        }
    }
}
