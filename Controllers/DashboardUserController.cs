using EFENGSI_RAHMANTO_ZALUKHU.Filters;
using EFENGSI_RAHMANTO_ZALUKHU.Interfaces;
using EFENGSI_RAHMANTO_ZALUKHU.Models;
using EFENGSI_RAHMANTO_ZALUKHU.Models.DB;
using EFENGSI_RAHMANTO_ZALUKHU.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EFENGSI_RAHMANTO_ZALUKHU.Controllers
{
    [Authorize]
    public class DashboardUserController : Controller
    {
        private readonly IProduct _product;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IUserProfile _userProfile;
        public DashboardUserController(IProduct product, IWebHostEnvironment hostingEnvironment, IUserProfile userProfile, ApplicationContext context)
        {
           _product = product;
           _hostingEnvironment = hostingEnvironment;
           _userProfile = userProfile;
        }

        public IActionResult Index()
        {
            try
            {
                var users =_product.GetListProduct();

                // Debugging
                if (users != null)
                {
                    foreach (var p in users)
                    {
                        System.Diagnostics.Debug.WriteLine($"Produk: {p.Name}, Gambar: {p.Image}");
                    }
                }

                return View("~/Views/DashboardUser/Index.cshtml", users);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
                TempData["Error"] = "Terjadi kesalahan saat memuat produk";
                return View(new List<ProductDTO>());
            }
        }
    }
}
