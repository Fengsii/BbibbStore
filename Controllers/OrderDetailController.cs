using EFENGSI_RAHMANTO_ZALUKHU.Interfaces;
using EFENGSI_RAHMANTO_ZALUKHU.Models.DB;
using EFENGSI_RAHMANTO_ZALUKHU.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace EFENGSI_RAHMANTO_ZALUKHU.Controllers
{
    public class OrderDetailController : Controller
    {
        private readonly IOrderDetail _orderDetail;
        private readonly IOrder _order;
        private readonly IProduct _product;

        public OrderDetailController (IOrder order, IOrderDetail orderDetail, IProduct product)
        {
            _orderDetail = orderDetail;
            _order = order;
            _product = product;
        }


        // GET: ProductController
        public ActionResult Index()
        {
            var data = _orderDetail.GetListOrderDetail();
            return View(data);
        }



        public IActionResult Edit(int id)
        {
            ViewBag.Order = _order.Orders();
            ViewBag.Product = _product.Products();
            var data = _orderDetail.GetOrderDetailById(id);
            return View(data);
        }

        [HttpPost]
        public IActionResult Edit(OrderDetailDTO orderDTO)
        {
            if (orderDTO.Id == 0)
            {
                var data = _orderDetail.AddOrderDetail(orderDTO);
                if (data)
                {
                    return RedirectToAction(nameof(Index));
                }

            }
            else
            {
                var data = _orderDetail.EditOrderDetail(orderDTO);
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
            var data = _orderDetail.DeleteOrderDetail(id);
            if (data)
            {
                return RedirectToAction(nameof(Index));
            }
            return BadRequest("Gagal menghapus.");
        }
    }
}
