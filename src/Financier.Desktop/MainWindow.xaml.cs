using Financier.Desktop.Entities;
using Financier.Desktop.ViewModel;
using FinancistoAdapter;
using MonoWizard;
using MonoWizard.ViewModel;
using System.IO;
using System.Windows;
using System.Windows.Controls.Ribbon;
using System.Windows.Forms;

namespace Financier.Desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : RibbonWindow
    {
        FinancierVM VM { get; }
        public MainWindow()
        {
            InitializeComponent();
            VM = new FinancierVM();
            DataContext = VM;
        }

        private async void RestoreBackup_OnClick(object sender, RoutedEventArgs e)
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
            Accounts_Click(null, null);
        }

        private void Accounts_Click(object sender, RoutedEventArgs e)
        {
            UIPanel.Children.Clear();
            UIPanel.Children.Add(new Accounts(VM.Accounts));
        }

        private void Categories_Click(object sender, RoutedEventArgs e)
        {
            UIPanel.Children.Clear();
            UIPanel.Children.Add(new Categories(VM.Categories));
        }

        private void Projects_Click(object sender, RoutedEventArgs e)
        {
            UIPanel.Children.Clear();
            UIPanel.Children.Add(new Projects(VM.Projects));
        }

        private void Payees_Click(object sender, RoutedEventArgs e)
        {
            UIPanel.Children.Clear();
            UIPanel.Children.Add(new Payees(VM.Payees));
        }

        private void Currencies_Click(object sender, RoutedEventArgs e)
        {
            UIPanel.Children.Clear();
            UIPanel.Children.Add(new Currencies(VM.Currencies));
        }

        private void ExchangeRates_Click(object sender, RoutedEventArgs e)
        {
            UIPanel.Children.Clear();
            UIPanel.Children.Add(new ExchangeRates(VM.Rates));
        }

        private void Locations_Click(object sender, RoutedEventArgs e)
        {
            UIPanel.Children.Clear();
            UIPanel.Children.Add(new Locations(VM.Locations));
        }
        
        private void Budget_Click(object sender, RoutedEventArgs e)
        {
            UIPanel.Children.Clear();
            UIPanel.Children.Add(new Budgets(VM.Budgets));
        }

        private void Blotter_Click(object sender, RoutedEventArgs e)
        {
            UIPanel.Children.Clear();
            UIPanel.Children.Add(new Blotter(VM.Transactions));
        }

        private async void RibbonWindow_Loaded(object sender, RoutedEventArgs e)
        {
#if DEBUG
            UIPanel.Children.Clear();
            await VM.OpenBackup(@"C:\Users\vkhmelovskyi\Desktop\Financisto\20210527_005841_365.backup");
            UIPanel.Children.Add(new Blotter(VM.Transactions));
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
                    var viewModel = new MonoWizardViewModel(VM.Accounts, fileName);
                    await viewModel.LoadTransactions();
                    viewModel.RequestClose += (sender, args) =>
                    {
                        dialog.Close();
                    };
                    dialog.DataContext = viewModel;
                    dialog.ShowDialog();
                    var monoToImport = viewModel.TransactionsToImport;
                    await VM.ImportMonoTransactions(viewModel.MonoBankAccount.Id, monoToImport);
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
    }
}
