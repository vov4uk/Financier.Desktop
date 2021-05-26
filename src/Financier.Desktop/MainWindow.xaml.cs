using Financier.Desktop.Entities;
using Financier.Desktop.ViewModel;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls.Ribbon;

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
            var openFileDialog = new OpenFileDialog
            {
                Multiselect = false,
                Filter = "Backup files (*.backup)|*.backup"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                await VM.GetEntities(openFileDialog.FileName);
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
            await VM.GetEntities(@"C:\Users\vkhmelovskyi\Desktop\Financisto\20210428_185525_708.backup");
            UIPanel.Children.Add(new Blotter(VM.Transactions));
#endif
        }
    }
}
