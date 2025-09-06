using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using ERPCore.Models;

namespace ERPCore.Desktop.Services
{
    public interface IReportService
    {
        Task<bool> GenerateSalesReportAsync(DateTime startDate, DateTime endDate, string filePath);
        Task<bool> GenerateProductsReportAsync(string filePath);
        Task<bool> GenerateDailySalesReportAsync(DateTime date, string filePath);
        Task<bool> GenerateInventoryReportAsync(string filePath);
    }
}