using EFENGSI_RAHMANTO_ZALUKHU.Interfaces;
using EFENGSI_RAHMANTO_ZALUKHU.Models.DB;
using EFENGSI_RAHMANTO_ZALUKHU.Models.DTO;
using EFENGSI_RAHMANTO_ZALUKHU.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using static EFENGSI_RAHMANTO_ZALUKHU.Models.GeneralStatus;

namespace EFENGSI_RAHMANTO_ZALUKHU.Services
{
    public class ProductSizeService : IProductSize
    {
        private readonly ApplicationContext _context;

        public ProductSizeService(ApplicationContext context)
        {
            _context = context;
        }
        public List<ProductSizeDTO> GetlistProductSize()
        {
            var data = _context.ProductSizes.Include(y => y.Product).Select(x => new ProductSizeDTO
            {
                Id = x.Id,
                Size = x.Size,
                Stock = x.Stock,
                ProductName = x.Product.Name,

            }).ToList();
            return data;

        }
        public ProductSize GetProductSizeById(int id)
        {
            var data = _context.ProductSizes.FirstOrDefault(x => x.Id == id);
            if (data == null)
            {
                return new ProductSize();
            }
            return data;
        }

        public List<ProductSizeDTO> GetListProductSizeById(int productId)
        {
            var data = _context.ProductSizes
                .Where(x => x.ProductId == productId)
                .Select(x => new ProductSizeDTO
                {
                    Id = x.Id,
                    Size = x.Size,
                    Stock = x.Stock,
                    ProductId = x.ProductId,
                    ProductName = x.Product.Name
                })
                .ToList();

            return data;
        }

        public bool EditProductSize(ProductSizeDTO productSizeDTO)
        {
            var data = _context.ProductSizes.FirstOrDefault(x => x.Id == productSizeDTO.Id);
            if (data == null)
            {
                return false;
            }
            data.ProductId = productSizeDTO.ProductId;
            data.Size = productSizeDTO.Size;
            data.Stock = productSizeDTO.Stock;

            _context.ProductSizes.Update(data);
            _context.SaveChanges();
            return true;
        }
        public bool DeleteProductSize(int id)
        {
            var data = _context.ProductSizes.FirstOrDefault(x => x.Id == id);
            if (data == null)
            {
                return false;
            }
            
            _context.ProductSizes.Remove(data);
            _context.SaveChanges();
            return true;
        }
        public bool AddProduct(ProductSizeDTO productSize)
        {

            var data = new ProductSize();

            data.Size = productSize.Size;
            data.Stock = productSize.Stock;
            data.ProductId = productSize.ProductId;

            _context.ProductSizes.Add(data);
            _context.SaveChanges();
            return true;
        }
        public List<SelectListItem> ProductSizes()
        {
            var datas = _context.ProductSizes
                .Select(x => new SelectListItem
                {
                    Text = x.Size,
                    Value = x.Id.ToString()
                }).ToList();

            return datas;
        }
    }
}
