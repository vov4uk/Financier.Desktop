﻿using Financier.Common;
using Financier.Common.Model;
using Financier.DataAccess.Abstractions;
using Financier.Desktop.Helpers;
using Mvvm.Async;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Financier.Desktop.ViewModel
{
    [ExcludeFromCodeCoverage]
    public abstract class EntityBaseVM<T> : BaseViewModel<T> 
        where T : BaseModel, new ()
    {
        private IAsyncCommand _addCommand;
        private IAsyncCommand _deleteCommand;
        private IAsyncCommand _editCommand;
        private T _selectedValue;
        protected readonly IDialogWrapper dialogWrapper;

        protected EntityBaseVM(IFinancierDatabase db, IDialogWrapper dialogWrapper)
            : base(db)
        {
            this.dialogWrapper = dialogWrapper;
        }
        public IAsyncCommand AddCommand => _addCommand ??= new AsyncCommand(OnAdd);

        public IAsyncCommand DeleteCommand => _deleteCommand ??= new AsyncCommand(() => OnDelete(SelectedValue), () => SelectedValue != null);

        public IAsyncCommand EditCommand => _editCommand ??= new AsyncCommand(() => OnEdit( SelectedValue), () => SelectedValue != null);

        public T SelectedValue
        {
            get => _selectedValue;
            set
            {
                SetProperty(ref _selectedValue, value);
                EditCommand.RaiseCanExecuteChanged();
                DeleteCommand.RaiseCanExecuteChanged();
            }
        }

        protected abstract Task OnDelete(T item);

        protected abstract Task OnEdit(T item);

        protected abstract Task OnAdd();
    }
}
