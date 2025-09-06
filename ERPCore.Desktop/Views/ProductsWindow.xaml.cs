using System;
using System.Windows;
using System.Collections.Generic;
using ERPCore.Desktop.Services;
using ERPCore.Models;

namespace ERPCore.Desktop.Views
{
    public partial class ProductsWindow : Window
    {
        private readonly IApiService _apiService;
        private List<Product> _products;

        public ProductsWindow()
        {
            InitializeComponent();
            _apiService = new ApiService();
            Loaded += ProductsWindow_Loaded;
        }

        private async void ProductsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadProductsAsync();
        }

        private async Task LoadProductsAsync()
        {
            try
            {
                txtStatus.Text = "Carregando produtos...";
                _products = await _apiService.GetProductsAsync();
                GridProducts.ItemsSource = _products;
                txtStatus.Text = $"{_products.Count} produtos carregados";
            }
            catch (Exception ex)
            {
                txtStatus.Text = "Erro ao carregar produtos";
                MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnNew_Click(object sender, RoutedEventArgs e)
        {
            var product = new Product
            {
                Code = GenerateProductCode(),
                Active = true,
                CreatedAt = DateTime.Now
            };

            var editWindow = new ProductEditWindow(product);
            if (editWindow.ShowDialog() == true)
            {
                _ = LoadProductsAsync(); // Recarregar lista
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (GridProducts.SelectedItem is Product selectedProduct)
            {
                var editWindow = new ProductEditWindow(selectedProduct);
                if (editWindow.ShowDialog() == true)
                {
                    _ = LoadProductsAsync(); // Recarregar lista
                }
            }
            else
            {
                MessageBox.Show("Selecione um produto para editar.", "Aviso",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (GridProducts.SelectedItem is Product selectedProduct)
            {
                var result = MessageBox.Show($"Deseja excluir o produto '{selectedProduct.Name}'?",
                    "Confirmação", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        var success = await _apiService.DeleteProductAsync(selectedProduct.Id);
                        if (success)
                        {
                            MessageBox.Show("Produto excluído com sucesso.", "Sucesso",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                            await LoadProductsAsync();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erro ao excluir: {ex.Message}", "Erro",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Selecione um produto para excluir.", "Aviso",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private string GenerateProductCode()
        {
            return $"PROD{DateTime.Now:yyyyMMddHHmmss}";
        }
    }
}