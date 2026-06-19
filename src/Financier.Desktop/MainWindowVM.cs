using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Financier.Common.Localization;
using Financier.Adapter;
using Financier.Common;
using Financier.Common.Entities;
using Financier.Common.Model;
using Financier.Converters;
using Financier.DataAccess.Abstractions;
using Financier.DataAccess.Data;
using Financier.DataAccess.Utils;
using Financier.Desktop.Data;
using Financier.Desktop.Helpers;
using Financier.Desktop.Helpers.BankHelper;
using Financier.Desktop.Pages.Dialogs;
using Financier.Desktop.Services;
using Financier.Desktop.ViewModel.Dialog;
using Financier.Desktop.Wizards;
using Financier.Desktop.Wizards.MonoWizard.ViewModel;
using Financier.Reports;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Prism.Mvvm;
using IAsyncCommand = Financier.Common.IAsyncCommand;

namespace Financier.Desktop.ViewModel
{
    public class MainWindowVM : BindableBase
    {
        private const string Backup = "backup";
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly ConcurrentDictionary<Type, BindableBase> _pages = new ConcurrentDictionary<Type, BindableBase>();
        private readonly IBankHelperFactory bankFactory;
        private readonly IFinancierDatabaseFactory dbFactory;
        private readonly IDialogWrapper dialogWrapper;
        private readonly List<Entity> keyLessEntities = new();
        private readonly IToastNotifierWrapper notifier;
        private readonly UpdateService updateService;
        private BackupVersion _backupVersion;
        private Dictionary<string, List<string>> _entityColumnsOrder;
        private IAsyncCommand<Type> _menuNavigateCommand;
        private IAsyncCommand<WizardTypes> _importCommand;
        private IAsyncCommand _openBackupCommand;
        private IAsyncCommand _saveBackupCommand;
        private IAsyncCommand _saveBackupAsDbCommand;
        private IAsyncCommand _settingsCommand;
        private IAsyncCommand _refreshExchangeRatesCommand;
        private IAsyncCommand<bool> _checkForUpdateCommand;
        private readonly IBackupWriter backupWriter;
        private BlotterVM blotterVm;
        private BindableBase currentPage;
        private IFinancierDatabase db;
        private readonly IEntityReader entityReader;
        private LocationsVM locationsVm;
        private string openBackupPath;
        private string defaultBackupDirectory;
        private bool isLoading;
        private PayeesVM payeesVm;
        private ProjectsVM projectsVm;
        private RulesVM rulesVm;

        public MainWindowVM(IDialogWrapper dialogWrapper,
            IFinancierDatabaseFactory dbFactory,
            IEntityReader entityReader,
            IBackupWriter backupWriter,
            IToastNotifierWrapper notifier,
            IBankHelperFactory bankFactory,
            UpdateService updateService)
        {
            this.dialogWrapper = dialogWrapper;
            this.dbFactory = dbFactory;
            this.entityReader = entityReader;
            this.backupWriter = backupWriter;
            this.notifier = notifier;
            this.bankFactory = bankFactory;
            this.updateService = updateService;
            db = dbFactory.CreateDatabase();

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
                SetProperty(ref currentPage, value, nameof(CurrentPage));
                Logger.Info($"CurrentPage -> {value?.GetType().FullName}");
                RaisePropertyChanged(nameof(IsTransactionPageSelected));
                RaisePropertyChanged(nameof(IsLocationPageSelected));
                RaisePropertyChanged(nameof(IsProjectPageSelected));
                RaisePropertyChanged(nameof(IsPayeePageSelected));
                RaisePropertyChanged(nameof(IsRulesPageSelected));
                RaisePropertyChanged(nameof(IsExchangeRatesPageSelected));
            }
        }

        public bool IsLocationPageSelected => currentPage is LocationsVM;

        public bool IsPayeePageSelected => currentPage is PayeesVM;

        public bool IsProjectPageSelected => currentPage is ProjectsVM;

        public bool IsTransactionPageSelected => currentPage is BlotterVM;

        public bool IsRulesPageSelected => currentPage is RulesVM;

        public bool IsExchangeRatesPageSelected => currentPage is ExchangeRatesVM;

