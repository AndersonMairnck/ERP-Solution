using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using ERPCore.Services;
using ERPCore.Models;

namespace ERPCore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FinancialController : ControllerBase
    {
        private readonly IFinancialService _financialService;

        public FinancialController(IFinancialService financialService)
        {
            _financialService = financialService;
        }

        [HttpGet("accounts")]
        public async Task<ActionResult<List<FinancialAccount>>> GetAccounts()
        {
            var accounts = await _financialService.GetAccountsAsync();
            return Ok(accounts);
        }

        [HttpGet("accounts/{id}")]
        public async Task<ActionResult<FinancialAccount>> GetAccount(int id)
        {
            var account = await _financialService.GetAccountByIdAsync(id);
            if (account == null) return NotFound();
            return Ok(account);
        }

        [HttpPost("accounts")]
        public async Task<ActionResult<FinancialAccount>> CreateAccount(FinancialAccount account)
        {
            var createdAccount = await _financialService.CreateAccountAsync(account);
            return CreatedAtAction(nameof(GetAccount), new { id = createdAccount.Id }, createdAccount);
        }

        [HttpGet("transactions")]
        public async Task<ActionResult<List<FinancialTransaction>>> GetTransactions(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var transactions = await _financialService.GetTransactionsAsync(startDate, endDate);
            return Ok(transactions);
        }

        [HttpPost("transactions")]
        public async Task<ActionResult<FinancialTransaction>> CreateTransaction(FinancialTransaction transaction)
        {
            var createdTransaction = await _financialService.CreateTransactionAsync(transaction);
            return CreatedAtAction(nameof(GetTransactions), new { id = createdTransaction.Id }, createdTransaction);
        }

        [HttpPost("transactions/{id}/process-payment")]
        public async Task<IActionResult> ProcessPayment(int id, [FromBody] ProcessPaymentRequest request)
        {
            var success = await _financialService.ProcessPaymentAsync(id, request.PaymentDate, request.AccountId);
            if (!success) return NotFound();
            return Ok();
        }

        [HttpGet("summary")]
        public async Task<ActionResult<FinancialSummary>> GetFinancialSummary(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var summary = await _financialService.GetFinancialSummaryAsync(startDate, endDate);
            return Ok(summary);
        }

        [HttpGet("suppliers")]
        public async Task<ActionResult<List<Supplier>>> GetSuppliers()
        {
            var suppliers = await _financialService.GetSuppliersAsync();
            return Ok(suppliers);
        }
    }

    public class ProcessPaymentRequest
    {
        public DateTime PaymentDate { get; set; }
        public int AccountId { get; set; }
    }
}