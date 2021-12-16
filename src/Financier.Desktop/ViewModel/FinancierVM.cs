using Financier.DataAccess;
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
using System.Windows.Forms;
using Financier.Desktop.Wizards.MonoWizard.ViewModel;
using System.IO;

namespace Financier.Desktop.ViewModel
{
    public class FinancierVM : BindableBase
    {
        private const string Backup = "backup";
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly List<Entity> keyLessEntities = new();
        private DelegateCommand<Type> _menuNavigateCommand;
        private DelegateCommand _monoCommand;
        private DelegateCommand _openBackupCommand;
        private DelegateCommand _saveBackupCommand;
        private BackupVersion _backupVersion;
        private Dictionary<string, List<string>> _entityColumnsOrder;

        private ReadOnlyCollection<BindableBase> _pages;
        private AccountsVM accountsVm;
        private BlotterVM blotter;
        private LocationsVM locations;
        private BindableBase currentPage;
        private FinancierDatabase db;
        private string openBackupPath;

        private string saveBackupPath;
        private ProjectsVM projects;
        private PayeesVM payees;

        private readonly IDialogWrapper dialogWrapper;

        public FinancierVM(IDialogWrapper dialogWrapper)
        {
            this.dialogWrapper = dialogWrapper;
            CreatePages();
        }

        public BlotterVM Blotter
        {
            get => blotter;
            private set => SetProperty(ref blotter, value);
        }

        public LocationsVM Locations
        {
            get => locations;
            private set => SetProperty(ref locations, value);
        }

        public ProjectsVM Projects
        {
            get => projects;
            private set => SetProperty(ref projects, value);
        }

        public PayeesVM Payees
        {
            get => payees;
            private set => SetProperty(ref payees, value);
        }

        public BindableBase CurrentPage
        {
            get => currentPage;
            set
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
        public bool IsTransactionPageSelected => currentPage is BlotterVM;

        public bool IsLocationPageSelected => currentPage is LocationsVM;

        public bool IsProjectPageSelected => currentPage is ProjectsVM;

        public bool IsPayeePageSelected => currentPage is PayeesVM;

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
                return _monoCommand ??= new DelegateCommand(OpenMonoWizard);
            }
        }

        public DelegateCommand OpenBackupCommand
        {
            get
            {
                return _openBackupCommand ??= new DelegateCommand(OpenBackup_OnClick);
            }
        }

        public DelegateCommand SaveBackupCommand
        {
            get
            {
                return _saveBackupCommand ??= new DelegateCommand(SaveBackup_Click);
            }
        }

        public string OpenBackupPath
        {
            get => openBackupPath;
            set => SetProperty(ref openBackupPath, value);
        }
        public ReadOnlyCollection<BindableBase> Pages => _pages;

        public string SaveBackupPath
        {
            get => saveBackupPath;
            set => SetProperty(ref saveBackupPath, value);
        }

        public async Task ImportMonoTransactions(List<Transaction> transactions)
        {
            await db.AddTransactionsAsync(transactions);

            var accountIds = transactions
                .Where(x => x.ToAccountId > 0)
                .Select(x => x.ToAccountId).Union(
                transactions
                .Where(x => x.FromAccountId > 0)
                .Select(x => x.FromAccountId))
                .Distinct();

            foreach (var accId in accountIds)
            {
                await db.RebuildRunningBalanceForAccount(accId);
            }

            await RefreshBlotterTransactions();
            await RefreshEntities<Account>();
        }

