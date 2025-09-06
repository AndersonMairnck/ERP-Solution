using ERPCore.Data;
using ERPCore.Models;
using Microsoft.EntityFrameworkCore;

namespace ERPCore.Services
{
    public class SaleService : ISaleService
    {
        private readonly ApplicationDbContext _context;

        public SaleService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Sale> CreateSale(Sale sale)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Gerar código da venda
                sale.SaleCode = GenerateSaleCode();
                sale.CreatedAt = DateTime.Now;

                // Calcular totais
                sale.TotalAmount = sale.SaleItems.Sum(item => item.TotalPrice);
                sale.FinalAmount = sale.TotalAmount - sale.Discount;

                _context.Sales.Add(sale);

                // Atualizar estoque e registrar movimentações
                foreach (var item in sale.SaleItems)
                {
                    var product = await _context.Products.FindAsync(item.ProductId);
                    if (product != null)
                    {
                        product.StockQuantity -= item.Quantity;

                        // Registrar movimentação de estoque
                        var movement = new InventoryMovement
                        {
                            ProductId = item.ProductId,
                            MovementType = "OUT",
                            Quantity = item.Quantity,
                            Reason = "SALE",
                            ReferenceId = sale.Id,
                            CreatedAt = DateTime.Now
                        };
                        _context.InventoryMovements.Add(movement);
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return sale;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Sale> GetSaleById(int id)
        {
            return await _context.Sales
                .Include(s => s.SaleItems)
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<List<Sale>> GetSalesByDateRange(DateTime startDate, DateTime endDate)
        {
            return await _context.Sales
                .Include(s => s.SaleItems)
                .Include(s => s.User)
                .Where(s => s.CreatedAt >= startDate && s.CreatedAt <= endDate)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Sale>> GetSalesByUser(int userId)
        {
            return await _context.Sales
                .Include(s => s.SaleItems)
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> CancelSale(int saleId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var sale = await _context.Sales
                    .Include(s => s.SaleItems)
                    .FirstOrDefaultAsync(s => s.Id == saleId);

                if (sale == null) return false;

                // Restaurar estoque
                foreach (var item in sale.SaleItems)
                {
                    var product = await _context.Products.FindAsync(item.ProductId);
                    if (product != null)
                    {
                        product.StockQuantity += item.Quantity;

                        // Registrar movimentação de estorno
                        var movement = new InventoryMovement
                        {
                            ProductId = item.ProductId,
                            MovementType = "IN",
                            Quantity = item.Quantity,
                            Reason = "SALE_CANCELLATION",
                            ReferenceId = sale.Id,
                            CreatedAt = DateTime.Now
                        };
                        _context.InventoryMovements.Add(movement);
                    }
                }

                sale.Status = "CANCELLED";
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<decimal> GetDailySalesTotal(DateTime date)
        {
            var startDate = date.Date;
            var endDate = date.Date.AddDays(1).AddTicks(-1);

            return await _context.Sales
                .Where(s => s.CreatedAt >= startDate && s.CreatedAt <= endDate && s.Status == "COMPLETED")
                .SumAsync(s => s.FinalAmount);
        }

        private string GenerateSaleCode()
        {
            var datePart = DateTime.Now.ToString("yyyyMMdd");
            var count = _context.Sales.Count(s => s.CreatedAt.Date == DateTime.Today) + 1;
            return $"V{datePart}{count:D4}";
        }
    }
}