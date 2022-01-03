using Financier.DataAccess.Data;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Financier.Desktop.ViewModel
{
    public abstract class EntityBaseVM<T> : BindableBase
    where T : Entity
    {
        private DelegateCommand _addCommand;
        private DelegateCommand _deleteCommand;
        private DelegateCommand _editCommand;
        private ObservableCollection<T> _entities;
        private T _selectedValue;
        public EntityBaseVM(IEnumerable<T> entities)
        {
            _entities = new ObservableCollection<T>(entities);
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

        public ObservableCollection<T> Entities
        {
            get
            {
                if (_entities == null)
                {
                    _entities = new ObservableCollection<T>();
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