        public async Task OpenBackup(string backupPath)
        {
            var start = DateTime.Now;
            CreatePages();
            var sb = new StringBuilder();

            OpenBackupPath = backupPath;
            var (entities, backupVersion, columnsOrder) = EntityReader.ParseBackupFile(backupPath);
            _backupVersion = backupVersion;
            _entityColumnsOrder = columnsOrder;

            db?.Dispose();

            db = new FinancierDatabase();
            await db.ImportEntities(entities);

            using (var uow = db.CreateUnitOfWork())
            {
                var allAccounts = await uow.GetRepository<Account>()
                    .GetAllAsync(x => x.Currency);

                var allTransactions = await uow.GetRepository<BlotterTransactions>()
                    .GetAllAsync(x => x.from_account_currency, x => x.to_account_currency);

                var byCategoryReport = await uow.GetRepository<ByCategoryReport>()
                    .GetAllAsync(x => x.from_account_currency, x => x.to_account_currency, x => x.category);

                var allCategories = await uow.GetRepository<Category>().GetAllAsync();

                var allRates = await uow.GetRepository<CurrencyExchangeRate>()
                    .GetAllAsync(x => x.FromCurrency, x => x.ToCurrency);

                AddEntities(allAccounts.OrderByDescending(x => x.IsActive).ThenBy(x => x.SortOrder).ToList(), sb);
                AddEntities(allTransactions.OrderByDescending(x => x.datetime).ToList(), sb, true);

                var orderedCategories = allCategories.Where(x => x.Id > 0).OrderBy(x => x.Left).ToList();
                AddEntities(orderedCategories, sb);
                AddEntities(allRates, sb);

                sb?.AppendLine($"Imported {typeof(ReportVM).Name}");
                var vm = _pages.OfType<ReportVM>().First();
                vm.Entities = new RangeObservableCollection<ByCategoryReport>(byCategoryReport);
                vm.AllCategories = orderedCategories;
            }

            AddEntities(entities.OfType<Project>().DefaultOrder(), sb);
            AddEntities(entities.OfType<Payee>().DefaultOrder(), sb);
            AddEntities(entities.OfType<Location>().DefaultOrder(), sb);
            AddEntities(entities.OfType<Currency>().ToList(), sb);
            AddEntities(entities.OfType<Budget>().ToList(), sb);

            keyLessEntities.Clear();

            AddKeylessEntities(entities.OfType<CCardClosingDate>().ToList(), sb);
            AddKeylessEntities(entities.OfType<CategoryAttribute>().ToList(), sb);
            AddKeylessEntities(entities.OfType<TransactionAttribute>().ToList(), sb);

            var duration = DateTime.Now - start;
            sb.AppendLine($"Duration : {duration}");
            var info = new InfoVM { Text = sb.ToString() };
            info.RequestClose += InfoClose;
            CurrentPage = info;
            Logger.Info(info.Text);
        }

        public async Task SaveBackup(string backupPath)
        {
            SaveBackupPath = backupPath;
            List<Entity> itemsToBackup = new();
            itemsToBackup.AddRange(keyLessEntities);
            using (IUnitOfWork uow = db.CreateUnitOfWork())
            {
                await AddBackupEntities<Budget>(uow, itemsToBackup);
                await AddBackupEntities<TransactionAttribute>(uow, itemsToBackup);
                await AddBackupEntities<CurrencyExchangeRate>(uow, itemsToBackup);
                await AddBackupEntities<Currency>(uow, itemsToBackup);
                await AddBackupEntities<Location>(uow, itemsToBackup, x => x.Id > 0);
                await AddBackupEntities<Payee>(uow, itemsToBackup);
                await AddBackupEntities<Project>(uow, itemsToBackup);
                await AddBackupEntities<Transaction>(uow, itemsToBackup);
                await AddBackupEntities<Account>(uow, itemsToBackup, orderBy: x => x.Id);
                await AddBackupEntities<AttributeDefinition>(uow, itemsToBackup, x => x.Id > 0);
                await AddBackupEntities<CategoryAttribute>(uow, itemsToBackup);
                await AddBackupEntities<CCardClosingDate>(uow, itemsToBackup);
                await AddBackupEntities<SmsTemplate>(uow, itemsToBackup);
                await AddBackupEntities<Category>(uow, itemsToBackup, x => x.Id > 0);
            }

            using var bw = new BackupWriter(backupPath, _backupVersion, _entityColumnsOrder);
            bw.GenerateBackup(itemsToBackup);
        }

        private async Task AddBackupEntities<T>(IUnitOfWork uow, List<Entity> itemsToBackup, Func<T, bool> where = null, Func<T, int> orderBy = null)
        where T : Entity
        {
            var allItems = await uow.GetRepository<T>().GetAllAsync();
            if (where != null)
            {
                allItems = allItems.Where(where).ToList();
            }
            if (orderBy != null)
            {
                allItems = allItems.OrderBy(orderBy).ToList();
            }
            itemsToBackup.AddRange(allItems);
        }

