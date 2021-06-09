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
        private readonly string csvFilePath;
        private readonly List<MonoTransaction> monoTransactions = new List<MonoTransaction>();
        private readonly List<Account> accounts = new List<Account>();
        public MonoWizardViewModel(List<Account> accounts, string csvFilePath)
        {
            this.accounts.AddRange(accounts);
            this.csvFilePath = csvFilePath;
        }

        /// <summary>
        /// Returns the command which, when executed, cancels the order 
        /// and causes the Wizard to be removed from the user interface.
        /// </summary>
        private DelegateCommand _cancelCommand;
        public DelegateCommand CancelCommand
        {
            get
            {
                if (_cancelCommand == null)
                    _cancelCommand = new DelegateCommand(Cancel);

                return _cancelCommand;
            }
        }

        void Cancel()
        {
            OnRequestClose(false);
        }

        private DelegateCommand _moveNextCommand;
        public DelegateCommand MoveNextCommand
        {
            get
            {
                return _moveNextCommand ?? (_moveNextCommand = new DelegateCommand(MoveToNextPage, () => CanMoveToNextPage));
            }
        }

        bool CanMoveToNextPage
        {
            get { return CurrentPage != null && CurrentPage.IsValid(); }
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

        #region MovePreviousCommand

        /// <summary>
        /// Returns the command which, when executed, causes the CurrentPage 
        /// property to reference the previous page in the workflow.
        /// </summary>
        private DelegateCommand _movePreviousCommand;
        public DelegateCommand MovePreviousCommand
        {
            get
            {
                return _movePreviousCommand ?? (_movePreviousCommand = new DelegateCommand(MoveToPreviousPage, () => CanMoveToPreviousPage));
            }
        }

        bool CanMoveToPreviousPage
        {
            get { return 0 < CurrentPageIndex; }
        }

        void MoveToPreviousPage()
        {
            if (CanMoveToPreviousPage)
                CurrentPage = Pages[CurrentPageIndex - 1];
        }

        #endregion // MovePreviousCommand

        /// <summary>
        /// Returns the page ViewModel that the user is currently viewing.
        /// </summary>
        private WizardBaseViewModel _currentPage;
        public WizardBaseViewModel CurrentPage
        {
            get { return _currentPage; }
            private set
            {
                if (value == _currentPage)
                    return;

                if (_currentPage != null)
                    _currentPage.IsCurrentPage = false;


                if (_currentPage is Page1ViewModel)
                {
                    var monoAccount = ((Page1ViewModel)_currentPage).MonoAccount;
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

        /// <summary>
        /// Returns true if the user is currently viewing the last page 
        /// in the workflow.  This property is used by CoffeeWizardView
        /// to switch the Next button's text to "Finish" when the user
        /// has reached the final page.
        /// </summary>
        public bool IsOnLastPage
        {
            get { return CurrentPageIndex == Pages.Count - 1; }
        }

        /// <summary>
        /// Returns a read-only collection of all page ViewModels.
        /// </summary>
        private ReadOnlyCollection<WizardBaseViewModel> _pages;
        public ReadOnlyCollection<WizardBaseViewModel> Pages
        {
            get
            {
                return _pages;
            }
        }

        public List<MonoTransaction> TransactionsToImport { get; set; }

        public Account MonoBankAccount { get; set; }

        #region Events

        /// <summary>
        /// Raised when the wizard should be removed from the UI.
        /// </summary>
        public event EventHandler<bool> RequestClose;

        #endregion // Events

        #region Private Helpers

        void CreatePages()
        {
            _pages = new List<WizardBaseViewModel>
                {
                    new Page1ViewModel(accounts),
                    new Page2ViewModel(monoTransactions)
                }.AsReadOnly();
        }

        public string Title
        {
            get { return CurrentPage == null ? string.Empty : CurrentPage.Title; }
        }

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

        void OnRequestClose(bool save)
        {
            TransactionsToImport = _pages.OfType<Page2ViewModel>().Single().TransactionsToImport;
            EventHandler <bool> handler = RequestClose;
            if (handler != null)
                handler(this, save);
        }

        public async Task LoadTransactions()
        {
            if (File.Exists(csvFilePath))
            {
                using (FileStream file = File.OpenRead(csvFilePath))
                {
                    using (StreamReader streamReader = new StreamReader(file, Encoding.UTF8))
                    {
                        using (var csv = new CsvReader(streamReader, CultureInfo.InvariantCulture))
                        {
                            var records = await csv.GetRecordsAsync<MonoTransaction>().ToListAsync();
                            monoTransactions.AddRange(records);
                            CreatePages();
                            CurrentPage = Pages[0];
                        }
                    }
                }
            }
        }
        #endregion
    }
}
