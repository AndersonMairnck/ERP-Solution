using System;
using System.ComponentModel.DataAnnotations;

namespace ERPCore.Models
{
    public class FinancialAccount
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(20)]
        public string AccountNumber { get; set; }

        [StringLength(50)]
        public string BankName { get; set; }

        [StringLength(20)]
        public string AccountType { get; set; } // Corrente, Poupança, Caixa, etc.

        public decimal InitialBalance { get; set; }
        public decimal CurrentBalance { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}