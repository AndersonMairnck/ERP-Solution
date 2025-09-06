using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ERPCore.Data;
using ERPCore.Models;

namespace ERPCore.Services
{
    public class FinancialService : IFinancialService
    {
        private readonly ApplicationDbContext _context;

        public FinancialService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Implementação dos métodos da interface
        public async Task<FinancialAccount> CreateAccountAsync(FinancialAccount account)
        {
            account.CreatedAt = DateTime.Now;
            account.CurrentBalance = account.InitialBalance;

            _context.FinancialAccounts.Add(account);
            await _context.SaveChangesAsync();

            return account;
        }

        public async Task<List<FinancialAccount>> GetAccountsAsync()
        {
            return await _context.FinancialAccounts
                .Where(a => a.IsActive)
                .OrderBy(a => a.Name)
                .ToListAsync();
        }

        public async Task<FinancialAccount> GetAccountByIdAsync(int id)
        {
            return await _context.FinancialAccounts.FindAsync(id);
        }

        public async Task<bool> UpdateAccountAsync(FinancialAccount account)
        {
            account.UpdatedAt = DateTime.Now;
            _context.Entry(account).State = EntityState.Modified;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAccountAsync(int id)
        {
            var account = await _context.FinancialAccounts.FindAsync(id);
            if (account == null) return false;

            account.IsActive = false;
            account.UpdatedAt = DateTime.Now;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<FinancialTransaction> CreateTransactionAsync(FinancialTransaction transaction)
        {
            transaction.CreatedAt = DateTime.Now;
            transaction.TransactionCode = GenerateTransactionCode();

            _context.FinancialTransactions.Add(transaction);

            // Atualizar saldo da conta se a transação estiver paga
            if (transaction.Status == "Pago" && transaction.FinancialAccountId.HasValue)
            {
                await UpdateAccountBalance(transaction.FinancialAccountId.Value, transaction.Amount, transaction.TransactionType);
            }

            await _context.SaveChangesAsync();
            return transaction;
        }

        public async Task<List<FinancialTransaction>> GetTransactionsAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.FinancialTransactions
                .Include(t => t.FinancialAccount)
                .Include(t => t.Sale)
                .Where(t => t.TransactionDate >= startDate && t.TransactionDate <= endDate)
                .OrderByDescending(t => t.TransactionDate)
                .ThenByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> ProcessPaymentAsync(int transactionId, DateTime paymentDate, int accountId)
        {
            var transaction = await _context.FinancialTransactions.FindAsync(transactionId);
            if (transaction == null) return false;

            transaction.PaymentDate = paymentDate;
            transaction.Status = "Pago";
            transaction.FinancialAccountId = accountId;
            transaction.UpdatedAt = DateTime.Now;

            // Atualizar saldo da conta
            await UpdateAccountBalance(accountId, transaction.Amount, transaction.TransactionType);

            return await _context.SaveChangesAsync() > 0;
        }

        private async Task UpdateAccountBalance(int accountId, decimal amount, string transactionType)
        {
            var account = await _context.FinancialAccounts.FindAsync(accountId);
            if (account == null) return;

            if (transactionType == "Receita")
            {
                account.CurrentBalance += amount;
            }
            else if (transactionType == "Despesa")
            {
                account.CurrentBalance -= amount;
            }

            account.UpdatedAt = DateTime.Now;
        }

        private string GenerateTransactionCode()
        {
            var count = _context.FinancialTransactions.Count(t => t.CreatedAt.Date == DateTime.Today) + 1;
            return $"FIN{DateTime.Now:yyyyMMdd}{count:D4}";
        }

        // Implementar outros métodos da interface...
        public async Task<FinancialSummary> GetFinancialSummaryAsync(DateTime startDate, DateTime endDate)
        {
            var transactions = await _context.FinancialTransactions
                .Where(t => t.TransactionDate >= startDate && t.TransactionDate <= endDate)
                .ToListAsync();

            return new FinancialSummary
            {
                TotalRevenue = transactions.Where(t => t.TransactionType == "Receita" && t.Status == "Pago").Sum(t => t.Amount),
                TotalExpenses = transactions.Where(t => t.TransactionType == "Despesa" && t.Status == "Pago").Sum(t => t.Amount),
                TotalTransactions = transactions.Count,
                PendingPayments = transactions.Count(t => t.Status == "Pendente"),
                OverduePayments = transactions.Count(t => t.Status == "Pendente" && t.DueDate < DateTime.Today)
            };
        }

        // Implementar métodos restantes da interface...
        public Task<List<FinancialTransaction>> GetTransactionsByAccountAsync(int accountId, DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }

        public Task<FinancialTransaction> GetTransactionByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateTransactionAsync(FinancialTransaction transaction)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteTransactionAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Supplier> CreateSupplierAsync(Supplier supplier)
        {
            throw new NotImplementedException();
        }

        public Task<List<Supplier>> GetSuppliersAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Supplier> GetSupplierByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateSupplierAsync(Supplier supplier)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteSupplierAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<CashFlowReport> GetCashFlowReportAsync(DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }

        public Task<List<AccountBalance>> GetAccountBalancesAsync()
        {
            throw new NotImplementedException();
        }
    }
}