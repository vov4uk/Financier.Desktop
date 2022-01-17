using System;
using System.Windows.Input;

namespace fcrd
{
    public class MainWindowVM : BaseViewModel
    {
        private RelayCommand _openloadDataCommand;
        private RelayCommand _clearDataCommand;
        private bool _isDataLoaded;
        private DataLoadControlVM _dataLoad;

        public ICommand OpenLoadDataCommand => (ICommand)(this._openloadDataCommand ?? (this._openloadDataCommand = new RelayCommand((Action<object>)(param => this.DataLoadInitLoad()))));

        public ICommand ClearDataCommand => (ICommand)(this._clearDataCommand ?? (this._clearDataCommand = new RelayCommand((Action<object>)(param => DB.TruncateTables()))));

        public bool IsDataLoaded
        {
            get => this._isDataLoaded;
            set
            {
                if (this._isDataLoaded == value)
                    return;
                this._isDataLoaded = value;
                this.OnPropertyChanged(nameof(IsDataLoaded));
            }
        }

        public DataLoadControlVM DataLoad
        {
            get => this._dataLoad;
            set
            {
                if (this._dataLoad == value)
                    return;
                this._dataLoad = value;
                this.OnPropertyChanged(nameof(DataLoad));
            }
        }

        public MainWindowVM() => this.DataLoadInitLoad();

        private void DataLoadInitLoad()
        {
            this.DataLoad = new DataLoadControlVM();
            this.DataLoad.DataLoaded += new DataLoadControlVM.DataLoadedDelegate(this.DataLoadDataLoaded);
            this.IsDataLoaded = false;
        }

        private void DataLoadDataLoaded(object sender)
        {
            this.IsDataLoaded = true;
            this.DataLoad = (DataLoadControlVM)null;
        }
    }
}