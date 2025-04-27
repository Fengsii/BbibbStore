namespace EFENGSI_RAHMANTO_ZALUKHU.Models
{
    public class GeneralOrderStatus
    {
        public enum GeneralOrderStatusData
        {
            Canceled,
            Unpaid,// Belum Bayar
            Processing,// Dikemas
            Shipped,// Dikirim
            Delivered // Selesai
        }
    }
}
