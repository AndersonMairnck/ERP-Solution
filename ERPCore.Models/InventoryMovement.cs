using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPCore.Models
{
    public class InventoryMovement
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string MovementType { get; set; }
        public int Quantity { get; set; }
        public string Reason { get; set; }
        public int? ReferenceId { get; set; }
        public DateTime CreatedAt { get; set; }
        public Product Product { get; set; }
    }
}
