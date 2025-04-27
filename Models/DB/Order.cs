using static EFENGSI_RAHMANTO_ZALUKHU.Models.GeneralOrderStatus;

namespace EFENGSI_RAHMANTO_ZALUKHU.Models.DB
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public string OrderCode { get; set; }
        public DateTime OrderDate { get; set; }

        // Relasi one-to-one dengan Payment
        public Payment Payment { get; set; }
        public GeneralOrderStatusData Status { get; set; } // Contoh: "Pending", "Shipped", "Delivered"
        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    }
}
