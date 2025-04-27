using EFENGSI_RAHMANTO_ZALUKHU.Filters;
using EFENGSI_RAHMANTO_ZALUKHU.Helper;
using EFENGSI_RAHMANTO_ZALUKHU.Interfaces;
using EFENGSI_RAHMANTO_ZALUKHU.Models;
using EFENGSI_RAHMANTO_ZALUKHU.Models.DB;
using EFENGSI_RAHMANTO_ZALUKHU.Models.DTO;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace EFENGSI_RAHMANTO_ZALUKHU.Controllers
{
    public class ProductController : BaseController
    {

        private readonly IProduct _product;
        private readonly ICategory _category;
        private readonly IProductSize _productSize;
        private readonly IOrder _order;
        private readonly IWebHostEnvironment _hostingEnvironment;
       

        public ProductController(
            IProduct product,
            ICategory category,
            IProductSize productSize,
            IOrder order,
            IWebHostEnvironment hostingEnvironment)
        {
            _product = product;
            _category = category;
            _productSize = productSize;
            _order = order;
            _hostingEnvironment = hostingEnvironment;
           
        }


        [HttpPost]
        public async Task<IActionResult> Edit(Product product)
        {
            if (product.ImageFile != null && product.ImageFile.Length > 0)
            {
                // Validasi ekstensi file
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var fileExtension = Path.GetExtension(product.ImageFile.FileName).ToLower();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("ImageFile", "Only image files (jpg, jpeg, png, gif) are allowed.");
                    return View(product);
                }

                // Validasi ukuran file (maksimal 5MB)
                if (product.ImageFile.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError("ImageFile", "The image file is too large (max 5MB).");
                    return View(product);
                }

                // Pastikan folder uploads ada
                var uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Generate nama file unik
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + product.ImageFile.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Simpan file ke folder uploads
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await product.ImageFile.CopyToAsync(fileStream);
                }

                // Hapus gambar lama jika ada
                if (!string.IsNullOrEmpty(product.Image))
                {
                    var oldImagePath = Path.Combine(_hostingEnvironment.WebRootPath, product.Image.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                // Simpan path gambar baru ke properti Image
                product.Image = "/uploads/" + uniqueFileName;
            }
            else if (product.Id == 0) // Jika produk baru dan tidak ada gambar yang diunggah
            {
                product.Image = "/images/default-product.png";
            }

            // Konversi ke DTO dan simpan ke database
            var productDto = new ProductDTO
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Image = product.Image,
                CategoryId = product.CategoryId,
                ProductStatus = product.ProductStatus
            };

            if (product.Id == 0)
            {
                _product.AddProduct(productDto);
            }
            else
            {
                _product.EditProduct(productDto);
            }

            return RedirectToAction(nameof(Index));
        }


        public IActionResult Index()
        {
            try
            {
                var products = _product.GetListProduct();
                System.Diagnostics.Debug.WriteLine("cek321");
                if (products == null || !products.Any())
                {
                    System.Diagnostics.Debug.WriteLine("cek43");
                    TempData["Warning"] = "Tidak ada produk yang tersedia";
                    products = new List<ProductDTO>();
                   
                }
                System.Diagnostics.Debug.WriteLine("cek4");
                return View(products);
            }
            catch (Exception ex)
            {
                // Log error (gunakan ILogger jika ada)
                TempData["Error"] = "Terjadi kesalahan saat memuat produk: " + ex.Message;
                return View(new List<ProductDTO>());
            }
        }
       
        public IActionResult Edit(int id)
        {
            ViewBag.Category = _category.Categories();
            var data = _product.GetProductById(id);
            return View(data);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var data = _product.DeleteProduct(id);
            if (data)
            {
                return RedirectToAction(nameof(Index));
            }
            return BadRequest("Gagal menghapus produk.");
        }


        public IActionResult Detail(int id)
        {
            try
            {
                var product = _product.GetProductById(id);
                if (product == null)
                {
                    TempData["Error"] = "Produk tidak ditemukan";
                    return RedirectToAction(nameof(Index));
                }

                // Pastikan GetProductById mengembalikan ProductDTO, bukan Product
                // Jika memang mengembalikan Product, konversi ke ProductDTO:

                // Jika product adalah tipe Product, konversi ke ProductDTO
                if (product is Product productEntity)
                {
                    var productDto = new ProductDTO
                    {
                        Id = productEntity.Id,
                        Name = productEntity.Name,
                        Description = productEntity.Description,
                        Price = productEntity.Price,
                        Image = productEntity.Image,
                        CategoryId = productEntity.CategoryId,
                        CategoryName = productEntity.Category?.Name, 
                        ProductStatus = productEntity.ProductStatus,
                        CreatedAt = productEntity.CreatedAt,
                        UpdatedAt = productEntity.UpdatedAt
                    };

                    var productSizes = _productSize.GetListProductSizeById(id);
                    ViewBag.ProductSizes = productSizes;

                    //return View("ProductDetail", productDto);
                    return View("/Views/ProductDetail/Index.cshtml", productDto);

                }

                var sizes = _productSize.GetListProductSizeById(id);
                ViewBag.ProductSizes = sizes;

                //return View("ProductDetail", product);
                return View("/Views/ProductDetail/Index.cshtml", product);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Terjadi kesalahan saat memuat detail produk: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }


        [HttpPost]
        [RequireLogin] // Tambahkan atribut ini untuk memastikan pengguna sudah login
        public IActionResult Checkout(int productId, int productSizeId, int quantity)
        {
            try
            {
                
                var userId = GetCurrentUserId();
                if (userId == 0) 
                {
                    TempData["Error"] = "Anda harus login terlebih dahulu";
                    return RedirectToAction("Login", "Authentication");
                }


               
                int orderId = _order.CreateNewOrder(
                    userId: userId,
                    productId: productId,
                    productSizeId: productSizeId,
                    quantity: quantity
                );

                TempData["Success"] = "Order berhasil dibuat! Silakan lanjutkan pembayaran.";

                return RedirectToAction("Index", "PaymentDetail", new { id = orderId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Terjadi kesalahan saat checkout: " + ex.Message;
                return RedirectToAction("Detail", new { id = productId });
            }
        }


       
    }

}

  
