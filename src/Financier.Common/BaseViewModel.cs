using Financier.Common.Model;
using Financier.DataAccess.Abstractions;
using Mvvm.Async;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Financier.Common
{
    public abstract class BaseViewModel<T> : BindableBase, IDataRefresh
        where T : BaseModel, new()
    {
        protected readonly IFinancierDatabase db;
        private IAsyncCommand _refreshDataCommand;
        private ObservableCollection<T> _entities;

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

        protected BaseViewModel(IFinancierDatabase financierDatabase)
        {
            this.db = financierDatabase;
        }

        public IAsyncCommand RefreshDataCommand => _refreshDataCommand ?? (_refreshDataCommand = new AsyncCommand(RefreshData));

        protected abstract Task RefreshData();
    }
}
