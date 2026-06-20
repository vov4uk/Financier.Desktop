using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Ribbon;
using Financier.Adapter;
using Financier.Common.Entities;
using Financier.Common.Localization;
using Financier.DataAccess;
using Financier.Desktop.Data;
using Financier.Desktop.Helpers;
using Financier.Desktop.Helpers.BankHelper;
using Financier.Desktop.Services;
using Financier.Desktop.ViewModel;
using Ookii.Dialogs.Wpf;
using DataFormats = System.Windows.DataFormats;

namespace Financier.Desktop
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public partial class MainWindow : RibbonWindow
    {
        private const string BackupFormat = "*.backup";
        private const string Backup = ".backup";
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly ToastNotifierWrapper notificator = new ToastNotifierWrapper();

        MainWindowVM ViewModel { get; }

        public MainWindow()
        {
            InitializeComponent();
            ViewModel = new MainWindowVM(new DialogHelper(), new FinancierDatabaseFactory(), new EntityReader(), new BackupWriter(), notificator, new BankHelperFactory(), new UpdateService());

            DataContext = ViewModel;
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            Title = $"Financier Desktop v.{version}";
            Logger.Info("App started");
        }

        private async void RibbonWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                SettingsService.Current.Load();
                ArgumentNullException.ThrowIfNull(SettingsService.Current.Settings);
                ArgumentNullException.ThrowIfNull(SettingsService.Current.Settings.ExchangeRates);
                ArgumentNullException.ThrowIfNull(SettingsService.Current.Settings.General);
            }
            catch
            {
                notificator.ShowWarning(LocalizationService.Instance.settings_corrupted);
                SettingsService.Current.Settings = new SettingsDto()
                {
                    ExchangeRates = new SettingsExchangeRates
                    {
                        Provider = ExchangeRatesProviders.None,
                        UpdateOnStart = false,
                    },
                    General = new SettingsGeneralDto
                    {
                        CheckForUpdatesOnStart = true
                    }
                };
                SettingsService.Current.Save();
            }

            LocalizationService.Instance.ApplyLanguage(SettingsService.Current.Settings?.General.Language ?? Common.Localization.Language.English);
            var bakupFolder = SettingsService.Current.DefaultBackupDir ?? @$"C:\Users\{Environment.UserName}\Dropbox\apps\Financisto Holo";
            ViewModel.DefaultBackupDirectory = SettingsService.Current.DefaultBackupDir;

            if (Directory.Exists(bakupFolder))
            {
                var backupFile = Directory.EnumerateFiles(bakupFolder, BackupFormat).OrderByDescending(x => x).FirstOrDefault();
                if (!string.IsNullOrEmpty(backupFile) && File.Exists(backupFile))
                {
                    Logger.Info($"Automatically loaded backup : {backupFile}");
                    await Task.Run(() => ViewModel.OpenBackup(backupFile));
                }

                if (SettingsService.Current.Settings?.General.CheckForUpdatesOnStart == true)
                {
                    await ViewModel.CheckForUpdateCommand.ExecuteAsync();
                }
            }
        }

        private void Grid_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (e.Data.GetData(DataFormats.FileDrop, true) as string[]);
                var path = files.FirstOrDefault(x => Path.GetExtension(x) == Backup);
                if (!string.IsNullOrEmpty(path))
                {
                    Logger.Info($"Drag & drop backup : {path}");
                    Task.Run(async () => await ViewModel.OpenBackup(path));
                }
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void RibbonApplicationMenuItem_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog openFileDialog = new VistaFolderBrowserDialog
            {
                Multiselect = false,
                ShowNewFolderButton = false
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var currentFolder = openFileDialog.SelectedPath;
                SettingsService.Current.DefaultBackupDir = currentFolder;
                SettingsService.Current.Save();
                ViewModel.DefaultBackupDirectory = currentFolder;
            }
        }
    }
}
