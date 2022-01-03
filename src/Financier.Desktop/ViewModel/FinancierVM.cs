using Financier.DataAccess.Abstractions;
using Financier.DataAccess.Data;
using Financier.DataAccess.View;
using Financier.Desktop.Views;
using Financier.Adapter;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Financier.Desktop.ViewModel.Dialog;
using Financier.Desktop.Views.Controls;
using Financier.Desktop.Reports.ViewModel;
using Financier.Desktop.Helpers;
using Financier.Desktop.Wizards.MonoWizard.ViewModel;
using System.IO;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using Financier.Desktop.Data;

namespace Financier.Desktop.ViewModel
{
    public class FinancierVM : BindableBase
    {
        private const string Backup = "backup";
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly ICsvHelper csvHelper;
        private readonly IFinancierDatabaseFactory dbFactory;
        private readonly IDialogWrapper dialogWrapper;
        private readonly List<Entity> keyLessEntities = new();
        private BackupVersion _backupVersion;
        private Dictionary<string, List<string>> _entityColumnsOrder;
        private DelegateCommand<Type> _menuNavigateCommand;
        private DelegateCommand _monoCommand;
        private DelegateCommand _openBackupCommand;
        private ConcurrentDictionary<Type, object> _pages = new ConcurrentDictionary<Type, object>();
        private DelegateCommand _saveBackupCommand;
        private AccountsVM accountsVM;
        private IBackupWriter backupWriter;
        private BlotterVM blotter;
        private BindableBase currentPage;
        private IFinancierDatabase db;
        private IEntityReader entityReader;
        private LocationsVM locations;
        private string openBackupPath;
        private PayeesVM payees;
        private ProjectsVM projects;
        public FinancierVM(IDialogWrapper dialogWrapper, IFinancierDatabaseFactory dbFactory, IEntityReader entityReader, IBackupWriter backupWriter, ICsvHelper csvHelper)
        {
            this.dialogWrapper = dialogWrapper;
            this.dbFactory = dbFactory;
            this.entityReader = entityReader;
            this.backupWriter = backupWriter;
            this.csvHelper = csvHelper;
            this.db = dbFactory.CreateDatabase();

            CreatePages();
        }

