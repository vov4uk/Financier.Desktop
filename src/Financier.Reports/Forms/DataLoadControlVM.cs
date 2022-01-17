using fcrd.Properties;
using System;
using System.Windows.Forms;
using System.Windows.Input;

namespace fcrd
{
    public class DataLoadControlVM : BaseViewModel
    {
        private RelayCommand _loadDataCommand;
        private RelayCommand _cancelLoadDataCommand;
        private RelayCommand _selectBackUpDirCommand;

        public string BackupDir
        {
            get => ExSettings.LastBackupDir;
            set
            {
                ExSettings.LastBackupDir = value;
                this.OnPropertyChanged(nameof(BackupDir));
            }
        }

        public event DataLoadControlVM.DataLoadedDelegate DataLoaded;

        public void OnDataLoaded()
        {
            if (this.DataLoaded == null)
                return;
            this.DataLoaded(this);
        }

        public ICommand LoadDataCommand => (ICommand)(this._loadDataCommand ?? (this._loadDataCommand = new RelayCommand((Action<object>)(param => this.LoadData()))));

        public void LoadData()
        {
            new DataLoader(Settings.Default.LastBackupDir).Start();
            this.OnDataLoaded();
        }

        public ICommand CancelLoadDataCommand => (ICommand)(this._cancelLoadDataCommand ?? (this._cancelLoadDataCommand = new RelayCommand((Action<object>)(param => this.OnDataLoaded()))));

        public ICommand SelectBackUpDirtCommand => (ICommand)(this._selectBackUpDirCommand ?? (this._selectBackUpDirCommand = new RelayCommand((Action<object>)(p => this.SelectBackUpDir()))));

        public void SelectBackUpDir()
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() != DialogResult.OK)
                return;
            this.BackupDir = folderBrowserDialog.SelectedPath;
        }

        public delegate void DataLoadedDelegate(object sender);
    }
}