using EFENGSI_RAHMANTO_ZALUKHU.Models.DB;


namespace EFENGSI_RAHMANTO_ZALUKHU.Models.DTO
{
    public class PaymentViewDTO
    {
        public int OrderId { get; set; }
        public string OrderCode { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
        public decimal TotalAmount { get; set; }
            
        public decimal saldo { get; set; }
    }
}
