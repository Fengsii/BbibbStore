using EFENGSI_RAHMANTO_ZALUKHU.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFENGSI_RAHMANTO_ZALUKHU.Models.DB
{
    public class OrderDetail
    {
        public int Id { get; set; }
        public int OrderId { get; set; } 
        public Order Order { get; set; }
        public int ProductId { get; set; } 
        public Product Product { get; set; }
        public string Image { get; set; }
        public int Quantity { get; set; }
        public string SelectedSize { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PriceAtPurchase { get; set; } // Harga saat pembelian
    }
}
