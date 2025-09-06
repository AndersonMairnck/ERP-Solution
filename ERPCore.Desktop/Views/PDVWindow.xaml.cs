using System;
using System.Windows;
using System.Windows.Input;
using System.Collections.Generic;
using System.Linq;
using ERPCore.Desktop.Services;
using ERPCore.Models;

namespace ERPCore.Desktop.Views
{
    public partial class PDVWindow : Window
    {
        private readonly IApiService _apiService;
        private List<SaleItem> _saleItems;
        private decimal _subtotal;
        private decimal _discount;
        private decimal _total;

        public PDVWindow()
        {
            InitializeComponent();
            _apiService = new ApiService();
            _saleItems = new List<SaleItem>();

            Loaded += PDVWindow_Loaded;
            cmbPaymentMethod.SelectionChanged += CmbPaymentMethod_SelectionChanged;
        }

        private void PDVWindow_Loaded(object sender, RoutedEventArgs e)
        {
            txtProductCode.Focus();
            UpdateTotals();
        }

        private void CmbPaymentMethod_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var method = ((System.Windows.Controls.ComboBoxItem)cmbPaymentMethod.SelectedItem).Content.ToString();
            pnlCash.Visibility = method == "Dinheiro" ? Visibility.Visible : Visibility.Collapsed;
        }

        private async void TxtProductCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                await AddProductToSaleAsync();
            }
        }

        private async void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            await AddProductToSaleAsync();
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            txtProductCode.Clear();
            txtProductCode.Focus();
        }

        private async Task AddProductToSaleAsync()
        {
            var code = txtProductCode.Text.Trim();
            if (string.IsNullOrEmpty(code)) return;

            try
            {
                var product = await _apiService.GetProductByCodeAsync(code);
                if (product != null)
                {
                    AddOrUpdateSaleItem(product);
                    txtProductCode.Clear();
                    txtProductCode.Focus();
                }
                else
                {
                    MessageBox.Show("Produto não encontrado.", "Erro",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro: {ex.Message}", "Erro",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddOrUpdateSaleItem(Product product)
        {
            var existingItem = _saleItems.FirstOrDefault(item => item.ProductId == product.Id);

            if (existingItem != null)
            {
                existingItem.Quantity++;
                existingItem.TotalPrice = existingItem.UnitPrice * existingItem.Quantity;
            }
            else
            {
                _saleItems.Add(new SaleItem
                {
                    ProductId = product.Id,
                    ProductCode = product.Code,
                    ProductName = product.Name,
                    UnitPrice = product.PriceRetail,
                    Quantity = 1,
                    TotalPrice = product.PriceRetail
                });
            }

            RefreshSaleItemsGrid();
            UpdateTotals();
        }

        private void BtnRemoveItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.Button button && button.DataContext is SaleItem item)
            {
                _saleItems.Remove(item);
                RefreshSaleItemsGrid();
                UpdateTotals();
            }
        }

        private void RefreshSaleItemsGrid()
        {
            GridSaleItems.ItemsSource = null;
            GridSaleItems.ItemsSource = _saleItems;
        }

        private void UpdateTotals()
        {
            _subtotal = _saleItems.Sum(item => item.TotalPrice);

            if (decimal.TryParse(txtDiscount.Text, out decimal discount))
                _discount = discount;
            else
                _discount = 0;

            _total = _subtotal - _discount;

            txtSubtotal.Text = _subtotal.ToString("C");
            txtTotal.Text = _total.ToString("C");

            UpdateChangeCalculation();
        }

        private void UpdateChangeCalculation()
        {
            if (decimal.TryParse(txtAmountReceived.Text, out decimal amountReceived))
            {
                var change = amountReceived - _total;
                txtChange.Text = change.ToString("C");
            }
            else
            {
                txtChange.Text = "R$ 0,00";
            }
        }

        private void TxtDiscount_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            UpdateTotals();
        }

        private void TxtAmountReceived_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            UpdateChangeCalculation();
        }

        private void TxtProductCode_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            // Implementar busca automática se necessário
        }

        private async void BtnFinalizeSale_Click(object sender, RoutedEventArgs e)
        {
            if (_saleItems.Count == 0)
            {
                MessageBox.Show("Adicione pelo menos um produto à venda.", "Aviso",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_total <= 0)
            {
                MessageBox.Show("O total da venda deve ser maior que zero.", "Aviso",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var sale = new Sale
                {
                    SaleItems = _saleItems,
                    TotalAmount = _subtotal,
                    Discount = _discount,
                    FinalAmount = _total,
                    PaymentMethod = ((System.Windows.Controls.ComboBoxItem)cmbPaymentMethod.SelectedItem).Content.ToString(),
                    ClientName = txtClientName.Text,
                    ClientDocument = txtClientDocument.Text,
                    CreatedAt = DateTime.Now,
                    Status = "COMPLETED"
                };

                var result = await _apiService.CreateSaleAsync(sale);
                if (result != null)
                {
                    MessageBox.Show($"Venda finalizada com sucesso!\nNúmero: {result.SaleCode}\nTotal: {result.FinalAmount:C}",
                        "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);

                    ResetSale();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao finalizar venda: {ex.Message}", "Erro",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ResetSale()
        {
            _saleItems.Clear();
            RefreshSaleItemsGrid();

            txtDiscount.Text = "0,00";
            txtClientName.Clear();
            txtClientDocument.Clear();
            txtAmountReceived.Text = "0,00";
            cmbPaymentMethod.SelectedIndex = 0;

            UpdateTotals();
            txtProductCode.Focus();
        }
    }
}