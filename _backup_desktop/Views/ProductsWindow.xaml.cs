using ERPCore.Desktop.Views;
using ERPCore.Models;
using System.Windows;

private void AddButton_Click(object sender, RoutedEventArgs e)
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
        // Salvar o produto através do serviço
        _ = SaveProductAsync(editWindow.CurrentProduct);
    }
}

private void EditButton_Click(object sender, RoutedEventArgs e)
{
    if (ProductsDataGrid.SelectedItem is Product selectedProduct)
    {
        var editWindow = new ProductEditWindow(selectedProduct);
        if (editWindow.ShowDialog() == true)
        {
            // Atualizar o produto através do serviço
            _ = UpdateProductAsync(editWindow.CurrentProduct);
        }
    }
    else
    {
        MessageBox.Show("Selecione um produto para editar.", "Aviso",
            MessageBoxButton.OK, MessageBoxImage.Warning);
    }
}

private async Task SaveProductAsync(Product product)
{
    try
    {
        var savedProduct = await _apiService.CreateProductAsync(product);
        if (savedProduct != null)
        {
            await LoadProductsAsync();
        }
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Erro ao salvar produto: {ex.Message}", "Erro",
            MessageBoxButton.OK, MessageBoxImage.Error);
    }
}

private async Task UpdateProductAsync(Product product)
{
    try
    {
        var success = await _apiService.UpdateProductAsync(product);
        if (success)
        {
            await LoadProductsAsync();
        }
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Erro ao atualizar produto: {ex.Message}", "Erro",
            MessageBoxButton.OK, MessageBoxImage.Error);
    }
}