using Financier.DataAccess.Abstractions;
using Financier.DataAccess.Data;
using Financier.DataAccess.Utils;
using Financier.Desktop.Views;
using Financier.Adapter;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Financier.Desktop.ViewModel.Dialog;
using Financier.Desktop.Views.Controls;
using Financier.Desktop.Helpers;
using Financier.Desktop.Wizards.MonoWizard.ViewModel;
using System.IO;
using System.Collections.Concurrent;
using Financier.Desktop.Data;
using Mvvm.Async;
using Financier.Reports;
using Financier.Common.Entities;
using Financier.Common.Model;
using Financier.Common;

namespace Financier.Desktop.ViewModel
{
    public class FinancierVM : BindableBase
    {
        private const string Backup = "backup";
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly ConcurrentDictionary<Type, object> _pages = new ConcurrentDictionary<Type, object>();
        private readonly IBankHelperFactory bankFactory;
        private readonly IFinancierDatabaseFactory dbFactory;
        private readonly IDialogWrapper dialogWrapper;
        private readonly List<Entity> keyLessEntities = new();
        private BackupVersion _backupVersion;
        private Dictionary<string, List<string>> _entityColumnsOrder;
        private IAsyncCommand<Type> _menuNavigateCommand;
        private IAsyncCommand _monoCommand;
        private IAsyncCommand _abankCommand;
        private IAsyncCommand _openBackupCommand;
        private IAsyncCommand _saveBackupCommand;
        private IAsyncCommand _saveBackupAsDbCommand;
        private readonly IBackupWriter backupWriter;
        private BlotterVM blotterVm;
        private BindableBase currentPage;
        private IFinancierDatabase db;
        private readonly IEntityReader entityReader;
        private LocationsVM locationsVm;
        private string openBackupPath;
        private PayeesVM payeesVm;
        private ProjectsVM projectsVm;

        public FinancierVM(IDialogWrapper dialogWrapper,
            IFinancierDatabaseFactory dbFactory,
            IEntityReader entityReader,
            IBackupWriter backupWriter,
            IBankHelperFactory bankFactory)
        {
            this.dialogWrapper = dialogWrapper;
            this.dbFactory = dbFactory;
            this.entityReader = entityReader;
            this.backupWriter = backupWriter;
            this.bankFactory = bankFactory;
            this.db = dbFactory.CreateDatabase();

            CreatePages();
        }

        public BlotterVM Blotter
        {
            get => blotterVm;
            private set => SetProperty(ref blotterVm, value);
        }

        public BindableBase CurrentPage
        {
            get => currentPage;
            private set
            {
                SetProperty(ref currentPage, value);
                Logger.Info($"CurrentPage -> {value?.GetType().FullName}");
                RaisePropertyChanged(nameof(CurrentPage));
                RaisePropertyChanged(nameof(IsTransactionPageSelected));
                RaisePropertyChanged(nameof(IsLocationPageSelected));
                RaisePropertyChanged(nameof(IsProjectPageSelected));
                RaisePropertyChanged(nameof(IsPayeePageSelected));
            }
        }

        public bool IsLocationPageSelected => currentPage is LocationsVM;

        public bool IsPayeePageSelected => currentPage is PayeesVM;

        public bool IsProjectPageSelected => currentPage is ProjectsVM;

        public bool IsTransactionPageSelected => currentPage is BlotterVM;

        public string OpenBackupPath
        {
            get => openBackupPath;
            private set => SetProperty(ref openBackupPath, value);
        }

        public LocationsVM Locations
        {
            get => locationsVm;
            private set => SetProperty(ref locationsVm, value);
        }

        public PayeesVM Payees
        {
            get => payeesVm;
            private set => SetProperty(ref payeesVm, value);
        }

        public ProjectsVM Projects
        {
            get => projectsVm;
            private set => SetProperty(ref projectsVm, value);
        }

        public IAsyncCommand<Type> MenuNavigateCommand => _menuNavigateCommand ??= new AsyncCommand<Type>(NavigateToType);

        public IAsyncCommand MonoCommand => _monoCommand ??= new AsyncCommand(() => OpenMonoWizardAsync("Monobank", "csv"));

        public IAsyncCommand AbankCommand => _abankCommand ??= new AsyncCommand(() => OpenMonoWizardAsync("A-Bank", "pdf"));

        public IAsyncCommand OpenBackupCommand => _openBackupCommand ??= new AsyncCommand(OpenBackup_Click);

