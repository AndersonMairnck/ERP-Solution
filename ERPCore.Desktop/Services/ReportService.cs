using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Windows;
using ClosedXML.Excel;
using ERPCore.Models;
using ERPCore.Desktop.Services;

namespace ERPCore.Desktop.Services
{
    public class ReportService : IReportService
    {
        private readonly IApiService _apiService;

        public ReportService(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<bool> GenerateSalesReportAsync(DateTime startDate, DateTime endDate, string filePath)
        {
            try
            {
                var sales = await _apiService.GetSalesByDateAsync(startDate, endDate);

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Vendas");

                    // Cabeçalho
                    worksheet.Cell(1, 1).Value = "Relatório de Vendas";
                    worksheet.Cell(1, 1).Style.Font.Bold = true;
                    worksheet.Cell(1, 1).Style.Font.FontSize = 16;
                    worksheet.Range(1, 1, 1, 7).Merge();

                    worksheet.Cell(2, 1).Value = $"Período: {startDate:dd/MM/yyyy} - {endDate:dd/MM/yyyy}";
                    worksheet.Cell(2, 1).Style.Font.Italic = true;
                    worksheet.Range(2, 1, 2, 7).Merge();

                    // Cabeçalhos das colunas
                    var headers = new[] { "Data", "Código", "Cliente", "Total", "Desconto", "Final", "Pagamento" };
                    for (int i = 0; i < headers.Length; i++)
                    {
                        worksheet.Cell(4, i + 1).Value = headers[i];
                        worksheet.Cell(4, i + 1).Style.Font.Bold = true;
                        worksheet.Cell(4, i + 1).Style.Fill.BackgroundColor = XLColor.Gray;
                    }

                    // Dados
                    int row = 5;
                    foreach (var sale in sales.OrderBy(s => s.CreatedAt))
                    {
                        worksheet.Cell(row, 1).Value = sale.CreatedAt;
                        worksheet.Cell(row, 1).Style.DateFormat.Format = "dd/MM/yyyy HH:mm";

                        worksheet.Cell(row, 2).Value = sale.SaleCode;
                        worksheet.Cell(row, 3).Value = sale.ClientName;
                        worksheet.Cell(row, 4).Value = sale.TotalAmount;
                        worksheet.Cell(row, 4).Style.NumberFormat.Format = "R$ #,##0.00";

                        worksheet.Cell(row, 5).Value = sale.Discount;
                        worksheet.Cell(row, 5).Style.NumberFormat.Format = "R$ #,##0.00";

                        worksheet.Cell(row, 6).Value = sale.FinalAmount;
                        worksheet.Cell(row, 6).Style.NumberFormat.Format = "R$ #,##0.00";

                        worksheet.Cell(row, 7).Value = sale.PaymentMethod;
                        row++;
                    }

                    // Totais
                    worksheet.Cell(row, 3).Value = "TOTAL:";
                    worksheet.Cell(row, 3).Style.Font.Bold = true;
                    worksheet.Cell(row, 6).Value = sales.Sum(s => s.FinalAmount);
                    worksheet.Cell(row, 6).Style.Font.Bold = true;
                    worksheet.Cell(row, 6).Style.NumberFormat.Format = "R$ #,##0.00";

                    // Ajustar colunas
                    worksheet.Columns().AdjustToContents();

                    // Salvar
                    workbook.SaveAs(filePath);
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao gerar relatório: {ex.Message}", "Erro",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public async Task<bool> GenerateProductsReportAsync(string filePath)
        {
            try
            {
                var products = await _apiService.GetProductsAsync();

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Produtos");

                    // Cabeçalho
                    worksheet.Cell(1, 1).Value = "Relatório de Produtos";
                    worksheet.Cell(1, 1).Style.Font.Bold = true;
                    worksheet.Cell(1, 1).Style.Font.FontSize = 16;
                    worksheet.Range(1, 1, 1, 8).Merge();

                    worksheet.Cell(2, 1).Value = $"Data: {DateTime.Now:dd/MM/yyyy HH:mm}";
                    worksheet.Cell(2, 1).Style.Font.Italic = true;
                    worksheet.Range(2, 1, 2, 8).Merge();

                    // Cabeçalhos das colunas
                    var headers = new[] { "Código", "Nome", "Descrição", "Preço Varejo", "Preço Atacado", "Estoque", "Mínimo", "Status" };
                    for (int i = 0; i < headers.Length; i++)
                    {
                        worksheet.Cell(4, i + 1).Value = headers[i];
                        worksheet.Cell(4, i + 1).Style.Font.Bold = true;
                        worksheet.Cell(4, i + 1).Style.Fill.BackgroundColor = XLColor.Gray;
                    }

                    // Dados
                    int row = 5;
                    foreach (var product in products.OrderBy(p => p.Code))
                    {
                        worksheet.Cell(row, 1).Value = product.Code;
                        worksheet.Cell(row, 2).Value = product.Name;
                        worksheet.Cell(row, 3).Value = product.Description;

                        worksheet.Cell(row, 4).Value = product.PriceRetail;
                        worksheet.Cell(row, 4).Style.NumberFormat.Format = "R$ #,##0.00";

                        worksheet.Cell(row, 5).Value = product.PriceWholesale;
                        worksheet.Cell(row, 5).Style.NumberFormat.Format = "R$ #,##0.00";

                        worksheet.Cell(row, 6).Value = product.StockQuantity;

                        worksheet.Cell(row, 7).Value = product.MinimumStock;

                        worksheet.Cell(row, 8).Value = product.Active ? "Ativo" : "Inativo";
                        worksheet.Cell(row, 8).Style.Font.FontColor =
                            product.Active ? XLColor.Green : XLColor.Red;

                        // Destacar produtos com estoque baixo - CORREÇÃO AQUI
                        if (product.StockQuantity <= product.MinimumStock)
                        {
                            worksheet.Range(row, 1, row, 8).Style.Fill.BackgroundColor = XLColor.LightYellow;
                        }

                        row++;
                    }

                    // Estatísticas
                    worksheet.Cell(row + 2, 1).Value = "ESTATÍSTICAS:";
                    worksheet.Cell(row + 2, 1).Style.Font.Bold = true;

                    worksheet.Cell(row + 3, 1).Value = "Total de Produtos:";
                    worksheet.Cell(row + 3, 2).Value = products.Count;

                    worksheet.Cell(row + 4, 1).Value = "Produtos Ativos:";
                    worksheet.Cell(row + 4, 2).Value = products.Count(p => p.Active);

                    worksheet.Cell(row + 5, 1).Value = "Estoque Baixo:";
                    worksheet.Cell(row + 5, 2).Value = products.Count(p => p.StockQuantity <= p.MinimumStock);

                    // Ajustar colunas
                    worksheet.Columns().AdjustToContents();

                    // Salvar
                    workbook.SaveAs(filePath);
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao gerar relatório: {ex.Message}", "Erro",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public async Task<bool> GenerateDailySalesReportAsync(DateTime date, string filePath)
        {
            try
            {
                var sales = await _apiService.GetSalesByDateAsync(date.Date, date.Date.AddDays(1).AddTicks(-1));
                var dailyTotal = await _apiService.GetDailyTotalAsync(date);

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Vendas Diárias");

                    // Cabeçalho
                    worksheet.Cell(1, 1).Value = "Relatório de Vendas Diárias";
                    worksheet.Cell(1, 1).Style.Font.Bold = true;
                    worksheet.Cell(1, 1).Style.Font.FontSize = 16;
                    worksheet.Range(1, 1, 1, 5).Merge();

                    worksheet.Cell(2, 1).Value = $"Data: {date:dd/MM/yyyy}";
                    worksheet.Cell(2, 1).Style.Font.Italic = true;
                    worksheet.Range(2, 1, 2, 5).Merge();

                    // Resumo
                    worksheet.Cell(4, 1).Value = "Total de Vendas:";
                    worksheet.Cell(4, 2).Value = sales.Count;
                    worksheet.Cell(4, 2).Style.Font.Bold = true;

                    worksheet.Cell(5, 1).Value = "Valor Total:";
                    worksheet.Cell(5, 2).Value = dailyTotal;
                    worksheet.Cell(5, 2).Style.NumberFormat.Format = "R$ #,##0.00";
                    worksheet.Cell(5, 2).Style.Font.Bold = true;

                    // Detalhes das vendas
                    if (sales.Any())
                    {
                        worksheet.Cell(7, 1).Value = "Detalhes das Vendas:";
                        worksheet.Cell(7, 1).Style.Font.Bold = true;
                        worksheet.Range(7, 1, 7, 4).Merge();

                        var headers = new[] { "Hora", "Código", "Valor", "Pagamento" };
                        for (int i = 0; i < headers.Length; i++)
                        {
                            worksheet.Cell(9, i + 1).Value = headers[i];
                            worksheet.Cell(9, i + 1).Style.Font.Bold = true;
                            worksheet.Cell(9, i + 1).Style.Fill.BackgroundColor = XLColor.Gray;
                        }

                        int row = 10;
                        foreach (var sale in sales.OrderBy(s => s.CreatedAt))
                        {
                            worksheet.Cell(row, 1).Value = sale.CreatedAt;
                            worksheet.Cell(row, 1).Style.DateFormat.Format = "HH:mm";

                            worksheet.Cell(row, 2).Value = sale.SaleCode;

                            worksheet.Cell(row, 3).Value = sale.FinalAmount;
                            worksheet.Cell(row, 3).Style.NumberFormat.Format = "R$ #,##0.00";

                            worksheet.Cell(row, 4).Value = sale.PaymentMethod;
                            row++;
                        }
                    }

                    // Ajustar colunas
                    worksheet.Columns().AdjustToContents();

                    // Salvar
                    workbook.SaveAs(filePath);
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao gerar relatório: {ex.Message}", "Erro",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public async Task<bool> GenerateInventoryReportAsync(string filePath)
        {
            try
            {
                var products = await _apiService.GetProductsAsync();

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Estoque");

                    // Cabeçalho
                    worksheet.Cell(1, 1).Value = "Relatório de Estoque";
                    worksheet.Cell(1, 1).Style.Font.Bold = true;
                    worksheet.Cell(1, 1).Style.Font.FontSize = 16;
                    worksheet.Range(1, 1, 1, 6).Merge();

                    // Dados
                    var headers = new[] { "Código", "Produto", "Estoque Atual", "Mínimo", "Status", "Valor Total" };
                    for (int i = 0; i < headers.Length; i++)
                    {
                        worksheet.Cell(3, i + 1).Value = headers[i];
                        worksheet.Cell(3, i + 1).Style.Font.Bold = true;
                        worksheet.Cell(3, i + 1).Style.Fill.BackgroundColor = XLColor.Gray;
                    }

                    int row = 4;
                    foreach (var product in products.OrderBy(p => p.StockQuantity).ThenBy(p => p.Code))
                    {
                        worksheet.Cell(row, 1).Value = product.Code;
                        worksheet.Cell(row, 2).Value = product.Name;
                        worksheet.Cell(row, 3).Value = product.StockQuantity;
                        worksheet.Cell(row, 4).Value = product.MinimumStock;

                        var status = product.StockQuantity switch
                        {
                            0 => "ESGOTADO",
                            _ when product.StockQuantity <= product.MinimumStock => "BAIXO",
                            _ => "OK"
                        };

                        worksheet.Cell(row, 5).Value = status;
                        worksheet.Cell(row, 5).Style.Font.FontColor = status switch
                        {
                            "ESGOTADO" => XLColor.Red,
                            "BAIXO" => XLColor.Orange,
                            _ => XLColor.Green
                        };

                        worksheet.Cell(row, 6).Value = product.StockQuantity * product.PriceRetail;
                        worksheet.Cell(row, 6).Style.NumberFormat.Format = "R$ #,##0.00";

                        // Destacar estoque baixo - CORREÇÃO AQUI
                        if (product.StockQuantity <= product.MinimumStock)
                        {
                            worksheet.Range(row, 1, row, 6).Style.Fill.BackgroundColor = XLColor.LightYellow;
                        }

                        row++;
                    }

                    // Totais
                    worksheet.Cell(row + 1, 2).Value = "TOTAL EM ESTOQUE:";
                    worksheet.Cell(row + 1, 2).Style.Font.Bold = true;
                    worksheet.Cell(row + 1, 6).FormulaA1 = $"SUM(F4:F{row})";
                    worksheet.Cell(row + 1, 6).Style.Font.Bold = true;
                    worksheet.Cell(row + 1, 6).Style.NumberFormat.Format = "R$ #,##0.00";

                    // Ajustar colunas
                    worksheet.Columns().AdjustToContents();

                    // Salvar
                    workbook.SaveAs(filePath);
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao gerar relatório: {ex.Message}", "Erro",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
    }
}