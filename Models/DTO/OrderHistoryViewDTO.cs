using EFENGSI_RAHMANTO_ZALUKHU.Models.DB;

namespace EFENGSI_RAHMANTO_ZALUKHU.Models.DTO
{
    public class OrderHistoryViewDTO
    {
        public List<Order> AllOrders { get; set; }
        public List<Order> CanceledOrders { get; set; }
        public List<Order> UnpaidOrders { get; set; }
        public List<Order> ProcessingOrders { get; set; }
        public List<Order> ShippedOrders { get; set; }
        public List<Order> DeliveredOrders { get; set; }
    }
}