        public IAsyncCommand SaveBackupCommand => _saveBackupCommand ??= new AsyncCommand(SaveBackup_Click);

        public IAsyncCommand SaveBackupAsDbCommand => _saveBackupAsDbCommand ??= new AsyncCommand(SaveBackupAsDb);

        public async Task OpenBackup(string backupPath)
        {
            var start = DateTime.Now;
            ClearPages();

            OpenBackupPath = backupPath;
            var (entities, backupVersion, columnsOrder) = this.entityReader.ParseBackupFile(backupPath);
            _backupVersion = backupVersion;
            _entityColumnsOrder = columnsOrder;

            db?.Dispose();

            db = dbFactory.CreateDatabase();
            await db.ImportEntitiesAsync(entities);

            keyLessEntities.Clear();

            AddKeylessEntities(entities.OfType<CCardClosingDate>());
            AddKeylessEntities(entities.OfType<CategoryAttribute>());
            AddKeylessEntities(entities.OfType<TransactionAttribute>());

            var duration = DateTime.Now - start;
            Logger.Info($"Duration : {duration}");
            dialogWrapper.ShowMessageBox($"Imported {entities.Count()} entities. Duration : {duration}","Success");

            DbManual.ResetAllManuals();
            await DbManual.SetupAsync(db);

            await NavigateToType(typeof(BlotterModel));
        }

        public async Task SaveBackup(string backupPath)
        {
            List<Entity> itemsToBackup = new();
            itemsToBackup.AddRange(keyLessEntities);
            using (IUnitOfWork uow = db.CreateUnitOfWork())
            {
                itemsToBackup.AddRange(await uow.GetAllAsync<Budget>());
                itemsToBackup.AddRange(await uow.GetAllAsync<TransactionAttribute>());
                itemsToBackup.AddRange(await uow.GetAllAsync<CurrencyExchangeRate>());
                itemsToBackup.AddRange(await uow.GetAllAsync<Currency>());
                itemsToBackup.AddRange((await uow.GetAllAsync<Location>()).Where(x => x.Id > 0));
                itemsToBackup.AddRange(await uow.GetAllAsync<Payee>());
                itemsToBackup.AddRange((await uow.GetAllAsync<Project>()).Where(x => x.Id > 0));
                itemsToBackup.AddRange(await uow.GetAllAsync<Transaction>());
                itemsToBackup.AddRange((await uow.GetAllAsync<Account>()).OrderBy(x => x.Id));
                itemsToBackup.AddRange((await uow.GetAllAsync<AttributeDefinition>()).Where(x => x.Id > 0));
                itemsToBackup.AddRange(await uow.GetAllAsync<CategoryAttribute>());
                itemsToBackup.AddRange(await uow.GetAllAsync<CCardClosingDate>());
                itemsToBackup.AddRange(await uow.GetAllAsync<SmsTemplate>());
                itemsToBackup.AddRange((await uow.GetAllAsync<Category>()).Where(x => x.Id > 0));
            }

            backupWriter.GenerateBackup(itemsToBackup, backupPath, _backupVersion, _entityColumnsOrder);
        }

        private void AddKeylessEntities<T>(IEnumerable<T> entities)
        where T : Entity
        {
            Logger.Info($"Imported {typeof(T).Name} {entities.Count()}");
            keyLessEntities.AddRange(entities);
        }

        private async void Blotter_AddTransactionRaised(object sender, EventArgs eventArgs)
        {
            await OpenTransactionDialogAsync(0);
        }

        private async void Blotter_AddTransferRaised(object sender, EventArgs eventArgs)
        {
            await OpenTransferDialogAsync(0);
        }

        private async void Blotter_DeleteRaised(object sender, BlotterModel eventArgs)
        {

            if (dialogWrapper.ShowMessageBox("Are you sure you want to delete transaction?", "Delete", true))
            {
                using (var uow = db.CreateUnitOfWork())
                {
                    var repo = uow.GetRepository<Transaction>();
                    var transaction = await repo.FindByAsync(x => x.Id == eventArgs.Id);

                    await repo.DeleteAsync(transaction);
                    await uow.SaveChangesAsync();
                }
                await db.RebuildAccountBalanceAsync(eventArgs.FromAccountId);
                await db.RebuildAccountBalanceAsync(eventArgs.ToAccountId ?? 0);
                await RefreshBlotterTransactionsAsync();
            }
        }

