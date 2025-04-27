using EFENGSI_RAHMANTO_ZALUKHU.Models.DB;
using EFENGSI_RAHMANTO_ZALUKHU.Models.DTO;

namespace EFENGSI_RAHMANTO_ZALUKHU.Interfaces
{
    public interface IPayment
    {
        public bool AddPayment(PaymentDTO paymentDTO);
        public bool EditPayment(PaymentDTO paymentDTO);
        public Payment GetPaymentlById(int id);
        public List<PaymentDTO> GetListPayment();
        public bool DeletePayment(int id);

        Task<bool> ConfirmPayment(int orderId, string paymentMethod, string selectedBank, string proofImage);

    }
}