        private void AddEntities<T>(List<T> entities, StringBuilder sb, bool replace = false)
        where T : Entity
        {
            sb?.AppendLine($"Imported {typeof(T).Name} {entities.Count}");
            var vm = _pages.FirstOrDefault(x => x.GetType().BaseType.GetGenericArguments().Single() == typeof(T)) as EntityBaseVM<T>;
            if (replace)
            {
                vm.Entities = new RangeObservableCollection<T>(entities);
            }
            else
            {
                vm.Entities.AddRange(entities);
            }
        }

        private void AddKeylessEntities<T>(List<T> entities, StringBuilder sb)
        where T : Entity
        {
            sb?.AppendLine($"Imported {typeof(T).Name} {entities.Count}");
            keyLessEntities.AddRange(entities);
        }

        private async void Blotter_AddTransactionRaised(object sender, EventArgs e)
        {
            await OpenTransactionDialog(0);
        }

        private async void Blotter_AddTransferRaised(object sender, EventArgs e)
        {
            await OpenTransferDialog(0);
        }

        private async void Blotter_DeleteRaised(object sender, TransactionsView e)
        {
            var result = System.Windows.Forms.MessageBox.Show("Are you sure you want to delete transaction?", "Delete", System.Windows.Forms.MessageBoxButtons.YesNo);
            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                using (var uow = db.CreateUnitOfWork())
                {
                    var repo = uow.GetRepository<Transaction>();
                    var transaction = await repo.FindByAsync(x => x.Id == e._id);

                    await repo.DeleteAsync(transaction);
                    await uow.SaveChangesAsync();
                }
                await RefreshBlotterTransactions();
            }
        }
        
        private async void BlotterVM_OpenTransactionRaised(object sender, TransactionsView e)
        {
            if (e.from_account_id > 0 && e.to_account_id > 0 && e.category_id == 0)
            {
                await OpenTransferDialog(e._id);
            }
            else
            {
                await OpenTransactionDialog(e._id);
            }
        }

        private void CreatePages()
        {
            accountsVm = new AccountsVM();
            if (Blotter != null)
            {
                Blotter.EditRaised -= BlotterVM_OpenTransactionRaised;
                Blotter.AddRaised -= Blotter_AddTransactionRaised;
                Blotter.AddTransferRaised -= Blotter_AddTransferRaised;
                Blotter.DeleteRaised -= Blotter_DeleteRaised;
            }

            Blotter = new BlotterVM();
            Blotter.EditRaised += BlotterVM_OpenTransactionRaised;
            Blotter.AddRaised += Blotter_AddTransactionRaised;
            Blotter.AddTransferRaised += Blotter_AddTransferRaised;
            Blotter.DeleteRaised += Blotter_DeleteRaised;

            if (Locations != null)
            {
                Locations.AddRaised -= Locations_AddRaised;
                Locations.EditRaised -= Locations_EditRaised;
            }
            Locations = new LocationsVM();
            Locations.AddRaised += Locations_AddRaised;
            Locations.EditRaised += Locations_EditRaised;

            if (Payees != null)
            {
                Payees.AddRaised -= Payees_AddRaised;
                Payees.EditRaised -= Payees_EditRaised;
            }
            Payees = new PayeesVM();
            Payees.AddRaised += Payees_AddRaised;
            Payees.EditRaised += Payees_EditRaised;

            if (Projects != null)
            {
                Projects.AddRaised -= Projects_AddRaised;
                Projects.EditRaised -= Projects_EditRaised;
            }
            Projects = new ProjectsVM();
            Projects.AddRaised += Projects_AddRaised;
            Projects.EditRaised += Projects_EditRaised;

            _pages = new List<BindableBase>
                {
                    accountsVm,
                    Blotter,
                    new BudgetsVM(),
                    new CategoriesVM(),
                    new CurrenciesVM(),
                    new ExchangeRatesVM(),
                    Locations,
                    Payees,
                    Projects,
                    new ReportVM()
                }.AsReadOnly();
        }

        private async void Projects_EditRaised(object sender, Project e)
        {
            await OpenEntityWithTitleDialog<Project>(e.Id);
        }

        private async void Projects_AddRaised(object sender, EventArgs e)
        {
            await OpenEntityWithTitleDialog<Project>(0);
        }

        private async void Payees_EditRaised(object sender, Payee e)
        {
            await OpenEntityWithTitleDialog<Payee>(e.Id);
        }

        private async void Payees_AddRaised(object sender, EventArgs e)
        {
            await OpenEntityWithTitleDialog<Payee>(0);
        }

        private async void Locations_EditRaised(object sender, Location e)
        {
            await OpenLocationDialog(e.Id);
        }

        private async void Locations_AddRaised(object sender, EventArgs e)
        {
            await OpenLocationDialog(0);
        }

        private void InfoClose(object sender, EventArgs e)
        {
            CurrentPage = Pages.OfType<AccountsVM>().First();
        }

        private void NavigateToType(Type type)
        {
            CurrentPage = Pages.FirstOrDefault(x => x.GetType().BaseType.GetGenericArguments().Single() == type);
        }

        private async void OpenMonoWizard()
        {
            using var openFileDialog = new OpenFileDialog
            {
                Multiselect = false,
                Filter = "CSV files (*.csv)|*.csv"
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                var fileName = openFileDialog.FileName;

                var accounts = Pages.OfType<AccountsVM>().First().Entities.ToList();
                var currencies = Pages.OfType<CurrenciesVM>().First().Entities.ToList();
                var locations = Locations.Entities.ToList();
                var categories = Pages.OfType<CategoriesVM>().First().Entities.ToList();
                var projects = Projects.Entities.ToList();

                var viewModel = new MonoWizardVM(accounts, currencies, locations, categories, projects, fileName);
                await viewModel.LoadTransactions();

                var args = dialogWrapper.ShowWizard(viewModel);

                if (args)
                {
                    var monoToImport = viewModel.TransactionsToImport.Where(item =>
                    !Blotter.Entities.Any(x =>
                    x.from_account_id == item.FromAccountId &&
                    x.datetime == item.DateTime &&
                    x.from_amount == item.FromAmount)).ToList();

                    var duplicatesCount = viewModel.TransactionsToImport.Count - monoToImport.Count;

                    await ImportMonoTransactions(monoToImport);
                    MessageBox.Show($"Imported {monoToImport.Count} transactions."
                        + ((duplicatesCount > 0) ? $" Skiped {duplicatesCount} duplicates." : string.Empty));

                    Logger.Info($"Imported {monoToImport.Count} transactions. Found duplicates : {duplicatesCount}");
                }
            }
        }

        private async Task OpenTransactionDialog(int id)
        {
            TransactionDialogVM dialogVm = new TransactionDialogVM(dialogWrapper);
            Transaction transaction = await db.GetOrCreateTransaction(id);
            IEnumerable<TransactionDTO> subTransactions = (await db.GetSubTransactions(id)).Select(x => new TransactionDTO(x));
            var transactionDto = new TransactionDTO(transaction);
            transactionDto.SubTransactions = new ObservableCollection<TransactionDTO>(subTransactions);
            dialogVm.Transaction = transactionDto;

            using (var uow = db.CreateUnitOfWork())
            {
                dialogVm.Accounts = await uow.GetAllOrderedByDefaultAsync<Account>(x => x.Currency);
                var allCategories = await uow.GetAllAsync<Category>();
                dialogVm.Categories = new ObservableCollection<Category>(allCategories.OrderBy(x => x.Left));
                dialogVm.Payees = await uow.GetAllOrderedByDefaultAsync<Payee>();
                dialogVm.Currencies = await uow.GetAllAsync<Currency>();
                dialogVm.Locations = await uow.GetAllOrderedByDefaultAsync<Location>();
                dialogVm.Projects = await uow.GetAllOrderedByDefaultAsync<Project>();
            }

            var result = dialogWrapper.ShowDialog<TransactionControl>(dialogVm, 640, 340, nameof(Transaction));

            if (result is TransactionDialogVM)
            {
                var resultVm = result as TransactionDialogVM;
                var resultTransactions = new List<Transaction>();

                MapperHelper.MapTransaction(resultVm.Transaction, transaction);

                resultTransactions.Add(transaction);
                if (resultVm.Transaction?.SubTransactions?.Any() == true)
                {
                    foreach (var subTransactionDto in resultVm.Transaction.SubTransactions)
                    {
                        var subTransaction = await db.GetOrCreate<Transaction>(subTransactionDto.Id);
                        subTransactionDto.Date = resultVm.Transaction.Date;
                        subTransactionDto.Time = resultVm.Transaction.Time;
                        MapperHelper.MapTransaction(subTransactionDto, subTransaction);
                        subTransaction.Parent = transaction;
                        subTransaction.FromAccountId = transaction.FromAccountId;
                        subTransaction.OriginalCurrencyId = transaction.OriginalCurrencyId ?? transaction.FromAccount.CurrencyId;
                        subTransaction.Category = default;
                        resultTransactions.Add(subTransaction);
                    }
                }

                await db.InsertOrUpdate(resultTransactions);
                await RefreshBlotterTransactions();
            }
        }

        private async Task OpenTransferDialog(int id)
        {
            TransferDialogVM dialogVm = new TransferDialogVM();
            Transaction transfer = await db.GetOrCreateTransaction(id);
            dialogVm.Transfer = new TransferDTO(transfer);
            var uow = db.CreateUnitOfWork();
            dialogVm.Accounts = await uow.GetAllAsync<Account>(x => x.Currency);

            var result = dialogWrapper.ShowDialog<TransferControl>(dialogVm, 385, 340, "Transfer");

            if (result is TransferDialogVM)
            {
                MapperHelper.MapTransfer((result as TransferDialogVM)?.Transfer, transfer);
                await db.InsertOrUpdate(new[] { transfer });
                await RefreshBlotterTransactions();
            }
        }

        private async Task RefreshBlotterTransactions()
        {
            using var uow = db.CreateUnitOfWork();
            var allTransactions = await uow.GetAllOrderedAsync<BlotterTransactions>(x => x.datetime, null, x => x.from_account_currency, x => x.to_account_currency);
            AddEntities(allTransactions.ToList(), null, true);
        }

        private async Task RefreshEntities<T>()
            where T: Entity, IActive
        {
            using var uow = db.CreateUnitOfWork();
            var entities = await uow.GetAllOrderedByDefaultAsync<T>();
            AddEntities(entities.ToList(), null, true);
        }

        private async Task OpenLocationDialog(int id)
        {
            Location selectedValue = await db.GetOrCreate<Location>(id);
            LocationVM locationVm = new LocationVM(selectedValue);

            var result = dialogWrapper.ShowDialog<LocationControl>(locationVm, 240, 300, nameof(Location));

            if (result is LocationVM)
            {
                var updatedItem = (LocationVM)result;
                selectedValue.IsActive = updatedItem.IsActive;
                selectedValue.Address = updatedItem.Address;
                selectedValue.Title = updatedItem.Title;
                selectedValue.Name = updatedItem.Title;
                selectedValue.Id = updatedItem.Id;
                if (id == 0)
                {
                    selectedValue.Count = 0;
                }

                await db.InsertOrUpdate(new[] { selectedValue });
                await RefreshEntities<Location>();
            }
        }

        private async Task OpenEntityWithTitleDialog<T>(int e)
            where T: Entity, IActive, new()
        {
            T selectedEntity = await db.GetOrCreate<T>(e); ;
            EntityWithTitleVM context = new EntityWithTitleVM(selectedEntity);

            var result = dialogWrapper.ShowDialog<EntityWithTitleControl>(context, 180, 300, typeof(T).Name);

            if (result is EntityWithTitleVM)
            {
                var updatedItem = (EntityWithTitleVM)result;
                selectedEntity.IsActive = updatedItem.IsActive;
                selectedEntity.Title = updatedItem.Title;
                selectedEntity.Id = updatedItem.Id;

                await db.InsertOrUpdate(new[] { selectedEntity });
                await RefreshEntities<T>();
            }
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

        private async void SaveBackup_Click()
        {
            var backupPath = dialogWrapper.SaveFileDialog(Backup, Path.Combine(Path.GetDirectoryName(OpenBackupPath), BackupWriter.GenerateFileName()));
            if (!string.IsNullOrEmpty(backupPath))
            {
                await SaveBackup(backupPath);

                MessageBox.Show($"Saved {backupPath}", "Backup done.");
                Logger.Info($"Backup done. Saved {backupPath}");
            }
        }
    }
}