        public BlotterVM Blotter
        {
            get => blotter;
            private set => SetProperty(ref blotter, value);
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

        public LocationsVM Locations
        {
            get => locations;
            private set => SetProperty(ref locations, value);
        }

        public DelegateCommand<Type> MenuNavigateCommand
        {
            get
            {
                return _menuNavigateCommand ??= new DelegateCommand<Type>(NavigateToType);
            }
        }

        public DelegateCommand MonoCommand
        {
            get
            {
                return _monoCommand ??= new DelegateCommand(OpenMonoWizardAsync);
            }
        }

        public DelegateCommand OpenBackupCommand
        {
            get
            {
                return _openBackupCommand ??= new DelegateCommand(OpenBackup_OnClick);
            }
        }

        public string OpenBackupPath
        {
            get => openBackupPath;
            private set => SetProperty(ref openBackupPath, value);
        }

        public PayeesVM Payees
        {
            get => payees;
            private set => SetProperty(ref payees, value);
        }

        public ProjectsVM Projects
        {
            get => projects;
            private set => SetProperty(ref projects, value);
        }
        public DelegateCommand SaveBackupCommand
        {
            get
            {
                return _saveBackupCommand ??= new DelegateCommand(SaveBackup_Click);
            }
        }

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
            NavigateToType(typeof(BlotterTransactions));
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

        private async void Blotter_DeleteRaised(object sender, TransactionsView eventArgs)
        {

            if (dialogWrapper.ShowMessageBox("Are you sure you want to delete transaction?", "Delete", true))
            {
                using (var uow = db.CreateUnitOfWork())
                {
                    var repo = uow.GetRepository<Transaction>();
                    var transaction = await repo.FindByAsync(x => x.Id == eventArgs._id);

                    await repo.DeleteAsync(transaction);
                    await uow.SaveChangesAsync();
                }
                await RefreshBlotterTransactionsAsync();
            }
        }

        private async void BlotterVM_OpenTransactionRaised(object sender, TransactionsView eventArgs)
        {
            if (eventArgs.from_account_id > 0 && eventArgs.to_account_id > 0 && eventArgs.category_id == 0)
            {
                await OpenTransferDialogAsync(eventArgs._id);
            }
            else
            {
                await OpenTransactionDialogAsync(eventArgs._id);
            }
        }

        private void ClearPages()
        {
            _pages?.Clear();
            accountsVM = null;
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
            accountsVM = new AccountsVM(Array.Empty<Account>());

            Blotter = new BlotterVM(Array.Empty<BlotterTransactions>());
            Blotter.EditRaised += BlotterVM_OpenTransactionRaised;
            Blotter.AddRaised += Blotter_AddTransactionRaised;
            Blotter.AddTransferRaised += Blotter_AddTransferRaised;
            Blotter.DeleteRaised += Blotter_DeleteRaised;

            Locations = new LocationsVM(Array.Empty<Location>());
            Locations.AddRaised += Locations_AddRaised;
            Locations.EditRaised += Locations_EditRaised;

            Payees = new PayeesVM(Array.Empty<Payee>());
            Payees.AddRaised += Payees_AddRaised;
            Payees.EditRaised += Payees_EditRaised;

            Projects = new ProjectsVM(Array.Empty<Project>());
            Projects.AddRaised += Projects_AddRaised;
            Projects.EditRaised += Projects_EditRaised;
        }
        private async Task<BindableBase> GetOrCreatePage(Type type)
        {

            switch (type.Name)
            {
                case nameof(Account):
                    accountsVM = await GetOrCreatePage<Account, AccountsVM>(
                            transform: (a) => a.OrderByDescending(x => x.IsActive).ThenBy(x => x.SortOrder).ToList(),
                            includes: x => x.Currency);
                    return accountsVM;
                case nameof(Budget):
                    return await GetOrCreatePage<Budget, BudgetsVM>();
                case nameof(Currency):
                    return await GetOrCreatePage<Currency, CurrenciesVM>();
                case nameof(Project):
                    Projects = await GetOrCreatePage<Project, ProjectsVM>(transform: x => x.DefaultOrder(),
                        addAction: Projects_AddRaised,
                        editAction: Projects_EditRaised);
                    return Projects;
                case nameof(Location):
                    Locations = await GetOrCreatePage<Location, LocationsVM>(transform: x => x.DefaultOrder(),
                    addAction: Locations_AddRaised,
                    editAction: Locations_EditRaised);
                    return Locations;
                case nameof(Payee):
                    Payees = await GetOrCreatePage<Payee, PayeesVM>(transform: x => x.DefaultOrder(),
                        addAction: Payees_AddRaised,
                        editAction: Payees_EditRaised);
                    return Payees;
                case nameof(BlotterTransactions):
                    {
                        if (Blotter == null)
                        {
                            Blotter = await GetOrCreatePage<BlotterTransactions, BlotterVM>(
                                transform: x => x.OrderByDescending(x => x.datetime),
                                addAction: Blotter_AddTransactionRaised,
                                deleteAction: Blotter_DeleteRaised,
                                editAction: BlotterVM_OpenTransactionRaised,
                                x => x.from_account_currency, x => x.to_account_currency);
                        }
                        return Blotter;
                    }
                case nameof(Category):
                    return await GetOrCreatePage<Category, CategoriesVM>(transform: x => x.Where(x => x.Id > 0).OrderBy(x => x.Left));

                case nameof(ByCategoryReport):
                    {
                        using var uow = db.CreateUnitOfWork();
                        var allCategories = await uow.GetAllAsync<Category>();
                        var orderedCategories = allCategories.Where(x => x.Id > 0).OrderBy(x => x.Left).ToList();
                        var byCategoryReport = await uow.GetAllAsync<ByCategoryReport>(x => x.from_account_currency, x => x.to_account_currency, x => x.category);
                        return new ReportVM(byCategoryReport, orderedCategories);
                    }
                case nameof(CurrencyExchangeRate):
                    return await GetOrCreatePage<CurrencyExchangeRate, ExchangeRatesVM>(transform: null,
                        addAction: null,
                        deleteAction: null,
                        editAction: null,
                        x => x.FromCurrency, x => x.ToCurrency);

                default: throw new NotSupportedException($"{type.FullName} not suported");
            }
        }

        private async Task<VMType> GetOrCreatePage<TEntity, VMType>(
            Func<IEnumerable<TEntity>, IEnumerable<TEntity>> transform = null,
            EventHandler addAction = null,
            EventHandler<TEntity> deleteAction = null,
            EventHandler<TEntity> editAction = null,
            params Expression<Func<TEntity, object>>[] includes)
            where VMType : EntityBaseVM<TEntity>
            where TEntity : Entity
        {
            var type = typeof(TEntity);
            if (!_pages.ContainsKey(type))
            {
                using var uow = db.CreateUnitOfWork();
                IEnumerable<TEntity> items = await uow.GetAllAsync(includes);

                if (transform != null)
                {
                    items = transform(items);
                }

                var vm = Activator.CreateInstance(typeof(VMType), items) as VMType;

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
            }
            return (VMType)_pages[type];
        }

        private async void Locations_AddRaised(object sender, EventArgs eventArgs)
        {
            await OpenLocationDialogAsync(0);
        }

        private async void Locations_EditRaised(object sender, Location eventArgs)
        {
            await OpenLocationDialogAsync(eventArgs.Id);
        }

        private async void NavigateToType(Type type)
        {
            CurrentPage = await GetOrCreatePage(type);
        }

        private async void OpenBackup_OnClick()
        {
            var backupPath = dialogWrapper.OpenFileDialog(Backup);
            if (!string.IsNullOrEmpty(backupPath))
            {
                Logger.Info($"Opened backup : {backupPath}");
                await OpenBackup(backupPath);
            }
        }

        private async Task OpenEntityWithTitleDialogAsync<T>(int e)
            where T : Entity, IActive, new()
        {
            T selectedEntity = await db.GetOrCreateAsync<T>(e);
            EntityWithTitleVM context = new EntityWithTitleVM(new EntityWithTitleDTO(selectedEntity));

            var result = dialogWrapper.ShowDialog<EntityWithTitleControl>(context, 180, 300, typeof(T).Name);

            if (result is EntityWithTitleDTO)
            {
                var updatedItem = (EntityWithTitleDTO)result;
                selectedEntity.IsActive = updatedItem.IsActive;
                selectedEntity.Title = updatedItem.Title;

                await db.InsertOrUpdateAsync(new[] { selectedEntity });
                await RefreshEntitiesAsync<T>();
            }
        }

        private async Task OpenLocationDialogAsync(int id)
        {
            Location selectedValue = await db.GetOrCreateAsync<Location>(id);
            LocationDialogVM locationVm = new LocationDialogVM(new LocationDTO(selectedValue));

            var result = dialogWrapper.ShowDialog<LocationControl>(locationVm, 240, 300, nameof(Location));

            if (result is LocationDTO)
            {
                var updatedItem = (LocationDTO)result;
                selectedValue.IsActive = updatedItem.IsActive;
                selectedValue.Address = updatedItem.Address;
                selectedValue.Title = updatedItem.Title;
                selectedValue.Name = updatedItem.Title;
                if (id == 0)
                {
                    selectedValue.Count = 0;
                }

                await db.InsertOrUpdateAsync(new[] { selectedValue });

                // TODO : позбутись використання даних з таблиць, перейти на DTO
                await RefreshEntitiesAsync<Location>();
            }
        }

        private async void OpenMonoWizardAsync()
        {
            var fileName = dialogWrapper.OpenFileDialog("csv");
            Logger.Info($"csv fileName -> {fileName}");
            if (!string.IsNullOrEmpty(fileName))
            {
                using var uow = db.CreateUnitOfWork();
                var accounts = await uow.GetAllOrderedByDefaultAsync<Account>();
                var currencies = await uow.GetAllAsync<Currency>();
                var locations = await uow.GetAllOrderedByDefaultAsync<Location>();
                var categories = await uow.GetAllAsync<Category>();
                var projects = await uow.GetAllOrderedByDefaultAsync<Project>();

                var csvTransactions = await csvHelper.ParseCsv(fileName);

                var vm = new MonoWizardVM(csvTransactions, accounts, currencies, locations, categories.OrderBy(x => x.Left), projects);

                var output = dialogWrapper.ShowWizard(vm);

                if (output is List<Transaction>)
                {
                    var outputTransactions = output as List<Transaction>;
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
                        "Monobank CSV Import");

                    Logger.Info($"Imported {monoToImport.Count} transactions. Found duplicates : {duplicatesCount}");
                }
            }
        }

