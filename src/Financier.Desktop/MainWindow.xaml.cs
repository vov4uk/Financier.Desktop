using System;
using Financier.Desktop.ViewModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Ribbon;
using DataFormats = System.Windows.DataFormats;
using Financier.Desktop.Helpers;
using Financier.DataAccess;
using Financier.Adapter;
using Financier.DataAccess.View;

namespace Financier.Desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
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

        FinancierVM ViewModel { get; }

        public MainWindow()
        {
            InitializeComponent();
            ViewModel = new FinancierVM(new DialogHelper(), new FinancierDatabaseFactory(), new EntityReader(), new BackupWriter());

            DataContext = ViewModel;
            Logger.Info("App started");
        }

        private async void RibbonWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var backup = Directory.EnumerateFiles(@$"C:\Users\{Environment.UserName}\Dropbox\apps\FinancierAndroid", BackupFormat).OrderByDescending(x => x).FirstOrDefault();
            if (!string.IsNullOrEmpty(backup))
            {
                Logger.Info($"Loaded backup : {backup}");
                await ViewModel.OpenBackup(backup);
                ViewModel.MenuNavigateCommand.Execute(typeof(BlotterTransactions));
            }
        }

        private async void Grid_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = e.Data.GetData(DataFormats.FileDrop, true) as string[];
                if (files?.Any(x => Path.GetExtension(x) == Backup) == true)
                {
                    Logger.Info($"Drag & drop backup : {files.FirstOrDefault(x => Path.GetExtension(x) == Backup)}");
                    await ViewModel.OpenBackup(files.FirstOrDefault(x => Path.GetExtension(x) == Backup));
                }
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
