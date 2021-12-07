using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;

namespace Financier.Desktop.Wizards
{
    public abstract class WizardBaseVM : BindableBase
    {
        protected static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private DelegateCommand _cancelCommand;
        private DelegateCommand _moveNextCommand;
        private DelegateCommand _movePreviousCommand;

        protected WizardPageBaseVM _currentPage;
        protected ReadOnlyCollection<WizardPageBaseVM> _pages;

        public event EventHandler<bool> RequestClose;

        public DelegateCommand CancelCommand
        {
            get
            {
                return _cancelCommand ??= new DelegateCommand(() => OnClose(false));
            }
        }

        public abstract void BeforeCurrentPageUpdated(WizardPageBaseVM old, WizardPageBaseVM newValue);
        public abstract void AfterCurrentPageUpdated(WizardPageBaseVM newValue);
        public abstract void CreatePages();
        public abstract void OnRequestClose(bool save);

        public WizardPageBaseVM CurrentPage
        {
            get => _currentPage;
            protected set
            {
                if (value == _currentPage)
                    return;

                BeforeCurrentPageUpdated(_currentPage, value);

                _currentPage = value;

                AfterCurrentPageUpdated(_currentPage);

                MovePreviousCommand.RaiseCanExecuteChanged();
                MoveNextCommand.RaiseCanExecuteChanged();
                RaisePropertyChanged(nameof(Title));
                RaisePropertyChanged(nameof(CurrentPage));
                RaisePropertyChanged(nameof(IsOnLastPage));
            }
        }

        public bool IsOnLastPage => CurrentPageIndex == Pages.Count - 1;

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

        public ReadOnlyCollection<WizardPageBaseVM> Pages => _pages;

        public string Title => CurrentPage == null ? string.Empty : CurrentPage.Title;

        bool CanMoveToNextPage => CurrentPage != null && CurrentPage.IsValid();

        bool CanMoveToPreviousPage => 0 < CurrentPageIndex;

        int CurrentPageIndex
        {
            get
            {
                if (CurrentPage == null)
                {
                    Logger.Error("Why is the current page null?");
                }
                return Pages.IndexOf(CurrentPage);
            }
        }

        void MoveToNextPage()
        {
            if (CanMoveToNextPage)
            {
                if (CurrentPageIndex < Pages.Count - 1)
                    CurrentPage = Pages[CurrentPageIndex + 1];
                else
                    OnClose(true);
            }
        }
        void MoveToPreviousPage()
        {
            if (CanMoveToPreviousPage)
                CurrentPage = Pages[CurrentPageIndex - 1];
        }

        void OnClose(bool save)
        {
            OnRequestClose(save);
            RequestClose?.Invoke(this, save);
        }
    }
}