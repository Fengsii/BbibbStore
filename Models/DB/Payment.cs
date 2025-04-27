using System.ComponentModel.DataAnnotations.Schema;
using static EFENGSI_RAHMANTO_ZALUKHU.Models.GeneralPaymentStatus;

namespace EFENGSI_RAHMANTO_ZALUKHU.Models.DB
{
    public class Payment
    {
        public int Id { get; set; }
        public int OrderId { get; set; } 
        public Order Order { get; set; }
        public string PaymentMethod { get; set; } // Misalnya: "Credit Card", "PayPal", "Bank Transfer"
        public GeneralPaymentStatusData PaymentStatus { get; set; } // Contoh: "Pending", "Completed", "Failed"

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; } 
        public DateTime PaymentDate { get; set; }
        public string? BankName { get; internal set; }
        public string? ProofImage { get; internal set; }
    }
}
