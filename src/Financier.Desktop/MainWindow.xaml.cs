using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Ribbon;
using Financier.Adapter;
using Financier.DataAccess;
using Financier.Desktop.Helpers;
using Financier.Desktop.Helpers.BankHelper;
using Financier.Desktop.Properties;
using Financier.Desktop.ViewModel;
using Microsoft.Win32;
using DataFormats = System.Windows.DataFormats;

namespace Financier.Desktop
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public partial class MainWindow : RibbonWindow
    {
        private const string BackupFormat = "*.backup";
        private const string Backup = ".backup";
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        static MainWindow()
        {
            System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";

            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;
        }

        MainWindowVM ViewModel { get; }

        public MainWindow()
        {
            InitializeComponent();
            ViewModel = new MainWindowVM(new DialogHelper(), new FinancierDatabaseFactory(), new EntityReader(), new BackupWriter(), new ToastNotifierWrapper(), new BankHelperFactory());

            DataContext = ViewModel;
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            var buildDate = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Local)
               .AddDays(version.Build).AddSeconds(version.Revision * 2);
            Title = $"Financier Desktop v.{buildDate.Date.ToShortDateString()}";
            Logger.Info("App started");
        }

        private void RibbonWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var bakupFolder = Settings.Default.DefaultBackupDir ?? @$"C:\Users\{Environment.UserName}\Dropbox\apps\Financisto Holo";
            ViewModel.DefaultBackupDirectory = Settings.Default.DefaultBackupDir;
            ViewModel.ExchangeRatesSettings = Settings.Default.ExchangeRatesSettings;
            if (Directory.Exists(bakupFolder))
            {
                var backupFile = Directory.EnumerateFiles(bakupFolder, BackupFormat).OrderByDescending(x => x).FirstOrDefault();
                if (!string.IsNullOrEmpty(backupFile) && File.Exists(backupFile))
                {
                    Logger.Info($"Loaded backup : {backupFile}");
                    Task.Run(() => ViewModel.OpenBackup(backupFile))
                        .ContinueWith((t) => ViewModel.RefreshExchangeRatesCommand.ExecuteAsync());
                }
            }
        }

        private void Grid_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = e.Data.GetData(DataFormats.FileDrop, true) as string[];
                if (files?.Any(x => Path.GetExtension(x) == Backup) == true)
                {
                    Logger.Info($"Drag & drop backup : {files.FirstOrDefault(x => Path.GetExtension(x) == Backup)}");
                    Task.Run(() => ViewModel.OpenBackup(files.FirstOrDefault(x => Path.GetExtension(x) == Backup)));
                }
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void RibbonApplicationMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFolderDialog openFileDialog = new OpenFolderDialog
            {
                Multiselect = false
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var currentFolder = openFileDialog.FolderName;
                Settings.Default.DefaultBackupDir = currentFolder;
                Settings.Default.Save();
                ViewModel.DefaultBackupDirectory = currentFolder;
            }
        }
    }
}
