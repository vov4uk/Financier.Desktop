using System;
using Financier.Desktop.MonoWizard.View;
using Financier.Desktop.MonoWizard.ViewModel;
using Financier.Desktop.ViewModel;
using Financier.Adapter;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Ribbon;
using System.Windows.Forms;
using DataFormats = System.Windows.DataFormats;

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

        FinancierVM VM { get; }

        public MainWindow()
        {
            AppDomain.CurrentDomain.UnhandledException += (_, e ) => Logger.Error((Exception)e.ExceptionObject);
            InitializeComponent();
            VM = new FinancierVM();

            DataContext = VM;
            Logger.Info("App started");
        }

        private async void OpenBackup_OnClick(object sender, RoutedEventArgs e)
        {
            using var openFileDialog = new OpenFileDialog
            {
                Multiselect = false,
                Filter = $"Backup files ({BackupFormat})|{BackupFormat}"
            };
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Logger.Info($"Opened backup : {openFileDialog.FileName}");
                await VM.OpenBackup(openFileDialog.FileName);
            }
        }

        private async void RibbonWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var backup = Directory.EnumerateFiles(@$"C:\Users\{Environment.UserName}\Dropbox\apps\FinancierAndroid", BackupFormat).OrderByDescending(x => x).FirstOrDefault();
            if (!string.IsNullOrEmpty(backup))
            {
                await VM.OpenBackup(backup);
                VM.CurrentPage = VM.Pages.OfType<BlotterVM>().First();
            }
        }

        private async void Mono_Click(object sender, RoutedEventArgs e)
        {
            using var openFileDialog = new OpenFileDialog
            {
                Multiselect = false,
                Filter = "CSV files (*.csv)|*.csv"
            };
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var fileName = openFileDialog.FileName;
                var dialog = new MonoWizardWindow();

                var accounts = VM.Pages.OfType<AccountsVM>().First().Entities.ToList();
                var currencies = VM.Pages.OfType<CurrenciesVM>().First().Entities.ToList();
                var locations = VM.Pages.OfType<LocationsVM>().First().Entities.ToList();
                var categories = VM.Pages.OfType<CategoriesVM>().First().Entities.ToList();

                var viewModel = new MonoWizardViewModel(accounts, currencies, locations, categories, fileName);
                await viewModel.LoadTransactions();
                viewModel.RequestClose += async (o, args) =>
                {
                    dialog.Close();
                    if (args)
                    {

                        var monoToImport = viewModel.TransactionsToImport.Where(item =>
                        !VM.Blotter.Entities.Any(x =>
                        x.from_account_id == item.FromAccountId &&
                        x.datetime == item.DateTime &&
                        x.from_amount == item.FromAmount)).ToList();

                        var duplicatesCount = viewModel.TransactionsToImport.Count - monoToImport.Count;

                        await VM.ImportMonoTransactions(monoToImport);
                        System.Windows.Forms.MessageBox.Show($"Imported {monoToImport.Count} transactions."
                            + ((duplicatesCount > 0) ? $" Skiped {duplicatesCount} duplicates." : string.Empty));

                        Logger.Info($"Imported {monoToImport.Count} transactions. Found duplicates : {duplicatesCount}");
                    }
                };
                dialog.DataContext = viewModel;
                dialog.ShowDialog();
            }
        }

        private async void SaveBackup_Click(object sender, RoutedEventArgs e)
        {
            using SaveFileDialog dialog = new SaveFileDialog
            {
                Filter = $"Backup files ({BackupFormat})|{BackupFormat}",
                FileName = Path.Combine(Path.GetDirectoryName(VM.OpenBackupPath), BackupWriter.GenerateFileName())
            };
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                await VM.SaveBackup(dialog.FileName);

                System.Windows.Forms.MessageBox.Show($"Backup done. Saved {dialog.FileName}");
                Logger.Info($"Backup done. Saved {dialog.FileName}");
            }
        }

        private async void Grid_Drop(object sender, System.Windows.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = e.Data.GetData(DataFormats.FileDrop, true) as string[];
                if (files?.Any(x => Path.GetExtension(x) == Backup) == true)
                {
                    Logger.Info($"Drag & drop backup  : {files.FirstOrDefault(x => Path.GetExtension(x) == Backup)}");
                    await VM.OpenBackup(files.FirstOrDefault(x => Path.GetExtension(x) == Backup));
                }
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
    }
}
