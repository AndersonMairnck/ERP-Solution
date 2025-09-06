using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ERPCore.Models;

namespace ERPCore.Desktop.Services
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://localhost:7001/api";

        public ApiService()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(BaseUrl);
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/auth/login",
                    new { username, password });
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<Product>> GetProductsAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<Product>>($"{BaseUrl}/products");
            }
            catch
            {
                return new List<Product>();
            }
        }

        public async Task<Product> GetProductByCodeAsync(string code)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<Product>($"{BaseUrl}/products/code/{code}");
            }
            catch
            {
                return null;
            }
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/products", product);
                return await response.Content.ReadFromJsonAsync<Product>();
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> UpdateProductAsync(Product product)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/products/{product.Id}", product);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{BaseUrl}/products/{id}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

       


        public async Task<Sale> CreateSaleAsync(Sale sale)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/sales", sale);
                return await response.Content.ReadFromJsonAsync<Sale>();
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<Sale>> GetSalesByDateAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                var url = $"{BaseUrl}/sales?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}";
                return await _httpClient.GetFromJsonAsync<List<Sale>>(url);
            }
            catch
            {
                return new List<Sale>();
            }
        }

        public async Task<decimal> GetDailyTotalAsync(DateTime date)
        {
            try
            {
                var url = $"{BaseUrl}/sales/daily-total?date={date:yyyy-MM-dd}";
                return await _httpClient.GetFromJsonAsync<decimal>(url);
            }
            catch
            {
                return 0;
            }
        }

        public async Task<List<FinancialAccount>> GetFinancialAccountsAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<FinancialAccount>>($"{BaseUrl}/financial/accounts");
            }
            catch
            {
                return new List<FinancialAccount>();
            }
        }

        public async Task<FinancialAccount> GetFinancialAccountByIdAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<FinancialAccount>($"{BaseUrl}/financial/accounts/{id}");
            }
            catch
            {
                return null;
            }
        }

        public async Task<FinancialAccount> CreateFinancialAccountAsync(FinancialAccount account)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/financial/accounts", account);
                return await response.Content.ReadFromJsonAsync<FinancialAccount>();
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> UpdateFinancialAccountAsync(FinancialAccount account)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/financial/accounts/{account.Id}", account);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteFinancialAccountAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{BaseUrl}/financial/accounts/{id}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}