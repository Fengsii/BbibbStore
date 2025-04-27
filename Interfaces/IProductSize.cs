using EFENGSI_RAHMANTO_ZALUKHU.Models.DB;
using EFENGSI_RAHMANTO_ZALUKHU.Models.DTO;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EFENGSI_RAHMANTO_ZALUKHU.Interfaces
{
    public interface IProductSize
    {
        public List<ProductSizeDTO> GetlistProductSize();
        public ProductSize GetProductSizeById(int id);
        public bool EditProductSize(ProductSizeDTO productSizeDTO);
        public bool DeleteProductSize(int id);
        public bool AddProduct(ProductSizeDTO productSize);
        public List<SelectListItem> ProductSizes();
        public List<ProductSizeDTO> GetListProductSizeById(int productId);
    }
}
