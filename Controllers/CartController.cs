using EFENGSI_RAHMANTO_ZALUKHU.Interfaces;
using EFENGSI_RAHMANTO_ZALUKHU.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EFENGSI_RAHMANTO_ZALUKHU.Controllers
{
    public class CartController : BaseController
    {
        private readonly ICart _cart;

        public CartController(ICart cart)
        {
            _cart = cart;
        }
        public IActionResult Index()
        {
            var userId = GetCurrentUserId();
            var cartItems = _cart.GetCartItems(userId);
            return View(cartItems);
        }

        //[HttpPost]
        //public IActionResult AddToCart(int productId, string size, int quantity)
        //{
        //    var userId = GetCurrentUserId();
        //    if (_cart.AddToCart(productId, size, quantity, userId))
        //    {
        //        TempData["SuccessMessage"] = "Produk berhasil ditambahkan ke keranjang";
        //    }
        //    else
        //    {
        //        TempData["ErrorMessage"] = "Gagal menambahkan produk ke keranjang";
        //    }
        //    return RedirectToAction("Index");
        //}

        [HttpPost]
        public IActionResult AddToCart(int productId, string size, int quantity)
        {
            var userId = GetCurrentUserId();

            // Debug: Cek parameter yang diterima
            Console.WriteLine($"ProductId: {productId}, Size: {size}, Quantity: {quantity}, UserId: {userId}");

            if (userId == 0)
            {
                TempData["ErrorMessage"] = "Anda harus login terlebih dahulu";
                return RedirectToAction("Login", "Account");
            }

            if (_cart.AddToCart(productId, size, quantity, userId))
            {
                TempData["SuccessMessage"] = "Produk berhasil ditambahkan ke keranjang";
            }
            else
            {
                TempData["ErrorMessage"] = "Gagal menambahkan produk ke keranjang";
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult UpdateQuantity(int cartItemId, int quantity)
        {
            if (_cart.UpdateCartItemQuantity(cartItemId, quantity))
            {
                TempData["SuccessMessage"] = "Jumlah produk berhasil diupdate";
            }
            else
            {
                TempData["ErrorMessage"] = "Gagal mengupdate jumlah produk";
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Remove(int cartItemId)
        {
            if (_cart.RemoveFromCart(cartItemId))
            {
                TempData["SuccessMessage"] = "Produk berhasil dihapus dari keranjang";
            }
            else
            {
                TempData["ErrorMessage"] = "Gagal menghapus produk dari keranjang";
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Checkout()
        {
            var userId = GetCurrentUserId();
            var result = _cart.Checkout(userId);

            if (result.Success)
            {
                return RedirectToAction("Checkout", "Order", new { orderId = result.OrderId });
            }

            TempData["ErrorMessage"] = result.Message;
            return RedirectToAction("Index");
        }

    }
}
