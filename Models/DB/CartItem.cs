using System.ComponentModel.DataAnnotations.Schema;

namespace EFENGSI_RAHMANTO_ZALUKHU.Models.DB
{
    public class CartItem
    {
        public int Id { get; set; }
        public int CartId { get; set; }
        public Cart Cart { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; } = 1;
        public string SelectedSize { get; set; }
        public string Image { get; set; }

        [NotMapped]
        public IFormFile ImageFile { get; set; }
    }
}
