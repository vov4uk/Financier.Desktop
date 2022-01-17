using Financier.Reports.Reports.Properties;
using Financier.Reports.Common;
using Financier.Reports.DataLoad;
using System.Windows.Input;

namespace Financier.Reports.Forms
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
                OnPropertyChanged(nameof(BackupDir));
            }
        }

        public event DataLoadedDelegate DataLoaded;

        public void OnDataLoaded()
        {
            if (DataLoaded == null)
                return;
            DataLoaded(this);
        }

        public ICommand LoadDataCommand => _loadDataCommand ?? (_loadDataCommand = new RelayCommand(param => LoadData()));

        public void LoadData()
        {
            new DataLoader(Settings.Default.LastBackupDir).Start();
            OnDataLoaded();
        }

        public ICommand CancelLoadDataCommand => _cancelLoadDataCommand ?? (_cancelLoadDataCommand = new RelayCommand(param => OnDataLoaded()));

        public ICommand SelectBackUpDirtCommand => _selectBackUpDirCommand ?? (_selectBackUpDirCommand = new RelayCommand(p => SelectBackUpDir()));

        public void SelectBackUpDir()
        {
            //FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            //if (folderBrowserDialog.ShowDialog() != DialogResult.OK)
            //    return;
            //BackupDir = folderBrowserDialog.SelectedPath;
        }

        public delegate void DataLoadedDelegate(object sender);
    }
}