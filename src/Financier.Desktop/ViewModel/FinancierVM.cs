﻿using Financier.DataAccess;
using Financier.DataAccess.Abstractions;
using Financier.DataAccess.Data;
using Financier.DataAccess.View;
using Financier.Desktop.Converters;
using Financier.Desktop.Views;
using Financier.Adapter;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Financier.Desktop.ViewModel.Dialog;
using Financier.Desktop.Views.Controls;
using Financier.Desktop.Reports.ViewModel;

namespace Financier.Desktop.ViewModel
{
    public class FinancierVM : BindableBase
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly List<Entity> keyLessEntities = new();
        private DelegateCommand<Type> _menuNavigateCommand;

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

        public FinancierVM()
        {
            CreatePages();
        }

        public BlotterVM Blotter
        {
            get => blotter;
            set => SetProperty(ref blotter, value);
        }

        public LocationsVM Locations
        {
            get => locations;
            set => SetProperty(ref locations, value);
        }

        public ProjectsVM Projects
        {
            get => projects;
            set => SetProperty(ref projects, value);
        }

        public PayeesVM Payees
        {
            get => payees;
            set => SetProperty(ref payees, value);
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
            await db.ImportMonoTransactions(transactions);

            foreach (var accId in transactions
                .Where(x => x.ToAccountId > 0)
                .Select(x => x.ToAccountId)
                .Distinct())
            {
                await db.RebuildRunningBalanceForAccount(accId);
            }
            await db.RebuildRunningBalanceForAccount(transactions.Select(x => x.FromAccountId).FirstOrDefault());
            await RefreshBlotterTransactions();
        }

        public async Task OpenBackup(string backupPath)
        {
            var start = DateTime.Now;
            CreatePages();
            var sb = new StringBuilder();

            OpenBackupPath = backupPath;
            var entities = EntityReader.ParseBackupFile(backupPath).ToList();

            db = new FinancierDatabase();
            await db.Import(entities);

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
                AddEntities(allCategories.Where(x => x.Id > 0).ToList(), sb);
                AddEntities(allRates.ToList(), sb);

                sb?.AppendLine($"Imported {typeof(ReportVM).Name}");
                var vm = _pages.OfType<ReportVM>().First();
                vm.Entities = new RangeObservableCollection<ByCategoryReport>(byCategoryReport.ToList());
                vm.AllCategories = allCategories.Where(x => x.Id > 0).ToList();
            }

