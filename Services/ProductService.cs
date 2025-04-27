using EFENGSI_RAHMANTO_ZALUKHU.Models.DB;
using EFENGSI_RAHMANTO_ZALUKHU.Models.DTO;
using EFENGSI_RAHMANTO_ZALUKHU.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using static EFENGSI_RAHMANTO_ZALUKHU.Models.GeneralStatus;
using Microsoft.EntityFrameworkCore;
using EFENGSI_RAHMANTO_ZALUKHU.Interfaces;

namespace EFENGSI_RAHMANTO_ZALUKHU.Services
{
    public class ProductService : IProduct
    {
        private readonly ApplicationContext _context;

        public ProductService(ApplicationContext conteks)
        {
            _context = conteks;
        }

        public List<ProductDTO> GetListProduct()
        {
            var data = _context.Products.Include(y => y.Category).Where(x => x.ProductStatus != GeneralStatusData.delete).Select(x => new ProductDTO
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                Price = x.Price,
                Image = x.Image,
                CategoryName = x.Category.Name,
                ProductStatus = x.ProductStatus,

            }).ToList();
            return data;

        }

        public Product GetProductById(int id)
        {
            var data = _context.Products.Where(x => x.Id == id && x.ProductStatus != GeneralStatusData.delete).FirstOrDefault();
            if (data == null)
            {
                return new Product();
            }

            return data;
        }

        public bool EditProduct(ProductDTO productDTO)
        {
            var data = _context.Products.FirstOrDefault(x => x.Id == productDTO.Id);
            if (data == null)
            {
                return false;
            }
            data.CategoryId = productDTO.CategoryId;
            data.Name = productDTO.Name;
            data.Description = productDTO.Description;
            data.Price = productDTO.Price;
            data.Image = productDTO.Image; 
            data.ProductStatus = productDTO.ProductStatus;
            data.UpdatedAt = DateTime.Now;

            _context.Products.Update(data);
            _context.SaveChanges();
            return true;
        }

        public bool DeleteProduct(int id)
        {
            var data = _context.Products.FirstOrDefault(x => x.Id == id);
            if (data == null)
            {
                return false;
            }

            data.ProductStatus = GeneralStatusData.delete;
            _context.SaveChanges();
            return true;
        }

        public bool AddProduct(ProductDTO productDTO)
        {
            var data = new Product();

            data.Name = productDTO.Name;
            data.Description = productDTO.Description;
            data.Price = productDTO.Price;
            data.Image = productDTO.Image;
            data.ProductStatus = productDTO.ProductStatus;
            data.CategoryId = productDTO.CategoryId;
            data.CreatedAt = DateTime.Now;

            _context.Products.Add(data);
            _context.SaveChanges();
            return true;

        }

        public List<SelectListItem> Products()
        {
            var datas = _context.Products
                .Where(x => x.ProductStatus == GeneralStatusData.Published)
                .Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }).ToList();

            return datas;
        }
    }
}