        private async void BlotterVM_OpenTransactionRaised(object sender, BlotterModel eventArgs)
        {
            if (eventArgs.Type == "Transfer")
            {
                await OpenTransferDialogAsync(eventArgs.Id);
            }
            else
            {
                await OpenTransactionDialogAsync(eventArgs.Id);
            }
        }

        private void ClearPages()
        {
            _pages?.Clear();
            if (Blotter != null)
            {
                Blotter.EditRaised -= BlotterVM_OpenTransactionRaised;
                Blotter.AddRaised -= Blotter_AddTransactionRaised;
                Blotter.AddTransferRaised -= Blotter_AddTransferRaised;
                Blotter.DeleteRaised -= Blotter_DeleteRaised;
            }

            Blotter = null;

            if (Locations != null)
            {
                Locations.AddRaised -= Locations_AddRaised;
                Locations.EditRaised -= Locations_EditRaised;
            }
            Locations = null;

            if (Payees != null)
            {
                Payees.AddRaised -= Payees_AddRaised;
                Payees.EditRaised -= Payees_EditRaised;
            }
            Payees = null;

            if (Projects != null)
            {
                Projects.AddRaised -= Projects_AddRaised;
                Projects.EditRaised -= Projects_EditRaised;
            }
            Projects = null;
        }

        private void CreatePages()
        {
            Blotter = new BlotterVM(db);
            Blotter.EditRaised += BlotterVM_OpenTransactionRaised;
            Blotter.AddRaised += Blotter_AddTransactionRaised;
            Blotter.AddTransferRaised += Blotter_AddTransferRaised;
            Blotter.DeleteRaised += Blotter_DeleteRaised;

            Locations = new LocationsVM(db);
            Locations.AddRaised += Locations_AddRaised;
            Locations.EditRaised += Locations_EditRaised;

            Payees = new PayeesVM(db);
            Payees.AddRaised += Payees_AddRaised;
            Payees.EditRaised += Payees_EditRaised;

            Projects = new ProjectsVM(db);
            Projects.AddRaised += Projects_AddRaised;
            Projects.EditRaised += Projects_EditRaised;
        }

        private async Task<BindableBase> GetOrCreatePage(Type type)
        {

            switch (type.Name)
            {
                case nameof(AccountModel):
                    return await GetOrCreatePage<AccountModel, AccountsVM>();
                case nameof(CurrencyModel):
                    return await GetOrCreatePage<CurrencyModel, CurrenciesVM>();
                case nameof(ProjectModel):
                    Projects = await GetOrCreatePage<ProjectModel, ProjectsVM>(
                        addAction: Projects_AddRaised,
                        editAction: Projects_EditRaised);
                    return Projects;
                case nameof(LocationModel):
                    Locations = await GetOrCreatePage<LocationModel, LocationsVM>(
                    addAction: Locations_AddRaised,
                    editAction: Locations_EditRaised);
                    return Locations;
                case nameof(PayeeModel):
                    Payees = await GetOrCreatePage<PayeeModel, PayeesVM>(
                        addAction: Payees_AddRaised,
                        editAction: Payees_EditRaised);
                    return Payees;
                case nameof(BlotterModel):
                    {
                        if (Blotter == null)
                        {
                            Blotter = await GetOrCreatePage<BlotterModel, BlotterVM>(
                                addAction: Blotter_AddTransactionRaised,
                                deleteAction: Blotter_DeleteRaised,
                                editAction: BlotterVM_OpenTransactionRaised
                                );
                        }
                        return Blotter;
                    }
                case nameof(CategoryTreeModel):
                    return await GetOrCreatePage<CategoryTreeModel, CategoriesVM>();
                case nameof(ExchangeRateModel):
                    return await GetOrCreatePage<ExchangeRateModel, ExchangeRatesVM>(
                        addAction: null,
                        deleteAction: null,
                        editAction: null);
                case nameof(ReportsControlVM):
                    {
                        return new ReportsControlVM(db);
                    }

                default: throw new NotSupportedException($"{type.FullName} not suported");
            }
        }

