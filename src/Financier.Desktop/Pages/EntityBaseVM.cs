using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Windows;
using Financier.Common;
using Financier.Common.Model;
using Financier.DataAccess.Abstractions;
using Financier.Desktop.Helpers;
using Financier.Desktop.Localization;

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
        protected LocalizationManager localizationManager;

        protected EntityBaseVM(IFinancierDatabase db, IDialogWrapper dialogWrapper, LocalizationManager localizationManager)
            : base(db)
        {
            this.dialogWrapper = dialogWrapper;
            this.localizationManager = localizationManager;
        }
        public IAsyncCommand AddCommand => _addCommand ??= new AsyncCommand(OnAdd);

        public IAsyncCommand DeleteCommand => _deleteCommand ??= new AsyncCommand(() => OnDelete(SelectedValue), () => SelectedValue != null);

        public IAsyncCommand EditCommand => _editCommand ??= new AsyncCommand(() => OnEdit(SelectedValue), () => SelectedValue != null);

        public LocalizationManager LocalizationManager
        {
            get => localizationManager;
        }

        public T SelectedValue
        {
            get => _selectedValue;
            set
            {
                SetProperty(ref _selectedValue, value);
                OnSelectedValueChanged();
            }
        }

        protected abstract Task OnDelete(T item);

        protected abstract Task OnEdit(T item);

        protected abstract Task OnAdd();

        protected virtual void OnSelectedValueChanged()
        {
            EditCommand.RaiseCanExecuteChanged();
            DeleteCommand.RaiseCanExecuteChanged();
        }
    }

    public class BindingProxy : Freezable
    {
        protected override Freezable CreateInstanceCore() => new BindingProxy();

        public object Data
        {
            get => GetValue(DataProperty);
            set => SetValue(DataProperty, value);
        }

        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register(nameof(Data), typeof(object), typeof(BindingProxy));
    }
}
