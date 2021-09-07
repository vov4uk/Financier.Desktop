using Financier.DataAccess.Data;
using Prism.Mvvm;
using System.Collections.ObjectModel;

namespace Financier.Desktop.ViewModel
{
    public abstract class EntityBaseVM<T> : BindableBase
    where T : Entity
    {
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
    }
}
