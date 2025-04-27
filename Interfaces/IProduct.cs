using EFENGSI_RAHMANTO_ZALUKHU.Models.DB;
using EFENGSI_RAHMANTO_ZALUKHU.Models.DTO;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EFENGSI_RAHMANTO_ZALUKHU.Interfaces
{
    public interface IProduct
    {
        public List<ProductDTO> GetListProduct();
        public Product GetProductById(int id);
        public bool EditProduct(ProductDTO productDTO);
        public bool DeleteProduct(int id);
        public bool AddProduct(ProductDTO productDTO);
        public List<SelectListItem> Products();
    }
}
