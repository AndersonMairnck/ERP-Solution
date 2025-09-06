using System;
using System.Windows;
using ERPCore.Models;

namespace ERPCore.Desktop.Views
{
    public partial class ProductEditWindow : Window
    {
        private Product _currentProduct;

        public Product CurrentProduct
        {
            get => _currentProduct;
            set
            {
                _currentProduct = value;
                UpdateWindowTitle();
            }
        }

        public ProductEditWindow(Product product = null)
        {
            InitializeComponent();

            CurrentProduct = product ?? new Product();
            DataContext = this;

            UpdateWindowTitle();
        }

        private void UpdateWindowTitle()
        {
            TitleText.Text = CurrentProduct.Id == 0 ? "NOVO PRODUTO" : "EDITAR PRODUTO";
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateProduct())
            {
                DialogResult = true;
                Close();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private bool ValidateProduct()
        {
            // Validar código
            if (string.IsNullOrWhiteSpace(CurrentProduct.Code))
            {
                MessageBox.Show("Código do produto é obrigatório.", "Validação",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                CodeTextBox.Focus();
                return false;
            }

            // Validar nome
            if (string.IsNullOrWhiteSpace(CurrentProduct.Name))
            {
                MessageBox.Show("Nome do produto é obrigatório.", "Validação",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                NameTextBox.Focus();
                return false;
            }

            // Validar preços
            if (CurrentProduct.PriceRetail <= 0)
            {
                MessageBox.Show("Preço de varejo deve ser maior que zero.", "Validação",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                PriceRetailTextBox.Focus();
                return false;
            }

            if (CurrentProduct.PriceWholesale <= 0)
            {
                MessageBox.Show("Preço de atacado deve ser maior que zero.", "Validação",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                PriceWholesaleTextBox.Focus();
                return false;
            }

            // Validar estoque
            if (CurrentProduct.StockQuantity < 0)
            {
                MessageBox.Show("Estoque não pode ser negativo.", "Validação",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                StockQuantityTextBox.Focus();
                return false;
            }

            if (CurrentProduct.MinimumStock < 0)
            {
                MessageBox.Show("Estoque mínimo não pode ser negativo.", "Validação",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                MinimumStockTextBox.Focus();
                return false;
            }

            return true;
        }
    }
}