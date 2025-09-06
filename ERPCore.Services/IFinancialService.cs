using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERPCore.Models;

namespace ERPCore.Services
{
    public interface IFinancialService
    {
        // Contas Financeiras
        Task<FinancialAccount> CreateAccountAsync(FinancialAccount account);
        Task<List<FinancialAccount>> GetAccountsAsync();
        Task<FinancialAccount> GetAccountByIdAsync(int id);
        Task<bool> UpdateAccountAsync(FinancialAccount account);
        Task<bool> DeleteAccountAsync(int id);

        // Transações Financeiras
        Task<FinancialTransaction> CreateTransactionAsync(FinancialTransaction transaction);
        Task<List<FinancialTransaction>> GetTransactionsAsync(DateTime startDate, DateTime endDate);
        Task<List<FinancialTransaction>> GetTransactionsByAccountAsync(int accountId, DateTime startDate, DateTime endDate);
        Task<FinancialTransaction> GetTransactionByIdAsync(int id);
        Task<bool> UpdateTransactionAsync(FinancialTransaction transaction);
        Task<bool> DeleteTransactionAsync(int id);
        Task<bool> ProcessPaymentAsync(int transactionId, DateTime paymentDate, int accountId);

        // Fornecedores
        Task<Supplier> CreateSupplierAsync(Supplier supplier);
        Task<List<Supplier>> GetSuppliersAsync();
        Task<Supplier> GetSupplierByIdAsync(int id);
        Task<bool> UpdateSupplierAsync(Supplier supplier);
        Task<bool> DeleteSupplierAsync(int id);

        // Relatórios
        Task<FinancialSummary> GetFinancialSummaryAsync(DateTime startDate, DateTime endDate);
        Task<CashFlowReport> GetCashFlowReportAsync(DateTime startDate, DateTime endDate);
        Task<List<AccountBalance>> GetAccountBalancesAsync();
    }

    public class FinancialSummary
    {
        public decimal TotalRevenue { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal NetProfit { get; set; }
        public int TotalTransactions { get; set; }
        public int PendingPayments { get; set; }
        public int OverduePayments { get; set; }
    }

    public class CashFlowReport
    {
        public List<CashFlowItem> Items { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal ClosingBalance { get; set; }
        public decimal NetCashFlow { get; set; }
    }

    public class CashFlowItem
    {
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public decimal Revenue { get; set; }
        public decimal Expenses { get; set; }
        public decimal DailyBalance { get; set; }
        public decimal CumulativeBalance { get; set; }
    }

    public class AccountBalance
    {
        public string AccountName { get; set; }
        public decimal Balance { get; set; }
        public decimal InitialBalance { get; set; }
    }
}