using EFENGSI_RAHMANTO_ZALUKHU.Models.DB;
using static EFENGSI_RAHMANTO_ZALUKHU.Models.GeneralPaymentStatus;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFENGSI_RAHMANTO_ZALUKHU.Models.DTO
{
    public class PaymentDTO
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string OrderCode { get; set; }
        public string PaymentProof { get; set; }
        public string PaymentMethod { get; set; } // Misalnya: "Credit Card", "PayPal", "Bank Transfer"
        public GeneralPaymentStatusData PaymentStatus { get; set; } // Contoh: "Pending", "Completed", "Failed"

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }
        public DateTime PaymentDate { get; set; }
    }
}
