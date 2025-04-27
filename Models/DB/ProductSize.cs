using static EFENGSI_RAHMANTO_ZALUKHU.Models.GeneralStatus;

namespace EFENGSI_RAHMANTO_ZALUKHU.Models.DB
{
    public class ProductSize
    {
        public int Id { get; set; }
        public string Size { get; set; }
        public int Stock { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }

    }
}
