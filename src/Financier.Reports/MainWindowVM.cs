using Financier.Reports.Common;
using Financier.Reports.Forms;
using System.Windows.Input;

namespace Financier.Reports
{
    public class MainWindowVM : BaseViewModel
    {
        private RelayCommand _openloadDataCommand;
        private RelayCommand _clearDataCommand;
        private bool _isDataLoaded;
        private DataLoadControlVM _dataLoad;

        public ICommand OpenLoadDataCommand => _openloadDataCommand ?? (_openloadDataCommand = new RelayCommand(param => DataLoadInitLoad()));

        public ICommand ClearDataCommand => _clearDataCommand ?? (_clearDataCommand = new RelayCommand(param => DB.TruncateTables()));

        public bool IsDataLoaded
        {
            get => _isDataLoaded;
            set
            {
                if (_isDataLoaded == value)
                    return;
                _isDataLoaded = value;
                OnPropertyChanged(nameof(IsDataLoaded));
            }
        }

        public DataLoadControlVM DataLoad
        {
            get => _dataLoad;
            set
            {
                if (_dataLoad == value)
                    return;
                _dataLoad = value;
                OnPropertyChanged(nameof(DataLoad));
            }
        }

        public MainWindowVM() => DataLoadInitLoad();

        private void DataLoadInitLoad()
        {
            DataLoad = new DataLoadControlVM();
            DataLoad.DataLoaded += new DataLoadControlVM.DataLoadedDelegate(DataLoadDataLoaded);
            IsDataLoaded = false;
        }

        private void DataLoadDataLoaded(object sender)
        {
            IsDataLoaded = true;
            DataLoad = null;
        }
    }
}