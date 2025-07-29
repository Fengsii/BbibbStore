using EFENGSI_RAHMANTO_ZALUKHU.Interfaces;
using EFENGSI_RAHMANTO_ZALUKHU.Models.DTO;
using EFENGSI_RAHMANTO_ZALUKHU.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EFENGSI_RAHMANTO_ZALUKHU.Controllers
{
    public class OrderController : BaseController
    {
        private readonly IOrder _orderan;
        private readonly IAuthentication _authentication;

        public OrderController (IOrder orderan, IAuthentication authentication)
        {
            _orderan = orderan;
            _authentication = authentication;
        }


        // GET: ProductController
        public ActionResult Index()
        {
            var data = _orderan.GetListOrderan();
            return View(data);
        }



        public IActionResult Edit(int id)
        {
            ViewBag.User = _authentication.Users();
            var data = _orderan.GetOrderanById(id);
            return View(data);
        }

        [HttpPost]
        public IActionResult Edit(OrderDTO orderanDTO)
        {
            if (orderanDTO.Id == 0)
            {
                var data = _orderan.AddOrderan(orderanDTO);
                if (data)
                {
                    return RedirectToAction(nameof(Index));
                }

            }
            else
            {
                var data = _orderan.EditOrderan(orderanDTO);
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
            var data = _orderan.DeleteOrderan(id);
            if (data)
            {
                return RedirectToAction(nameof(Index));
            }
            return BadRequest("Gagal menghapus produk.");
        }


        //============================== New ================\\
        public IActionResult Checkout(int orderId)
        {
            var userId = GetCurrentUserId();
            var orderDetails = _orderan.GetOrderDetails(orderId, userId);

            if (orderDetails == null)
            {
                return NotFound();
            }

            return View(orderDetails);
        }

        [HttpPost]
        public IActionResult ProcessPayment(int orderId, string paymentMethod, IFormFile proofImage)
        {
            var userId = GetCurrentUserId();

            string proofImagePath = null;
            if (proofImage != null && proofImage.Length > 0)
            {
                var fileName = $"payment_{orderId}_{DateTime.Now:yyyyMMddHHmmss}{Path.GetExtension(proofImage.FileName)}";
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/payments", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    proofImage.CopyTo(stream);
                }

                proofImagePath = $"payments/{fileName}";
            }

            if (_orderan.ProcessPayment(orderId, paymentMethod, proofImagePath))
            {
                TempData["SuccessMessage"] = "Pembayaran berhasil diproses";
            }
            else
            {
                TempData["ErrorMessage"] = "Gagal memproses pembayaran";
            }

            return RedirectToAction("Details", new { id = orderId });
        }

        public IActionResult Details(int id)
        {
            var userId = GetCurrentUserId();
            var orderDetails = _orderan.GetOrderDetails(id, userId);

            if (orderDetails == null)
            {
                return NotFound();
            }

            return View(orderDetails);
        }

        public IActionResult History()
        {
            var userId = GetCurrentUserId();
            var orders = _orderan.GetUserOrders(userId);
            return View(orders);
        }


    }
}