        public string OpenBackupPath
        {
            get => openBackupPath;
            private set => SetProperty(ref openBackupPath, value);
        }
        public string DefaultBackupDirectory
        {
            get => defaultBackupDirectory;
            internal set => SetProperty(ref defaultBackupDirectory, value);
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

        public RulesVM Rules
        {
            get => rulesVm;
            private set => SetProperty(ref rulesVm, value);
        }

        public bool IsLoading
        {
            get => isLoading;
            private set => SetProperty(ref isLoading, value);
        }

        public IAsyncCommand<Type> MenuNavigateCommand => _menuNavigateCommand ??= new AsyncCommand<Type>(NavigateToType);

        public IAsyncCommand<WizardTypes> ImportCommand => _importCommand ??= new AsyncCommand<WizardTypes>(OpenImportWizardAsync);

        public IAsyncCommand OpenBackupCommand => _openBackupCommand ??= new AsyncCommand(OpenBackup_Click);

        public IAsyncCommand SaveBackupCommand => _saveBackupCommand ??= new AsyncCommand(SaveBackup_Click);

        public IAsyncCommand SaveBackupAsDbCommand => _saveBackupAsDbCommand ??= new AsyncCommand(SaveBackupAsDb);

        public IAsyncCommand SettingsCommand => _settingsCommand ??= new AsyncCommand(Settings_Click);
        public IAsyncCommand RefreshExchangeRatesCommand => _refreshExchangeRatesCommand ??= new AsyncCommand(RefreshExchangeRates_Click);
        public IAsyncCommand<bool> CheckForUpdateCommand => _checkForUpdateCommand ??= new AsyncCommand<bool>(CheckForUpdatesAsync);

        public async Task OpenBackup(string backupPath)
        {
            try
            {
                OpenBackupPath = backupPath;
                IsLoading = true;
                ClearPages();

                var (entities, backupVersion, columnsOrder) = await entityReader.ParseBackupFileAsync(backupPath);
                _backupVersion = backupVersion;
                _entityColumnsOrder = columnsOrder;

                db?.Dispose();
                db = dbFactory.CreateDatabase();
                await db.ImportEntitiesAsync(entities);

                keyLessEntities.Clear();

                AddKeylessEntities(entities.OfType<CCardClosingDate>());
                AddKeylessEntities(entities.OfType<CategoryAttribute>());
                AddKeylessEntities(entities.OfType<TransactionAttribute>());

                IsLoading = false;

                DbManual.ResetAllDatabaseManuals();
                await DbManual.SetupAsync(db);
                await DbManual.LoadRulesAsync();

                await NavigateToType(typeof(BlotterModel));

                notifier?.ShowMessage(string.Format(LocalizationService.Instance.entities_loaded, entities?.Count()));

                if (SettingsService.Current.Settings.ExchangeRates.UpdateOnStart)
                {
                    await RefreshExchangeRatesCommand.ExecuteAsync();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ex.ToString());
                IsLoading = false;
                throw;
            }
        }

        public async Task SaveBackup(string backupPath)
        {
            List<Entity> itemsToBackup = [.. keyLessEntities];
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

        private void ClearPages()
        {
            _pages?.Clear();
            Blotter = null;
            Locations = null;
            Payees = null;
            Projects = null;
            Rules = null;
        }

        private void CreatePages()
        {
            Blotter = new BlotterVM(db, dialogWrapper);
            Locations = new LocationsVM(db, dialogWrapper);
            Payees = new PayeesVM(db, dialogWrapper);
            Projects = new ProjectsVM(db, dialogWrapper);
            Rules = new RulesVM(db, dialogWrapper);

            _pages.TryAdd(typeof(BlotterModel), Blotter);
            _pages.TryAdd(typeof(LocationModel), Locations);
            _pages.TryAdd(typeof(PayeeModel), Payees);
            _pages.TryAdd(typeof(ProjectModel), Projects);
            _pages.TryAdd(typeof(RuleModel), Rules);
        }

        private BindableBase GetOrCreatePage(Type type)
        {
            switch (type.Name)
            {
                case nameof(AccountModel):
                    return GetOrCreatePage<AccountModel, AccountsVM>();
                case nameof(CurrencyModel):
                    return GetOrCreatePage<CurrencyModel, CurrenciesVM>();
                case nameof(ProjectModel):
                    return Projects ??= GetOrCreatePage<ProjectModel, ProjectsVM>();
                case nameof(LocationModel):
                    return Locations ??= GetOrCreatePage<LocationModel, LocationsVM>();
                case nameof(PayeeModel):
                    return Payees ??= GetOrCreatePage<PayeeModel, PayeesVM>();
                case nameof(BlotterModel):
                    return Blotter ??= GetOrCreatePage<BlotterModel, BlotterVM>();
                case nameof(CategoryTreeModel):
                    return GetOrCreatePage<CategoryTreeModel, CategoriesVM>();
                case nameof(ExchangeRateModel):
                    return GetOrCreatePage<ExchangeRateModel, ExchangeRatesVM>();
                case nameof(RuleModel):
                    return Rules ??= GetOrCreatePage<RuleModel, RulesVM>();
                case nameof(ReportsControlVM):
                    {
                        if (!_pages.ContainsKey(type))
                        {
                            _pages.TryAdd(type, new ReportsControlVM(db));
                        }
                        return _pages[type];
                    }

                default: throw new NotSupportedException($"{type.FullName} not supported");
            }
        }

        private VMType GetOrCreatePage<TEntity, VMType>()
            where VMType : EntityBaseVM<TEntity>
            where TEntity : BaseModel, new()
        {
            var type = typeof(TEntity);
            if (!_pages.ContainsKey(type))
            {
                var viewModel = Activator.CreateInstance(typeof(VMType), db, dialogWrapper) as VMType;

                _pages.TryAdd(type, viewModel!);
            }

            return (VMType)_pages[type];
        }

        private async Task NavigateToType(Type type)
        {
            CurrentPage = GetOrCreatePage(type);
            await RefreshCurrentPage();
        }

        private async Task OpenBackup_Click()
        {
            if (IsLoading) return;
            var backupPath = dialogWrapper.OpenFileDialog(Backup);
            if (!string.IsNullOrEmpty(backupPath))
            {
                Logger.Info($"Opened backup : {backupPath}");
                await OpenBackup(backupPath);
            }
        }

        private async Task OpenImportWizardAsync(WizardTypes bankType)
        {
            var fileExtension = bankType.GetEnumDescription();
            var fileName = dialogWrapper.OpenFileDialog(fileExtension);
            Logger.Info($"{fileExtension} fileName -> {fileName}");
            if (!string.IsNullOrEmpty(fileName))
            {
                var importHelper = this.bankFactory.CreateBankHelper(bankType);
                var sourceData = importHelper.ParseReport(fileName);

                Dictionary<int, BlotterModel> lastTransactions = new();
                foreach (var acc in DbManual.Account.Where(x => x.Id.HasValue))
                {
                    var last = Blotter.Entities.FirstOrDefault(x => x.Id == acc.LastTransactionId);
                    lastTransactions.Add(acc.Id!.Value, last!);
                }

                var vm = new MonoWizardVM(importHelper.BankTitle, sourceData, lastTransactions, dialogWrapper);

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

                    await RefreshAffectedAccounts(monoToImport);
                    await RefreshCurrentPage();

                    var message = duplicatesCount > 0
                        ? string.Format(LocalizationService.Instance.import_result_with_duplicates, monoToImport.Count, duplicatesCount)
                        : string.Format(LocalizationService.Instance.import_result, monoToImport.Count);

                    this.notifier.ShowMessage(string.Format(message, $"{importHelper.BankTitle} {LocalizationService.Instance.import}"));

                    Logger.Info($"Imported {monoToImport.Count} transactions. Found duplicates : {duplicatesCount}");
                }
            }
        }

        private async Task RefreshAffectedAccounts(List<Transaction> transactions)
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

            DbManual.ResetManuals(nameof(DbManual.Account));
            await DbManual.SetupAsync(db);
        }

