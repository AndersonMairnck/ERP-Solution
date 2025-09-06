using System.Collections.Generic;
using System.Threading.Tasks;
using ERPCore.Models;

namespace ERPCore.Desktop.Services
{
    public interface IApiService
    {
        Task<bool> LoginAsync(string username, string password);
        Task<List<Product>> GetProductsAsync();
        Task<Product> GetProductByCodeAsync(string code);
        Task<Product> CreateProductAsync(Product product);
        Task<bool> UpdateProductAsync(Product product);
        Task<bool> DeleteProductAsync(int id);
        Task<Sale> CreateSaleAsync(Sale sale);
     
        Task<List<Sale>> GetSalesByDateAsync(DateTime startDate, DateTime endDate);
        Task<decimal> GetDailyTotalAsync(DateTime date);



    }
}