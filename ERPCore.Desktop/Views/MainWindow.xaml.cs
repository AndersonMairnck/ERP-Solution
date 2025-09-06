using System.Windows;

namespace ERPCore.Desktop.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MenuItemExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MenuItemProducts_Click(object sender, RoutedEventArgs e)
        {
            var productsWindow = new ProductsWindow();
            productsWindow.Owner = this;
            productsWindow.ShowDialog();
        }

        private void MenuItemPDV_Click(object sender, RoutedEventArgs e)
        {
            var pdvWindow = new PDVWindow();
            pdvWindow.Owner = this;
            pdvWindow.ShowDialog();
        }

        private void MenuItemReports_Click(object sender, RoutedEventArgs e)
        {
            var reportsWindow = new ReportsWindow();
            reportsWindow.Owner = this;
            reportsWindow.ShowDialog();
        }
        private void MenuItemAccounts_Click(object sender, RoutedEventArgs e)
        {
            //var accountsWindow = new FinancialAccountsWindow();
            //accountsWindow.Owner = this;
            //accountsWindow.ShowDialog();
        }

        private void MenuItemTransactions_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Módulo de Transações em desenvolvimento", "Info",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void MenuItemSuppliers_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Módulo de Fornecedores em desenvolvimento", "Info",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}