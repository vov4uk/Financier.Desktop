﻿using Financier.DataAccess.Data;
using Financier.Desktop.MonoWizard.View;
using Financier.Desktop.MonoWizard.ViewModel;
using Financier.Desktop.ViewModel;
using FinancistoAdapter;
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

        static MainWindow()
        {
            System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";

            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;
        }

        FinancierVM VM { get; }
        public MainWindow()
        {
            InitializeComponent();
            VM = new FinancierVM();

            DataContext = VM;
        }

        private async void OpenBackup_OnClick(object sender, RoutedEventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog
            {
                Multiselect = false,
                Filter = "Backup files (*.backup)|*.backup"
            })
            {
                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    await VM.OpenBackup(openFileDialog.FileName);
                }
            }
        }

        private async void RibbonWindow_Loaded(object sender, RoutedEventArgs e)
        {
            #if DEBUG
            await VM.OpenBackup(@"C:\D\Financisto\20210428_185525_708.backup");
            VM.CurrentPage = VM.Pages.OfType<BlotterVM>().First();
            #endif
        }

        private async void Mono_Click(object sender, RoutedEventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog
            {
                Multiselect = false,
                Filter = "CSV files (*.csv)|*.csv"
            })
            {
                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    var fileName = openFileDialog.FileName;
                    var dialog = new MonoWizardWindow();
                    var viewModel = new MonoWizardViewModel(VM.Pages.OfType<AccountsVM>().First().Entities.OfType<Account>().ToList() , fileName);
                    await viewModel.LoadTransactions();
                    viewModel.RequestClose += async (sender, args) =>
                    {
                        dialog.Close();
                        if (args)
                        {
                            var monoToImport = viewModel.TransactionsToImport;
                            await VM.ImportMonoTransactions(viewModel.MonoBankAccount.Id, monoToImport);
                        }
                    };
                    dialog.DataContext = viewModel;
                    dialog.ShowDialog();
                }
            }
        }

        private async void SaveBackup_Click(object sender, RoutedEventArgs e)
        {
            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                dialog.Filter = "Backup files (*.backup)|*.backup";
                dialog.FileName = Path.Combine(Path.GetDirectoryName(VM.OpenBackupPath), BackupWriter.generateFileName());
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    await VM.SaveBackup(dialog.FileName);

                    System.Windows.Forms.MessageBox.Show("Backup done.");
                }
            }
        }

        private async void Grid_Drop(object sender, System.Windows.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = e.Data.GetData(DataFormats.FileDrop, true) as string[];
                if (files?.Any(x => Path.GetExtension(x) == ".backup") == true)
                {
                    await VM.OpenBackup(files.FirstOrDefault(x => Path.GetExtension(x) == ".backup"));
                }
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
    }
}
