using System.Threading.Tasks;
using ERPCore.Models;

namespace ERPCore.Desktop.Services
{
    public interface IApiService
    {
        Task<bool> LoginAsync(string username, string password);
        Task<string> GetTokenAsync();
        Task<bool> IsAuthenticatedAsync();
        void Logout();

        // Produtos
        Task<List<Product>> GetProductsAsync();
        Task<Product> GetProductByCodeAsync(string code);
        Task<Product> CreateProductAsync(Product product);
        Task<bool> UpdateProductAsync(Product product);
        Task<bool> DeleteProductAsync(int id);

        // Vendas
        Task<Sale> CreateSaleAsync(Sale sale);
        Task<List<Sale>> GetSalesAsync(DateTime startDate, DateTime endDate);
        Task<Sale> GetSaleByIdAsync(int id);
        Task<decimal> GetDailySalesTotalAsync(DateTime date);
    }
}