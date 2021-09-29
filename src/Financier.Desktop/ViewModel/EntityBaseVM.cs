using Financier.DataAccess.Data;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;

namespace Financier.Desktop.ViewModel
{
    public abstract class EntityBaseVM<T> : BindableBase
    where T : Entity
    {
        private T _selectedValue;

        private DelegateCommand _addCommand;

        private DelegateCommand _deleteCommand;

        private DelegateCommand _editCommand;

        private RangeObservableCollection<T> _entities;

        public RangeObservableCollection<T> Entities
        {
            get
            {
                if (_entities == null)
                {
                    _entities = new RangeObservableCollection<T>();
                    RaisePropertyChanged(nameof(Entities));
                }

                return _entities;
            }
            set
            {
                _entities = value;
                RaisePropertyChanged(nameof(Entities));
            }
        }

        public event EventHandler AddRaised;

        public event EventHandler<T> DeleteRaised;

        public event EventHandler<T> EditRaised;

        public DelegateCommand AddCommand
        {
            get
            {
                return _addCommand ??= new DelegateCommand(() => AddRaised?.Invoke(this, EventArgs.Empty));
            }
        }

        public DelegateCommand DeleteCommand
        {
            get
            {
                return _deleteCommand ??= new DelegateCommand(() => DeleteRaised?.Invoke(this, SelectedValue), () => SelectedValue != null);
            }
        }

        public DelegateCommand EditCommand
        {
            get
            {
                return _editCommand ??= new DelegateCommand(() => EditRaised?.Invoke(this, SelectedValue), () => SelectedValue != null);
            }
        }

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
