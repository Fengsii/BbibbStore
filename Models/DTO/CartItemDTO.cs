using EFENGSI_RAHMANTO_ZALUKHU.Models.DB;

namespace EFENGSI_RAHMANTO_ZALUKHU.Models.DTO
{
    public class CartItemDTO
    {
        public int Id { get; set; }
        public int CartId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string SelectedSize { get; set; }
        public string Image { get; set; }
    }
}
