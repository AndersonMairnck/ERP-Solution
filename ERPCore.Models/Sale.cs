using System.ComponentModel;

namespace ERPCore.Models
{
    public class Sale : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string SaleCode { get; set; }
        public string ClientDocument { get; set; }
        public string ClientName { get; set; }

        private decimal _totalAmount;
        public decimal TotalAmount
        {
            get => _totalAmount;
            set
            {
                _totalAmount = value;
                OnPropertyChanged(nameof(TotalAmount));
                OnPropertyChanged(nameof(Subtotal));
                OnPropertyChanged(nameof(Total));
            }
        }

        private decimal _discount;
        public decimal Discount
        {
            get => _discount;
            set
            {
                _discount = value;
                OnPropertyChanged(nameof(Discount));
                OnPropertyChanged(nameof(Total));
            }
        }

        public decimal FinalAmount { get; set; }
        public string PaymentMethod { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? UserId { get; set; }
        public User User { get; set; }
        public List<SaleItem> SaleItems { get; set; } = new List<SaleItem>();

        // Propriedades calculadas para binding
        public decimal Subtotal => TotalAmount;
        public decimal Total => TotalAmount - Discount;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}