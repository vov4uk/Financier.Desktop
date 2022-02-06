using Financier.Common;
using Financier.Common.Model;
using Financier.DataAccess.Abstractions;
using Prism.Commands;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Financier.Desktop.ViewModel
{
    [ExcludeFromCodeCoverage]
    public abstract class EntityBaseVM<T> : BaseViewModel<T> 
        where T : BaseModel, new ()
    {
        private DelegateCommand _addCommand;
        private DelegateCommand _deleteCommand;
        private DelegateCommand _editCommand;
        private T _selectedValue;

        protected EntityBaseVM(IFinancierDatabase financierDatabase)
            : base(financierDatabase)
        {
        }
        public event EventHandler AddRaised;

        public event EventHandler<T> DeleteRaised;

        public event EventHandler<T> EditRaised;

        public DelegateCommand AddCommand => _addCommand ??= new DelegateCommand(() => AddRaised?.Invoke(this, EventArgs.Empty));

        public DelegateCommand DeleteCommand => _deleteCommand ??= new DelegateCommand(() => DeleteRaised?.Invoke(this, SelectedValue), () => SelectedValue != null);

        public DelegateCommand EditCommand => _editCommand ??= new DelegateCommand(() => EditRaised?.Invoke(this, SelectedValue), () => SelectedValue != null);

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
    }
}
