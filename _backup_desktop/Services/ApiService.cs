using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ERPCore.Models;

namespace ERPCore.Desktop.Services
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private string _authToken;
        private const string BaseUrl = "https://localhost:7001/api";

        public ApiService()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(BaseUrl);
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            try
            {
                var loginData = new { username, password };
                var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/auth/login", loginData);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<JsonElement>();
                    if (result.TryGetProperty("token", out var tokenProperty))
                    {
                        _authToken = tokenProperty.GetString();
                        _httpClient.DefaultRequestHeaders.Authorization =
                            new AuthenticationHeaderValue("Bearer", _authToken);
                        return true;
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<string> GetTokenAsync() => _authToken;

        public async Task<bool> IsAuthenticatedAsync() => !string.IsNullOrEmpty(_authToken);

        public void Logout()
        {
            _authToken = null;
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }

        // Implementações dos métodos de produtos
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

        // Implementações dos métodos de vendas
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

        public async Task<List<Sale>> GetSalesAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<Sale>>(
                    $"{BaseUrl}/sales/date-range?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}");
            }
            catch
            {
                return new List<Sale>();
            }
        }

        public async Task<Sale> GetSaleByIdAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<Sale>($"{BaseUrl}/sales/{id}");
            }
            catch
            {
                return null;
            }
        }

        public async Task<decimal> GetDailySalesTotalAsync(DateTime date)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<decimal>(
                    $"{BaseUrl}/sales/daily-total?date={date:yyyy-MM-dd}");
            }
            catch
            {
                return 0;
            }
        }
    }
}