using ERPCore.Models;

namespace ERPCore.Services
{
    public interface ISaleService
    {
        Task<Sale> CreateSale(Sale sale);
        Task<Sale> GetSaleById(int id);
        Task<List<Sale>> GetSalesByDateRange(DateTime startDate, DateTime endDate);
        Task<List<Sale>> GetSalesByUser(int userId);
        Task<bool> CancelSale(int saleId);
        Task<decimal> GetDailySalesTotal(DateTime date);
    }
}