            AddEntities(entities.OfType<Project>().Where(x => x.Id > 0).OrderBy(x => x.Id).ToList(), sb);
            AddEntities(entities.OfType<Payee>().Where(x => x.Id > 0).OrderBy(x => x.Id).ToList(), sb);
            AddEntities(entities.OfType<Location>().Where(x => x.Id > 0).OrderBy(x => x.Id).ToList(), sb);
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
                await AddBackupEntities<Account>(uow, itemsToBackup, keySelector: x => x.Id);
                await AddBackupEntities<AttributeDefinition>(uow, itemsToBackup, x => x.Id > 0);
                await AddBackupEntities<CategoryAttribute>(uow, itemsToBackup);
                await AddBackupEntities<CCardClosingDate>(uow, itemsToBackup);
                await AddBackupEntities<SmsTemplate>(uow, itemsToBackup);
                await AddBackupEntities<Category>(uow, itemsToBackup, x => x.Id > 0);
            }

            using var bw = new BackupWriter(backupPath, EntityReader.BackupVersion);
            bw.GenerateBackup(itemsToBackup);
        }

        private async Task AddBackupEntities<T>(IUnitOfWork uow, List<Entity> itemsToBackup, Func<T, bool> predicate = null, Func<T, int> keySelector = null)
        where T : Entity
        {
            var allItems = await uow.GetRepository<T>().GetAllAsync();
            if (predicate != null)
            {
                allItems = allItems.Where(predicate).ToList();
            }
            if (keySelector != null)
            {
                allItems = allItems.OrderBy(keySelector).ToList();
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

        private void Blotter_DuplicateRaised(object sender, TransactionsView e)
        {
            throw new NotImplementedException();
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

        private TransactionDTO ConvertTransaction(Transaction transaction)
        {
            return new()
            {
                Id = transaction.Id,
                AccountId = transaction.FromAccountId,
                CategoryId = transaction.CategoryId,
                Category = transaction.Category,
                PayeeId = transaction.PayeeId,
                OriginalCurrencyId = transaction.OriginalCurrencyId,
                OriginalCurrency = transaction.OriginalCurrency,
                LocationId = transaction.LocationId,
                ProjectId = transaction.ProjectId,
                Note = transaction.Note,
                FromAmount = transaction.FromAmount,
                OriginalFromAmount = transaction.OriginalFromAmount,
                IsAmountNegative = transaction.FromAmount <= 0,
                Date = DateTimeConverter.Convert(transaction.DateTime).Date,
                Time = DateTimeConverter.Convert(transaction.DateTime),
            };
        }

        private TransferDTO ConvertTransfer(Transaction transaction)
        {
            return new()
            {
                Id = transaction.Id,
                FromAccountId = transaction.FromAccountId,
                ToAccountId = transaction.ToAccountId,
                Note = transaction.Note,
                FromAmount = transaction.FromAmount,
                ToAmount = transaction.ToAmount,
                Date = DateTimeConverter.Convert(transaction.DateTime).Date,
                Time = DateTimeConverter.Convert(transaction.DateTime),
            };
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
                Blotter.DuplicateRaised -= Blotter_DuplicateRaised;
            }

            Blotter = new BlotterVM();
            Blotter.EditRaised += BlotterVM_OpenTransactionRaised;
            Blotter.AddRaised += Blotter_AddTransactionRaised;
            Blotter.AddTransferRaised += Blotter_AddTransferRaised;
            Blotter.DeleteRaised += Blotter_DeleteRaised;
            Blotter.DuplicateRaised += Blotter_DuplicateRaised;

            if (Locations != null)
            {
                Locations.AddRaised -= Locations_AddRaised;
                Locations.EditRaised -= Locations_EditRaised;
                Locations.DeleteRaised -= Locations_DeleteRaised;
            }
            Locations = new LocationsVM();
            Locations.AddRaised += Locations_AddRaised;
            Locations.EditRaised += Locations_EditRaised;
            Locations.DeleteRaised += Locations_DeleteRaised;

            if (Payees != null)
            {
                Payees.AddRaised -= Payees_AddRaised;
                Payees.EditRaised -= Payees_EditRaised;
                Payees.DeleteRaised -= Payees_DeleteRaised;
            }
            Payees = new PayeesVM();
            Payees.AddRaised += Payees_AddRaised; ;
            Payees.EditRaised += Payees_EditRaised; ;
            Payees.DeleteRaised += Payees_DeleteRaised;

            if (Projects != null)
            {
                Projects.AddRaised -= Projects_AddRaised;
                Projects.EditRaised -= Projects_EditRaised;
                Projects.DeleteRaised -= Projects_DeleteRaised;
            }
            Projects = new ProjectsVM();
            Projects.AddRaised += Projects_AddRaised;
            Projects.EditRaised += Projects_EditRaised;
            Projects.DeleteRaised += Projects_DeleteRaised;

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

        private void Projects_DeleteRaised(object sender, Project e)
        {
            throw new NotImplementedException();
        }

        private async void Projects_EditRaised(object sender, Project e)
        {
            await OpenProjectDialog(e.Id);
        }

        private async void Projects_AddRaised(object sender, EventArgs e)
        {
            await OpenProjectDialog(0);
        }

        private void Payees_DeleteRaised(object sender, Payee e)
        {
            throw new NotImplementedException();
        }

        private async void Payees_EditRaised(object sender, Payee e)
        {
            await OpenPayeeDialog(e.Id);
        }

        private async void Payees_AddRaised(object sender, EventArgs e)
        {
            await OpenPayeeDialog(0);
        }

        private void Locations_DeleteRaised(object sender, Location e)
        {
            throw new NotImplementedException();
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

        private async Task InsertOrUpdate<T>(IEnumerable<T> entities)
            where T: Entity, IIdentity
        {
            using var uow = db.CreateUnitOfWork();
            var trRepo = uow.GetRepository<T>();
            foreach (var item in entities)
            {
                item.UpdatedOn = DateTimeConverter.ConvertBack(DateTime.Now);
                if (item.Id == 0)
                {
                    await trRepo.AddAsync(item);
                }
                else
                {
                    await trRepo.UpdateAsync(item);
                }
            }
            await uow.SaveChangesAsync();
        }

        private void MapTransaction(TransactionDTO vm, Transaction tr)
        {
            tr.Id = vm.Id;
            tr.FromAccountId = vm.AccountId;
            tr.FromAmount = vm.RealFromAmount;
            tr.OriginalFromAmount = vm.OriginalFromAmount ?? 0;
            tr.OriginalCurrencyId = vm.OriginalCurrencyId ?? 0;
            tr.CategoryId = vm.CategoryId ?? 0;
            tr.Category = default;
            tr.PayeeId = vm.PayeeId ?? 0;
            tr.LocationId = vm.LocationId ?? 0;
            tr.ProjectId = vm.CategoryId == Category.Split.Id ? 0 : (vm.ProjectId ?? 0);
            tr.Note = vm.Note;
            tr.DateTime = DateTimeConverter.ConvertBack(vm.DateTime);
        }

        private void MapTransfer(TransferDTO vm, Transaction tr)
        {
            tr.Id = vm.Id;
            tr.FromAccountId = vm.FromAccountId;
            tr.ToAccountId = vm.ToAccountId;
            tr.Note = vm.Note;
            tr.FromAmount = vm.FromAmount;
            tr.ToAmount = vm.ToAmount;
            tr.DateTime = DateTimeConverter.ConvertBack(vm.DateTime);
            tr.OriginalCurrencyId = vm.FromAccount.CurrencyId;
            tr.OriginalFromAmount = vm.FromAmount;
            tr.CategoryId = 0;
        }

        private void NavigateToType(Type type)
        {
            CurrentPage = Pages.FirstOrDefault(x => x.GetType().BaseType.GetGenericArguments().Single() == type);
        }

        private async Task OpenTransactionDialog(int e)
        {
            TransactionDialogVM context = new TransactionDialogVM();
            Transaction transaction;
            using (var uow = db.CreateUnitOfWork())
            {
                if (e > 0)
                {
                    Logger.Info($"Edit Transaction {e}");
                    var transactions = await uow.GetRepository<Transaction>().FindManyAsync(x => x.ParentId == e || x.Id == e, o => o.OriginalCurrency, c => c.Category);
                    transaction = transactions.First(x => x.Id == e);
                    var transactionVm = ConvertTransaction(transaction);
                    if (transactions.Any(x => x.ParentId == e))
                    {
                        IEnumerable<TransactionDTO> subTransactions = transactions.Where(x => x.ParentId == e).Select(ConvertTransaction);
                        transactionVm.SubTransactions = new ObservableCollection<TransactionDTO>(subTransactions);
                    }
                    context.Transaction = transactionVm;
                }
                else
                {
                    Logger.Info("Create Transaction");
                    transaction = new Transaction { DateTime = DateTimeConverter.ConvertBack(DateTime.Now), Id = 0 };
                    context.Transaction = ConvertTransaction(transaction);
                }

                var allAccounts = await uow.GetRepository<Account>().GetAllAsync(x => x.Currency);
                var allCategories = await uow.GetRepository<Category>().GetAllAsync();
                var allPayees = await uow.GetRepository<Payee>().GetAllAsync();
                var currencies = await uow.GetRepository<Currency>().GetAllAsync();
                var locations = await uow.GetRepository<Location>().GetAllAsync();
                var projects = await uow.GetRepository<Project>().GetAllAsync();
                context.Accounts = new ObservableCollection<Account>(allAccounts);
                context.Categories = new ObservableCollection<Category>(allCategories);
                context.Payees = new ObservableCollection<Payee>(allPayees);
                context.Currencies = new ObservableCollection<Currency>(currencies);
                context.Locations = new ObservableCollection<Location>(locations);
                context.Projects = new ObservableCollection<Project>(projects);
            }

            var dialog = new Window
            {
                Content = new TransactionControl { DataContext = context },
                ResizeMode = ResizeMode.NoResize,
                Height = 640,
                Width = 340,
                ShowInTaskbar = Debugger.IsAttached
            };
            context.RequestCancel += (_, _) =>
            {
                dialog.Close();
                Logger.Info("Transaction close");
            };
            context.RequestSave += async (sender, _) =>
            {
                dialog.Close();
                Logger.Info("Transaction save");
                try
                {
                    using var uow = db.CreateUnitOfWork();
                    var trRepo = uow.GetRepository<Transaction>();
                    var vm = sender as TransactionDialogVM;
                    var transactions = new List<Transaction>();

                    MapTransaction(vm.Transaction, transaction);

                    transactions.Add(transaction);
                    if (vm.Transaction?.SubTransactions?.Any() == true)
                    {
                        foreach (var item in vm.Transaction.SubTransactions)
                        {
                            var tr = item.Id == 0 ? new Transaction() : await trRepo.FindByAsync(x => x.Id == item.Id);
                            item.Date = vm.Transaction.Date;
                            item.Time = vm.Transaction.Time;
                            MapTransaction(item, tr);
                            tr.Parent = transaction;
                            tr.FromAccountId = transaction.FromAccountId;
                            tr.OriginalCurrencyId = transaction.OriginalCurrencyId ?? transaction.FromAccount.CurrencyId;
                            tr.OriginalFromAmount = item.OriginalFromAmount;
                            tr.DateTime = transaction.DateTime;
                            tr.Category = default;
                            transactions.Add(tr);
                        }
                    }

                    foreach (var entity in transactions)
                    {
                        if (entity.Id > 0)
                        {
                            await trRepo.UpdateAsync(entity);
                        }
                        else
                        {
                            await trRepo.AddAsync(entity);
                        }
                    }
                    await uow.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }

                await RefreshBlotterTransactions();
            };
            dialog.ShowDialog();
        }

        private async Task OpenTransferDialog(int e)
        {
            TransferDialogVM context = new TransferDialogVM();
            Transaction transaction;

            using (var uow = db.CreateUnitOfWork())
            {
                if (e != 0)
                {
                    Logger.Info($"Edit transfer {e}");
                    transaction = await uow.GetRepository<Transaction>().FindByAsync(x => x.Id == e, o => o.OriginalCurrency, c => c.Category);
                }
                else
                {
                    Logger.Info("Create transfer");
                    transaction = new Transaction { DateTime = DateTimeConverter.ConvertBack(DateTime.Now), Id = 0, CategoryId = 0 };
                }

                context.Transfer = ConvertTransfer(transaction);

                var allAccounts = await uow.GetRepository<Account>().GetAllAsync(x => x.Currency);
                context.Accounts = new ObservableCollection<Account>(allAccounts);
            }

            var dialog = new Window
            {
                Content = new TransferControl() { DataContext = context },
                ResizeMode = ResizeMode.NoResize,
                Height = 385,
                Width = 340,
                ShowInTaskbar = Debugger.IsAttached
            };
            context.RequestCancel += (_, _) =>
            {
                dialog.Close();
                Logger.Info("Transfer close");
            };
            context.RequestSave += async (sender, _) =>
            {
                dialog.Close();
                Logger.Info("Transfer save");
                MapTransfer((sender as TransferDialogVM)?.Transfer, transaction);
                await InsertOrUpdate(new[] { transaction });
                await RefreshBlotterTransactions();
            };
            dialog.ShowDialog();
        }

        private async Task RefreshBlotterTransactions()
        {
            using var uow = db.CreateUnitOfWork();
            var allTransactions = await uow.GetRepository<BlotterTransactions>().GetAllAsync(x => x.from_account_currency, x => x.to_account_currency);
            AddEntities(allTransactions.OrderByDescending(x => x.datetime).ToList(), null, true);
        }

        private async Task RefreshEntities<T>()
            where T: Entity, IIdentity
        {
            using var uow = db.CreateUnitOfWork();
            var locations = await uow.GetRepository<T>().GetAllAsync();
            AddEntities(locations.Where(x => x.Id > 0).OrderBy(x => x.Id).ToList(), null, true);
        }

        private async Task OpenLocationDialog(int e)
        {
            LocationVM context;
            Location selectedValue;

            using (var uow = db.CreateUnitOfWork())
            {
                if (e != 0)
                {
                    Logger.Info($"Edit location {e}");
                    selectedValue = await uow.GetRepository<Location>().FindByAsync(x => x.Id == e);
                }
                else
                {
                    Logger.Info("Create location");
                    selectedValue = new Location { Date = DateTimeConverter.ConvertBack(DateTime.Now), Id = 0 };
                }

                context = new LocationVM(selectedValue);
            }

            var dialog = new Window
            {
                Content = new LocationControl() { DataContext = context },
                ResizeMode = ResizeMode.NoResize,
                Height = 240,
                Width = 300,
                ShowInTaskbar = Debugger.IsAttached
            };
            context.RequestCancel += (_, _) =>
            {
                dialog.Close();
                Logger.Info("Location dialog close");
            };
            context.RequestSave += async (sender, _) =>
            {
                dialog.Close();
                Logger.Info("Location dialog save");
                var updatedItem = sender as LocationVM;
                selectedValue.IsActive = updatedItem.IsActive;
                selectedValue.Address = updatedItem.Address;
                selectedValue.Title = updatedItem.Title;
                selectedValue.Name = updatedItem.Title;
                selectedValue.Id = updatedItem.Id;

                await InsertOrUpdate(new[] { selectedValue });
                await RefreshEntities<Location>();
            };
            dialog.ShowDialog();
        }

        private async Task OpenProjectDialog(int e)
        {
            EntityWithTitleVM context;
            Project selectedValue;

            using (var uow = db.CreateUnitOfWork())
            {
                if (e != 0)
                {
                    Logger.Info($"Edit Project {e}");
                    selectedValue = await uow.GetRepository<Project>().FindByAsync(x => x.Id == e);
                }
                else
                {
                    Logger.Info("Create Project");
                    selectedValue = new Project { Id = 0 };
                }

                context = new EntityWithTitleVM(selectedValue);
            }

            var dialog = new Window
            {
                Content = new EntityWithTitleControl() { DataContext = context },
                ResizeMode = ResizeMode.NoResize,
                Height = 180,
                Width = 300,
                ShowInTaskbar = Debugger.IsAttached
            };
            context.RequestCancel += (_, _) =>
            {
                dialog.Close();
                Logger.Info("Project dialog close");
            };
            context.RequestSave += async (sender, _) =>
            {
                dialog.Close();
                Logger.Info("Project dialog save");
                var updatedItem = sender as EntityWithTitleVM;
                selectedValue.IsActive = updatedItem.IsActive;
                selectedValue.Title = updatedItem.Title;
                selectedValue.Id = updatedItem.Id;

                await InsertOrUpdate(new[] { selectedValue });
                await RefreshEntities<Project>();
            };
            dialog.ShowDialog();
        }

        private async Task OpenPayeeDialog(int e)
        {
            EntityWithTitleVM context;
            Payee selectedValue;

            using (var uow = db.CreateUnitOfWork())
            {
                if (e != 0)
                {
                    Logger.Info($"Edit Payee {e}");
                    selectedValue = await uow.GetRepository<Payee>().FindByAsync(x => x.Id == e);
                }
                else
                {
                    Logger.Info("Create Payee");
                    selectedValue = new Payee { Id = 0 };
                }

                context = new EntityWithTitleVM(selectedValue);
            }

            var dialog = new Window
            {
                Content = new EntityWithTitleControl() { DataContext = context },
                ResizeMode = ResizeMode.NoResize,
                Height = 180,
                Width = 300,
                ShowInTaskbar = Debugger.IsAttached
            };
            context.RequestCancel += (_, _) =>
            {
                dialog.Close();
                Logger.Info("Project dialog close");
            };
            context.RequestSave += async (sender, _) =>
            {
                dialog.Close();
                Logger.Info("Project dialog save");
                var updatedItem = sender as EntityWithTitleVM;
                selectedValue.IsActive = updatedItem.IsActive;
                selectedValue.Title = updatedItem.Title;
                selectedValue.Id = updatedItem.Id;

                await InsertOrUpdate(new[] { selectedValue });
                await RefreshEntities<Payee>();
            };
            dialog.ShowDialog();
        }
    }
}
