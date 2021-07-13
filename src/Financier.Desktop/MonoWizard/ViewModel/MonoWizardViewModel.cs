using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CsvHelper;
using Financier.DataAccess.Data;
using Prism.Commands;
using Prism.Mvvm;
using Financier.DataAccess.Monobank;

namespace Financier.Desktop.MonoWizard.ViewModel
{
    public class MonoWizardViewModel : BindableBase
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly List<Account> accounts;
        private readonly List<Currency> currencies;
        private readonly List<Location> locations;
        private readonly List<Category> categories;
        private readonly string csvFilePath;
        private readonly List<MonoTransaction> monoTransactions = new();

        private DelegateCommand _cancelCommand;
        private DelegateCommand _moveNextCommand;
        private DelegateCommand _movePreviousCommand;

        private WizardBaseViewModel _currentPage;
        private ReadOnlyCollection<WizardBaseViewModel> _pages;

        public MonoWizardViewModel(List<Account> accounts, List<Currency> currencies, List<Location> locations, List<Category> categories, string csvFilePath)
        {
            this.accounts = new List<Account>(accounts);
            this.currencies = new List<Currency>(currencies);
            this.locations = new List<Location>(locations);
            this.categories = new List<Category>(categories);
            this.csvFilePath = csvFilePath;
        }
        public event EventHandler<bool> RequestClose;

        public DelegateCommand CancelCommand
        {
            get
            {
                return _cancelCommand ??= new DelegateCommand(()=> OnRequestClose(false));
            }
        }

        public WizardBaseViewModel CurrentPage
        {
            get => _currentPage;
            private set
            {
                if (value == _currentPage)
                    return;

                if (_currentPage != null)
                    _currentPage.IsCurrentPage = false;


                if (_currentPage is Page1ViewModel page1)
                {
                    var monoAccount = page1.MonoAccount;
                    ((Page2ViewModel)value).MonoAccount = monoAccount;
                    MonoBankAccount = monoAccount;
                    Logger.Info($"MonoBankAccount -> {JsonSerializer.Serialize(monoAccount)}");
                }

                if (_currentPage is Page2ViewModel page2)
                {
                    ((Page3ViewModel)value).MonoAccount = MonoBankAccount;
                    ((Page3ViewModel)value).SetMonoTransactions(page2.MonoTransactions);
                    Logger.Info($"MonoTransactions count -> {page2.MonoTransactions.Count}");
                }

                _currentPage = value;

                if (_currentPage != null)
                {
                    _currentPage.IsCurrentPage = true;
                    Logger.Info($"Current page -> {_currentPage.GetType().FullName}");
                }


                MovePreviousCommand.RaiseCanExecuteChanged();
                MoveNextCommand.RaiseCanExecuteChanged();
                RaisePropertyChanged(nameof(Title));
                RaisePropertyChanged(nameof(CurrentPage));
                RaisePropertyChanged(nameof(IsOnLastPage));
            }
        }

        public bool IsOnLastPage => CurrentPageIndex == Pages.Count - 1;

        public Account MonoBankAccount { get; set; }

        public DelegateCommand MoveNextCommand
        {
            get
            {
                return _moveNextCommand ??= new DelegateCommand(MoveToNextPage, () => CanMoveToNextPage);
            }
        }

        public DelegateCommand MovePreviousCommand
        {
            get
            {
                return _movePreviousCommand ??= new DelegateCommand(MoveToPreviousPage, () => CanMoveToPreviousPage);
            }
        }

        public ReadOnlyCollection<WizardBaseViewModel> Pages => _pages;

        public string Title => CurrentPage == null ? string.Empty : CurrentPage.Title;

        public List<Transaction> TransactionsToImport { get; set; }

        bool CanMoveToNextPage => CurrentPage != null && CurrentPage.IsValid();

        bool CanMoveToPreviousPage => 0 < CurrentPageIndex;

        int CurrentPageIndex
        {
            get
            {
                if (CurrentPage == null)
                {
                    Debug.Fail("Why is the current page null?");
                }
                return Pages.IndexOf(CurrentPage);
            }
        }

        public async Task LoadTransactions()
        {
            if (File.Exists(csvFilePath))
            {
                Logger.Info($"csvFilePath -> {csvFilePath}");
                await using FileStream file = File.OpenRead(csvFilePath);
                using StreamReader streamReader = new StreamReader(file, Encoding.UTF8);
                using var csv = new CsvReader(streamReader, CultureInfo.InvariantCulture);
                var records = await csv.GetRecordsAsync<MonoTransaction>().ToListAsync();
                monoTransactions.AddRange(records);
                CreatePages();
                CurrentPage = Pages[0];
            }
        }

        void CreatePages()
        {
            _pages = new List<WizardBaseViewModel>
                {
                    new Page1ViewModel(accounts),
                    new Page2ViewModel(monoTransactions),
                    new Page3ViewModel(accounts, currencies, locations, categories)
                }.AsReadOnly();
        }

        void MoveToNextPage()
        {
            if (CanMoveToNextPage)
            {
                if (CurrentPageIndex < Pages.Count - 1)
                    CurrentPage = Pages[CurrentPageIndex + 1];
                else
                    OnRequestClose(true);
            }
        }
        void MoveToPreviousPage()
        {
            if (CanMoveToPreviousPage)
                CurrentPage = Pages[CurrentPageIndex - 1];
        }

        void OnRequestClose(bool save)
        {
            if (save)
            {
                TransactionsToImport = _pages.OfType<Page3ViewModel>()
                    .Single()
                    .FinancierTransactions
                    .Select(TransformMonoTransaction)
                    .ToList();
            }

            RequestClose?.Invoke(this, save);
        }


        private Transaction TransformMonoTransaction(FinancierTransactionViewModel x)
        {
            var result = new Transaction
            {
                Id = 0,
                FromAmount = x.FromAmount,
                OriginalFromAmount = x.OriginalFromAmount ?? 0,
                OriginalCurrencyId = x.OriginalCurrencyId,
                Note = x.Note,
                LocationId = x.LocationId,
                CategoryId = 0,
                DateTime = x.DateTime,
                ToAmount = 0
            };

            if (x.ToAccountId > 0) // Transfer From Mono
            {
                result.FromAccountId = x.MonoAccountId;
                result.ToAccountId = x.ToAccountId;
                result.ToAmount = Math.Abs(x.OriginalFromAmount ?? x.FromAmount);
            }
            else
            if (x.FromAccountId > 0) // Transfer To Mono
            {
                result.FromAccountId = x.FromAccountId;
                result.ToAccountId = x.MonoAccountId;
                result.ToAmount = Math.Abs(x.OriginalFromAmount ?? x.FromAmount);
                result.FromAmount = -1 * Math.Abs(x.OriginalFromAmount ?? x.FromAmount);
            }
            else // Expanse
            {
                result.FromAccountId = x.MonoAccountId;
                result.CategoryId = x.CategoryId;
                result.ToAccountId = 0;
                result.ToAmount = 0;
            }

            return result;
        }
    }
}