        private async Task<VMType> GetOrCreatePage<TEntity, VMType>(
            EventHandler addAction = null,
            EventHandler<TEntity> deleteAction = null,
            EventHandler<TEntity> editAction = null)
            where VMType : EntityBaseVM<TEntity>
            where TEntity : BaseModel, new()
        {
            var type = typeof(TEntity);
            if (!_pages.ContainsKey(type))
            {
                var vm = Activator.CreateInstance(typeof(VMType), db) as VMType;

                if (addAction != null)
                {
                    vm.AddRaised += addAction;
                }
                if (deleteAction != null)
                {
                    vm.DeleteRaised += deleteAction;
                }
                if (editAction != null)
                {
                    vm.EditRaised += editAction;
                }
                _pages.TryAdd(type, vm);

                await vm.RefreshDataCommand.ExecuteAsync();
            }
            return (VMType)_pages[type];
        }

        private async void Locations_AddRaised(object sender, EventArgs eventArgs)
        {
            await OpenLocationDialogAsync(sender as IDataRefresh, 0);
        }

        private async void Locations_EditRaised(object sender, LocationModel eventArgs)
        {
            await OpenLocationDialogAsync(sender as IDataRefresh, (int)eventArgs.Id);
        }

        private async Task NavigateToType(Type type)
        {
            CurrentPage = await GetOrCreatePage(type);
        }

        private async Task OpenBackup_Click()
        {
            var backupPath = dialogWrapper.OpenFileDialog(Backup);
            if (!string.IsNullOrEmpty(backupPath))
            {
                Logger.Info($"Opened backup : {backupPath}");
                await OpenBackup(backupPath);
            }
        }

        private async Task OpenTagDialogAsync<T>(IDataRefresh sender, int e)
            where T : Tag, new()
        {
            T selectedEntity = await db.GetOrCreateAsync<T>(e);
            TagVM context = new TagVM(new TagDto(selectedEntity));

            var result = dialogWrapper.ShowDialog<TagControl>(context, 180, 300, typeof(T).Name);

            var updatedItem = result as TagDto;
            if (updatedItem != null)
            {
                selectedEntity.IsActive = updatedItem.IsActive;
                selectedEntity.Title = updatedItem.Title;

                await db.InsertOrUpdateAsync(new[] { selectedEntity });
                await sender.RefreshDataCommand.ExecuteAsync();
            }
        }

        private async Task OpenLocationDialogAsync(IDataRefresh sender, int id)
        {
            Location selectedValue = await db.GetOrCreateAsync<Location>(id);
            LocationDialogVM locationVm = new LocationDialogVM(new LocationDto(selectedValue));

            var result = dialogWrapper.ShowDialog<LocationControl>(locationVm, 240, 300, nameof(Location));

            var updatedItem = result as LocationDto;
            if (updatedItem != null)
            {
                selectedValue.IsActive = updatedItem.IsActive;
                selectedValue.Address = updatedItem.Address;
                selectedValue.Title = updatedItem.Title;
                selectedValue.Name = updatedItem.Title;
                if (id == 0)
                {
                    selectedValue.Count = 0;
                }

                await db.InsertOrUpdateAsync(new[] { selectedValue });

                await sender.RefreshDataCommand.ExecuteAsync();
            }
        }

        private async Task OpenMonoWizardAsync(string bank, string fileExtention)
        {
            var fileName = dialogWrapper.OpenFileDialog(fileExtention);
            Logger.Info($"{fileExtention} fileName -> {fileName}");
            if (!string.IsNullOrEmpty(fileName))
            {
                var csvHelper = this.bankFactory.CreateBankHelper(bank);
                var csvTransactions = await csvHelper.ParseReport(fileName);

                var vm = new MonoWizardVM(csvTransactions);

                var output = dialogWrapper.ShowWizard(vm);

                var outputTransactions = output as List<Transaction>;
                if (outputTransactions != null)
                {
                    using var blotter = db.CreateUnitOfWork();
                    var times = outputTransactions.Select(x => x.DateTime).Distinct().ToArray();
                    var transactionRepo = blotter.GetRepository<Transaction>();
                    List<Transaction> accTransactions = await transactionRepo.FindManyAsync(predicate: x => times.Contains(x.DateTime));

                    List<Transaction> monoToImport = outputTransactions.Where(item =>
                    !accTransactions.Any(x =>
                    x.FromAccountId == item.FromAccountId &&
                    x.DateTime == item.DateTime &&
                    x.FromAmount == item.FromAmount)).ToList();

                    var duplicatesCount = outputTransactions.Count - monoToImport.Count;

                    await db.AddTransactionsAsync(monoToImport);

                    await RefreshAccountsAndTransactionsViewModels(monoToImport);

                    this.dialogWrapper.ShowMessageBox(
                        $"Imported {monoToImport.Count} transactions."
                        + ((duplicatesCount > 0) ? $" Skiped {duplicatesCount} duplicates." : string.Empty),
                        $"{bank} Import");

                    Logger.Info($"Imported {monoToImport.Count} transactions. Found duplicates : {duplicatesCount}");
                }
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

            TransactionDialogVM dialogVm = new TransactionDialogVM(transactionDto, dialogWrapper);

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
                await RefreshBlotterTransactionsAsync();
            }
        }

