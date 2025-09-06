using System;
using System.Windows;
using Microsoft.Win32;
using ERPCore.Desktop.Services;

namespace ERPCore.Desktop.Views
{
    public partial class ReportsWindow : Window
    {
        private readonly IReportService _reportService;

        public ReportsWindow()
        {
            InitializeComponent();
            _reportService = new ReportService(new ApiService());

            // Definir datas padrão
            dpSalesStart.SelectedDate = DateTime.Today.AddDays(-30);
            dpSalesEnd.SelectedDate = DateTime.Today;
            dpDaily.SelectedDate = DateTime.Today;
        }

        private string GetSaveFilePath(string defaultFileName)
        {
            var saveFileDialog = new SaveFileDialog
            {
                FileName = defaultFileName,
                DefaultExt = ".xlsx",
                Filter = "Planilhas Excel (.xlsx)|*.xlsx"
            };

            return saveFileDialog.ShowDialog() == true ? saveFileDialog.FileName : null;
        }

        private async void BtnSalesReport_Click(object sender, RoutedEventArgs e)
        {
            if (dpSalesStart.SelectedDate == null || dpSalesEnd.SelectedDate == null)
            {
                MessageBox.Show("Selecione as datas para o relatório.", "Aviso",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var filePath = GetSaveFilePath($"Relatorio_Vendas_{DateTime.Now:yyyyMMdd}.xlsx");
            if (string.IsNullOrEmpty(filePath)) return;

            var success = await _reportService.GenerateSalesReportAsync(
                dpSalesStart.SelectedDate.Value,
                dpSalesEnd.SelectedDate.Value,
                filePath);

            if (success)
            {
                MessageBox.Show("Relatório de vendas gerado com sucesso!", "Sucesso",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private async void BtnProductsReport_Click(object sender, RoutedEventArgs e)
        {
            var filePath = GetSaveFilePath($"Relatorio_Produtos_{DateTime.Now:yyyyMMdd}.xlsx");
            if (string.IsNullOrEmpty(filePath)) return;

            var success = await _reportService.GenerateProductsReportAsync(filePath);
            if (success)
            {
                MessageBox.Show("Relatório de produtos gerado com sucesso!", "Sucesso",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private async void BtnDailyReport_Click(object sender, RoutedEventArgs e)
        {
            if (dpDaily.SelectedDate == null)
            {
                MessageBox.Show("Selecione a data para o relatório.", "Aviso",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var filePath = GetSaveFilePath($"Relatorio_Diario_{dpDaily.SelectedDate.Value:yyyyMMdd}.xlsx");
            if (string.IsNullOrEmpty(filePath)) return;

            var success = await _reportService.GenerateDailySalesReportAsync(
                dpDaily.SelectedDate.Value,
                filePath);

            if (success)
            {
                MessageBox.Show("Relatório diário gerado com sucesso!", "Sucesso",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private async void BtnInventoryReport_Click(object sender, RoutedEventArgs e)
        {
            var filePath = GetSaveFilePath($"Relatorio_Estoque_{DateTime.Now:yyyyMMdd}.xlsx");
            if (string.IsNullOrEmpty(filePath)) return;

            var success = await _reportService.GenerateInventoryReportAsync(filePath);
            if (success)
            {
                MessageBox.Show("Relatório de estoque gerado com sucesso!", "Sucesso",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}