        private async Task RefreshCurrentPage()
        {
            var page = CurrentPage as IDataRefresh;
            if (page != null)
            {
                await page.RefreshDataCommand.ExecuteAsync();
            }
        }

        private async Task SaveBackup_Click()
        {
            string defaultPath = string.IsNullOrEmpty(OpenBackupPath)
                ? BackupWriter.GenerateFileName()
                : Path.Combine(Path.GetDirectoryName(OpenBackupPath), BackupWriter.GenerateFileName());
            var backupPath = dialogWrapper.SaveFileDialog(Backup, defaultPath);
            if (!string.IsNullOrEmpty(backupPath))
            {
                await SaveBackup(backupPath);

                dialogWrapper.ShowMessageBox(string.Format(LocalizationService.Instance.saved_message, backupPath), LocalizationService.Instance.backup_done);
                Logger.Info($"Backup done. Saved {backupPath}");
            }
        }

        private async Task SaveBackupAsDb()
        {
            string fileName = Path.ChangeExtension(BackupWriter.GenerateFileName(), "db");
            string defaultPath;
            if (!string.IsNullOrEmpty(OpenBackupPath))
            {
                defaultPath = Path.Combine(Path.GetDirectoryName(OpenBackupPath ?? string.Empty)!, fileName);
            }
            else
            {
                defaultPath = fileName;
            }

            var backupPath = dialogWrapper.SaveFileDialog("db", defaultPath);
            if (!string.IsNullOrEmpty(backupPath))
            {
                await db.SaveAsFile(backupPath);

                dialogWrapper.ShowMessageBox(string.Format(LocalizationService.Instance.saved_message, backupPath), LocalizationService.Instance.backup_done);
                Logger.Info($"Backup done. Saved {backupPath}");
            }
        }