        private async Task OpenTransferDialogAsync(int id)
        {
            Transaction transfer = await db.GetOrCreateTransactionAsync(id);

            TransferDialogVM dialogVm = new TransferDialogVM(new TransferDto(transfer));

            var result = dialogWrapper.ShowDialog<TransferControl>(dialogVm, 385, 340, "Transfer");

            var output = result as TransferDto;
            if (output != null)
            {
                MapperHelper.MapTransfer(result as TransferDto, transfer);
                await db.InsertOrUpdateAsync(new[] { transfer });

                await db.RebuildAccountBalanceAsync(transfer.FromAccountId);
                await db.RebuildAccountBalanceAsync(transfer.ToAccountId);

                await RefreshBlotterTransactionsAsync();

                await RefreshViewModelAsync<AccountModel>(); 
            }
        }

        private async void Payees_AddRaised(object sender, EventArgs eventArgs)
        {
            await OpenTagDialogAsync<Payee>(sender as IDataRefresh, 0);
        }

        private async void Payees_EditRaised(object sender, PayeeModel eventArgs)
        {
            await OpenTagDialogAsync<Payee>(sender as IDataRefresh, (int)eventArgs.Id);
        }

        private async void Projects_AddRaised(object sender, EventArgs eventArgs)
        {
            await OpenTagDialogAsync<Project>(sender as IDataRefresh, 0);
        }

        private async void Projects_EditRaised(object sender, ProjectModel eventArgs)
        {
            await OpenTagDialogAsync<Project>(sender as IDataRefresh, (int)eventArgs.Id);
        }

        private async Task RefreshAccountsAndTransactionsViewModels(List<Transaction> transactions)
        {

            var accountIds = transactions
                .Where(x => x.ToAccountId > 0)
                .Select(x => x.ToAccountId).Union(
                transactions
                .Where(x => x.FromAccountId > 0)
                .Select(x => x.FromAccountId))
                .Distinct();

            foreach (var accId in accountIds)
            {
                await db.RebuildAccountBalanceAsync(accId);
            }

            await RefreshBlotterTransactionsAsync();
            await RefreshViewModelAsync<AccountModel>();
        }
        private async Task RefreshBlotterTransactionsAsync()
        {
            if (Blotter != null)
            {
                await Blotter.RefreshDataCommand.ExecuteAsync();
            }
        }

        private async Task RefreshViewModelAsync<T>()
            where T: BaseModel, new()
        {
            if (_pages.TryGetValue(typeof(T), out var val))
            {
                var vm = val as EntityBaseVM<T>;
                await vm.RefreshDataCommand.ExecuteAsync();
            }
        }

        private async Task SaveBackup_Click()
        {
            var backupPath = dialogWrapper.SaveFileDialog(Backup, Path.Combine(Path.GetDirectoryName(OpenBackupPath), BackupWriter.GenerateFileName()));
            if (!string.IsNullOrEmpty(backupPath))
            {
                await SaveBackup(backupPath);

                dialogWrapper.ShowMessageBox($"Saved {backupPath}", "Backup done.");
                Logger.Info($"Backup done. Saved {backupPath}");
            }
        }

        private async Task SaveBackupAsDb()
        {
            string fileName = Path.ChangeExtension(BackupWriter.GenerateFileName(), "db");
            string defaultPath;
            if (!string.IsNullOrEmpty(OpenBackupPath))
            {
                defaultPath = Path.Combine(Path.GetDirectoryName(OpenBackupPath ?? string.Empty), fileName);
            }
            else
            {
                defaultPath = fileName;
            }

            var backupPath = dialogWrapper.SaveFileDialog("db", defaultPath);
            if (!string.IsNullOrEmpty(backupPath))
            {
                await db.SaveAsFile(backupPath);

                dialogWrapper.ShowMessageBox($"Saved {backupPath}", "Backup done.");
                Logger.Info($"Backup done. Saved {backupPath}");
            }
        }
    }
}
