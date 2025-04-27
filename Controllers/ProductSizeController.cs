using EFENGSI_RAHMANTO_ZALUKHU.Interfaces;
using EFENGSI_RAHMANTO_ZALUKHU.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace EFENGSI_RAHMANTO_ZALUKHU.Controllers
{
    public class ProductSizeController : Controller
    {
        private readonly IProduct _product;
        private readonly IProductSize _productSize;

        public ProductSizeController(IProductSize productSize, IProduct product)
        {
            _productSize = productSize;
            _product = product;
        }


        // GET: ProductController
        public ActionResult Index()
        {
            var data = _productSize.GetlistProductSize();
            return View(data);
        }



        public IActionResult Edit(int id)
        {
            ViewBag.Product = _product.Products();
            var data = _productSize.GetProductSizeById(id);
            return View(data);
        }

        [HttpPost]
        public IActionResult Edit(ProductSizeDTO productSizeDTO)
        {
            if (productSizeDTO.Id == 0)
            {
                var data = _productSize.AddProduct(productSizeDTO);
                if (data)
                {
                    return RedirectToAction(nameof(Index));
                }

            }
            else
            {
                var data = _productSize.EditProductSize(productSizeDTO);
                if (data)
                {
                    return RedirectToAction(nameof(Index));
                }

            }
            return View();

        }


        [HttpPost]
        public IActionResult Delete(int id)
        {
            var data = _productSize.DeleteProductSize(id);
            if (data)
            {
                return RedirectToAction(nameof(Index));
            }
            return BadRequest("Gagal menghapus Size.");
        }

    }
}
