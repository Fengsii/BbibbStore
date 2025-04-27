using static EFENGSI_RAHMANTO_ZALUKHU.Models.GeneralOrderStatus;

namespace EFENGSI_RAHMANTO_ZALUKHU.Models.DTO
{
    public class OrderDetailDTO
    {
        public int Id { get; set; }
        public int OrderId { get; set; } // Foreign key ke Order
        public string OrderCode { get; set; }
        public int ProductId { get; set; } // Foreign key ke Product
        public string ProductName { get; set; }
        public string Image { get; set; }
        public int Quantity { get; set; }
        public string SelectedSize { get; set; }
        public decimal PriceAtPurchase { get; set; } // Harga saat pembelian

    }
}
