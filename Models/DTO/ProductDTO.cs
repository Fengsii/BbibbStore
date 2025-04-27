using static EFENGSI_RAHMANTO_ZALUKHU.Models.GeneralStatus;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFENGSI_RAHMANTO_ZALUKHU.Models.DTO
{
    public class ProductDTO
    {
        public int Id { get; set; } 
        public string Name { get; set; }
        public string Description { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        public string Image { get; set; }
        public string CategoryName { get; set; }
        public int CategoryId { get; set; }
        public GeneralStatusData ProductStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