        private async Task Settings_Click()
        {
            SettingsDto settings = SettingsService.Current.Settings.Clone() as SettingsDto;

            DialogBaseVM vm = new SettingsVM(settings);
            var updated = dialogWrapper.ShowDialog<SettingsControl>(vm, 300, 400, LocalizationService.Instance.settings) as SettingsDto;

            if (updated != null)
            {
                Language before = SettingsService.Current.Settings.General.Language;

                SettingsService.Current.Settings = updated;
                SettingsService.Current.Save();

                if (before != updated.General.Language)
                {
                    LocalizationService.Instance.ApplyLanguage(updated.General.Language);

                    DbManual.ResetManuals(nameof(DbManual.MCCEnums));
                    DbManual.ResetManuals(nameof(DbManual.MCCTitles));
                    DbManual.ResetManuals(nameof(DbManual.Currencies));
                    await DbManual.SetupAsync(db);
                }
            }
        }

        private async Task RefreshExchangeRates_Click()
        {
            var erSettings = SettingsService.Current.Settings.ExchangeRates;

            if (erSettings.Provider != ExchangeRatesProviders.None)
            {
                var exchangeRateLoader = new ExchangeRateLoader();
                List<CurrencyExchangeRate> exchangeRates = new List<CurrencyExchangeRate>();

                switch (erSettings.Provider)
                {
                    case ExchangeRatesProviders.FreeCurrencyRates:
                        exchangeRates = await exchangeRateLoader.LoadFreeCurrencyRates();
                        break;
                    case ExchangeRatesProviders.OpenExchangeRates:
                        exchangeRates = await exchangeRateLoader.LoadOpenExchangeRates(erSettings.OpenExchangeRatesProviderAppId);
                        break;
                    case ExchangeRatesProviders.Monobank:
                        exchangeRates = await exchangeRateLoader.LoadMonobankRates();
                        break;
                }

                if (exchangeRates.Any())
                {
                    using var uow = db.CreateUnitOfWork();
                    var currencyExchangeRepo = uow.GetRepository<CurrencyExchangeRate>();
                    await currencyExchangeRepo.AddRangeAsync(exchangeRates);
                    try
                    {
                        await uow.SaveChangesAsync();
                    }
                    catch (DbUpdateException ex)
                    {
                        string msg = ex?.InnerException?.Message!;
                        if (!string.IsNullOrEmpty(msg) && msg.Contains("UNIQUE constraint failed", StringComparison.OrdinalIgnoreCase))
                        {
                            notifier?.ShowWarning(LocalizationService.Instance.exchange_rates_exist);
                        }
                        else
                        {
                            notifier?.ShowWarning(LocalizationService.Instance.exchange_rates_not_updated);
                        }
                        return;
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex, "Error saving exchange rates to database.");
                        notifier?.ShowWarning(LocalizationService.Instance.exchange_rates_not_updated);
                        return;
                    }

                    notifier?.ShowMessage(string.Format(LocalizationService.Instance.exchange_rates_updated, erSettings.Provider));
                }
            }
            else
            {
                notifier?.ShowWarning(LocalizationService.Instance.exchange_rates_provider_not_configured);
            }
        }

        private async Task CheckForUpdatesAsync(bool showMessageIfLatest = true)
        {
            try
            {
                if(updateService == null)
                    return;

                var updateVersion = await updateService.CheckForUpdatesAsync();
                if (updateVersion is null)
                {
                    if (showMessageIfLatest)
                    {
                        notifier.ShowMessage(LocalizationService.Instance.latest_version);
                    }
                    return;
                }

                var result = dialogWrapper.ShowMessageBox(
                   LocalizationService.Instance.update_available_question,
                   string.Format(LocalizationService.Instance.update_available, updateVersion),
                   true);

                if (result)
                {

                    notifier.ShowMessage(string.Format(LocalizationService.Instance.downloading_update,
                        "Financier.Desktop",
                        updateVersion));

                    await updateService.PrepareUpdateAsync(updateVersion);

                    notifier.ShowMessage(LocalizationService.Instance.update_downloaded);
                    await Task.Delay(3000);
                    updateService.FinalizeUpdate(true);
                    await Task.Delay(3000);
                    Environment.Exit(0);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ex.ToString());
                notifier.ShowWarning(LocalizationService.Instance.update_failed);
            }
        }
    }
}
