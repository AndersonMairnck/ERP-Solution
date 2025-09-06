using System;
using System.Windows;
using ERPCore.Models;

namespace ERPCore.Desktop.Views
{
    public partial class ProductEditWindow : Window
    {
        public Product Product { get; set; }

        public ProductEditWindow(Product product = null)
        {
            InitializeComponent();
            Product = product ?? new Product();
            LoadProductData();
        }

        private void LoadProductData()
        {
            if (Product.Id == 0)
            {
                txtTitle.Text = "NOVO PRODUTO";
            }
            else
            {
                txtTitle.Text = "EDITAR PRODUTO";
                txtCode.Text = Product.Code;
                txtName.Text = Product.Name;
                txtDescription.Text = Product.Description;
                txtPriceRetail.Text = Product.PriceRetail.ToString();
                txtPriceWholesale.Text = Product.PriceWholesale.ToString();
                txtStockQuantity.Text = Product.StockQuantity.ToString();
                txtMinimumStock.Text = Product.MinimumStock.ToString();
                chkActive.IsChecked = Product.Active;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateProduct())
            {
                SaveProductData();
                DialogResult = true;
                Close();
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private bool ValidateProduct()
        {
            if (string.IsNullOrWhiteSpace(txtCode.Text))
            {
                MessageBox.Show("Código do produto é obrigatório.", "Validação",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtCode.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Nome do produto é obrigatório.", "Validação",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtName.Focus();
                return false;
            }

            return true;
        }

        private void SaveProductData()
        {
            Product.Code = txtCode.Text;
            Product.Name = txtName.Text;
            Product.Description = txtDescription.Text;

            if (decimal.TryParse(txtPriceRetail.Text, out decimal priceRetail))
                Product.PriceRetail = priceRetail;

            if (decimal.TryParse(txtPriceWholesale.Text, out decimal priceWholesale))
                Product.PriceWholesale = priceWholesale;

            if (int.TryParse(txtStockQuantity.Text, out int stockQuantity))
                Product.StockQuantity = stockQuantity;

            if (int.TryParse(txtMinimumStock.Text, out int minimumStock))
                Product.MinimumStock = minimumStock;

            Product.Active = chkActive.IsChecked ?? false;

            if (Product.Id == 0)
            {
                Product.CreatedAt = DateTime.Now;
            }
            else
            {
                Product.UpdatedAt = DateTime.Now;
            }
        }
    }
}