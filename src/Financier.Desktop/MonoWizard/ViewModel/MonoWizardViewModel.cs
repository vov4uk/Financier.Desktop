using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
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
        private readonly List<Account> accounts = new();
        private readonly string csvFilePath;
        private readonly List<MonoTransaction> monoTransactions = new();

        private DelegateCommand _cancelCommand;
        private DelegateCommand _moveNextCommand;
        private DelegateCommand _movePreviousCommand;

        private WizardBaseViewModel _currentPage;
        private ReadOnlyCollection<WizardBaseViewModel> _pages;

        public MonoWizardViewModel(List<Account> accounts, string csvFilePath)
        {
            this.accounts.AddRange(accounts);
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


                if (_currentPage is Page1ViewModel model)
                {
                    var monoAccount = model.MonoAccount;
                    ((Page2ViewModel)value).MonoAccount = monoAccount;
                    MonoBankAccount = monoAccount;
                }

                _currentPage = value;

                if (_currentPage != null)
                    _currentPage.IsCurrentPage = true;

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
        public List<MonoTransaction> TransactionsToImport { get; set; }

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
                    new Page2ViewModel(monoTransactions)
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
            TransactionsToImport = _pages.OfType<Page2ViewModel>().Single().TransactionsToImport;
            EventHandler <bool> handler = RequestClose;
            if (handler != null)
                handler(this, save);
        }

    }
}
