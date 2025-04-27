using EFENGSI_RAHMANTO_ZALUKHU.Interfaces;
using EFENGSI_RAHMANTO_ZALUKHU.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace EFENGSI_RAHMANTO_ZALUKHU.Controllers
{
    public class OrderController : Controller
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
    }
}
