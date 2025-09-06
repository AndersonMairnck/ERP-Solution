using System;
using System.ComponentModel.DataAnnotations;

namespace ERPCore.Models
{
    public class FinancialTransaction
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string TransactionCode { get; set; }

        [Required]
        [StringLength(200)]
        public string Description { get; set; }

        public decimal Amount { get; set; }

        [Required]
        [StringLength(10)]
        public string TransactionType { get; set; } // Receita, Despesa, Transferência

        [StringLength(20)]
        public string Category { get; set; } // Vendas, Compras, Salários, etc.

        public DateTime TransactionDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? PaymentDate { get; set; }

        [StringLength(20)]
        public string Status { get; set; } // Pendente, Pago, Cancelado

        public int? FinancialAccountId { get; set; }
        public FinancialAccount FinancialAccount { get; set; }

        public int? SaleId { get; set; }
        public Sale Sale { get; set; }

        public int? SupplierId { get; set; }
        public int? CustomerId { get; set; }

        [StringLength(500)]
        public string Notes { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}