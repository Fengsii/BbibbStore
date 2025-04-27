using static EFENGSI_RAHMANTO_ZALUKHU.Models.GeneralStatus;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFENGSI_RAHMANTO_ZALUKHU.Models.DB
{
    public class Product
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        public string Image { get; set; }

        [NotMapped]
        public IFormFile ImageFile { get; set; }
        public int CategoryId { get; set; } // Foreign key ke Category
        public Category Category { get; set; }
        public GeneralStatusData ProductStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        public ICollection<ProductSize> ProductSizes { get; set; } = new List<ProductSize>();
        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
        //public ICollection<Cart> Carts { get; set; } = new List<Cart>();
        //public ICollection<Review> Reviews { get; set; } = new List<Review>();
        //public ICollection<Wishlists> Wishlists { get; set; } = new List<Wishlists>();

    }
}