        private async Task OpenTransactionDialogAsync(int id)
        {
            Transaction transaction = await db.GetOrCreateTransactionAsync(id);
            IEnumerable<Transaction> subTransactions = await db.GetSubTransactionsAsync(id);
            var transactionDto = new TransactionDTO(transaction, subTransactions);

            using var uow = db.CreateUnitOfWork();

            TransactionDialogVM dialogVm = new TransactionDialogVM(
                transactionDto,
                dialogWrapper,
                (await uow.GetAllAsync<Category>()).OrderBy(x => x.Left).ToList(),
                await uow.GetAllOrderedByDefaultAsync<Project>(),
                await uow.GetAllOrderedByDefaultAsync<Account>(x => x.Currency),
                await uow.GetAllAsync<Currency>(),
                await uow.GetAllOrderedByDefaultAsync<Location>(),
                await uow.GetAllOrderedByDefaultAsync<Payee>());

            var result = dialogWrapper.ShowDialog<TransactionControl>(dialogVm, 640, 340, nameof(Transaction));

            if (result is TransactionDTO)
            {
                var resultVm = result as TransactionDTO;
                var resultTransactions = new List<Transaction>();

                MapperHelper.MapTransaction(resultVm, transaction);

                resultTransactions.Add(transaction);
                if (resultVm?.SubTransactions?.Any() == true)
                {
                    // TODO : Add Unit Test gor code below
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
                        resultTransactions.Add(subTransaction);
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

            using var uow = db.CreateUnitOfWork();
            var accounts = await uow.GetAllOrderedByDefaultAsync<Account>(x => x.Currency);
            TransferDialogVM dialogVm = new TransferDialogVM(new TransferDTO(transfer), accounts);

            var result = dialogWrapper.ShowDialog<TransferControl>(dialogVm, 385, 340, "Transfer");

            if (result is TransferDTO)
            {
                MapperHelper.MapTransfer(result as TransferDTO, transfer);
                await db.InsertOrUpdateAsync(new[] { transfer });

                await db.RebuildAccountBalanceAsync(transfer.FromAccountId);
                await db.RebuildAccountBalanceAsync(transfer.ToAccountId);

                await RefreshBlotterTransactionsAsync();
                await RefreshEntitiesAsync<Account>();
            }
        }

        private async void Payees_AddRaised(object sender, EventArgs eventArgs)
        {
            await OpenEntityWithTitleDialogAsync<Payee>(0);
        }

        private async void Payees_EditRaised(object sender, Payee eventArgs)
        {
            await OpenEntityWithTitleDialogAsync<Payee>(eventArgs.Id);
        }

        private async void Projects_AddRaised(object sender, EventArgs eventArgs)
        {
            await OpenEntityWithTitleDialogAsync<Project>(0);
        }

        private async void Projects_EditRaised(object sender, Project eventArgs)
        {
            await OpenEntityWithTitleDialogAsync<Project>(eventArgs.Id);
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
            await RefreshEntitiesAsync<Account>();
        }
        private async Task RefreshBlotterTransactionsAsync()
        {
            using var uow = db.CreateUnitOfWork();
            var allTransactions = await uow.GetAllAsync<BlotterTransactions>(x => x.from_account_currency, x => x.to_account_currency);
            blotter.Entities = new ObservableCollection<BlotterTransactions>(allTransactions.OrderByDescending(x => x.datetime));
        }

        private async Task RefreshEntitiesAsync<T>()
            where T: Entity, IActive
        {
            using var uow = db.CreateUnitOfWork();
            var entities = await uow.GetAllOrderedByDefaultAsync<T>();
            if (_pages.TryGetValue(typeof(T), out var val))
            {
                var vm = val as EntityBaseVM<T>;
                vm.Entities = new ObservableCollection<T>(entities);
            }
        }
        private async void SaveBackup_Click()
        {
            var backupPath = dialogWrapper.SaveFileDialog(Backup, Path.Combine(Path.GetDirectoryName(OpenBackupPath), BackupWriter.GenerateFileName()));
            if (!string.IsNullOrEmpty(backupPath))
            {
                await SaveBackup(backupPath);

                dialogWrapper.ShowMessageBox($"Saved {backupPath}", "Backup done.");
                Logger.Info($"Backup done. Saved {backupPath}");
            }
        }
    }
}
