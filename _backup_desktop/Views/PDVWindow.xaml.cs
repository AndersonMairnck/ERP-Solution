using ERPCore.Desktop.Services;
using ERPCore.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ERPCore.Desktop.Views
{
    public partial class PDVWindow : Window
    {
        private readonly IApiService _apiService;
        private Sale _currentSale;

        public PDVWindow()
        {
            InitializeComponent();
            _apiService = App.ServiceProvider.GetService<IApiService>();
            _currentSale = new Sale { SaleItems = new List<SaleItem>() };

            DataContext = _currentSale;
            Loaded += PDVWindow_Loaded;
        }

        private void PDVWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ProductCodeTextBox.Focus();
            ProductCodeTextBox.KeyDown += ProductCodeTextBox_KeyDown;
        }

        private async void ProductCodeTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                await ScanProductAsync();
            }
        }

        private async void ScanButton_Click(object sender, RoutedEventArgs e)
        {
            await ScanProductAsync();
        }

        private async Task ScanProductAsync()
        {
            var code = ProductCodeTextBox.Text.Trim();
            if (string.IsNullOrEmpty(code)) return;

            try
            {
                var product = await _apiService.GetProductByCodeAsync(code);
                if (product != null)
                {
                    AddProductToSale(product);
                    ProductCodeTextBox.Clear();
                    ProductCodeTextBox.Focus();
                }
                else
                {
                    MessageBox.Show("Produto não encontrado!", "Erro",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro: {ex.Message}", "Erro",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddProductToSale(Product product)
        {
            var existingItem = _currentSale.SaleItems.FirstOrDefault(i => i.ProductId == product.Id);

            if (existingItem != null)
            {
                existingItem.Quantity++;
                existingItem.TotalPrice = existingItem.UnitPrice * existingItem.Quantity;
            }
            else
            {
                _currentSale.SaleItems.Add(new SaleItem
                {
                    ProductId = product.Id,
                    ProductCode = product.Code,
                    ProductName = product.Name,
                    UnitPrice = product.PriceRetail,
                    Quantity = 1,
                    TotalPrice = product.PriceRetail
                });
            }

            UpdateTotals();
            RefreshDataGrid();
        }

        private void UpdateTotals()
        {
            _currentSale.TotalAmount = _currentSale.SaleItems.Sum(i => i.TotalPrice);
            _currentSale.FinalAmount = _currentSale.TotalAmount - _currentSale.Discount;
        }

        private void RefreshDataGrid()
        {
            SaleItemsDataGrid.Items.Refresh();
        }

        private async void FinalizeSaleButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentSale.SaleItems.Count == 0)
            {
                MessageBox.Show("Adicione produtos à venda!", "Aviso",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                _currentSale.PaymentMethod = ((ComboBoxItem)PaymentMethodComboBox.SelectedItem).Content.ToString();
                _currentSale.CreatedAt = DateTime.Now;
                _currentSale.Status = "COMPLETED";

                var result = await _apiService.CreateSaleAsync(_currentSale);
                if (result != null)
                {
                    MessageBox.Show($"Venda finalizada!\nCódigo: {result.SaleCode}\nTotal: {result.FinalAmount:C}",
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
            _currentSale = new Sale { SaleItems = new List<SaleItem>() };
            DataContext = _currentSale;
            RefreshDataGrid();
            ProductCodeTextBox.Clear();
            ProductCodeTextBox.Focus();
        }
    }
}