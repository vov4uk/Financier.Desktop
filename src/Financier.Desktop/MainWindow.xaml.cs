using FinancierDesktop.Entities;
using FinancierDesktop.ViewModel;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls.Ribbon;

namespace FinancierDesktop
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

        private void RestoreBackup_OnClick(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Multiselect = false,
                Filter = "Backup files (*.backup)|*.backup"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                VM.GetEntities(openFileDialog.FileName);
            }
            UIPanel.Children.Add(new Accounts(VM.Accounts));
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
    }
}
