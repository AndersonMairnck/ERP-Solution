using ERPCore.Data;
using ERPCore.Models;
using Microsoft.EntityFrameworkCore;



namespace ERPCore.Services
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;

        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetAllProducts()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<Product> GetProductById(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task<Product> GetProductByCode(string code)
        {
            return await _context.Products.FirstOrDefaultAsync(p => p.Code == code);
        }

        public async Task<Product> CreateProduct(Product product)
        {
            product.CreatedAt = DateTime.Now;
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task UpdateProduct(Product product)
        {
            product.UpdatedAt = DateTime.Now;
            _context.Entry(product).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ProductExists(int id)
        {
            return await _context.Products.AnyAsync(e => e.Id == id);
        }

        public async Task<bool> ProductCodeExists(string code)
        {
            return await _context.Products.AnyAsync(e => e.Code == code);
        }
    }
}