using EFENGSI_RAHMANTO_ZALUKHU.Interfaces;
using EFENGSI_RAHMANTO_ZALUKHU.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace EFENGSI_RAHMANTO_ZALUKHU.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IPayment _payment;
        private readonly IOrder _order;

        public PaymentController(IPayment payment, IOrder order)
        {
            _payment = payment;
            _order = order;
        }


        // GET: ProductController
        public ActionResult Index()
        {
            var data = _payment.GetListPayment();
            return View(data);
        }



        public IActionResult Edit(int id)
        {
            ViewBag.Order = _order.Orders();
            var data = _payment.GetPaymentlById(id);
            return View(data);
        }

        [HttpPost]
        public IActionResult Edit(PaymentDTO paymentDTO)
        {
            if (paymentDTO.Id == 0)
            {
                var data = _payment.AddPayment(paymentDTO);
                if (data)
                {
                    return RedirectToAction(nameof(Index));
                }

            }
            else
            {
                var data = _payment.EditPayment(paymentDTO);
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
            var data = _payment.DeletePayment(id);
            if (data)
            {
                return RedirectToAction(nameof(Index));
            }
            return BadRequest("Gagal menghapus.");
        }
    }
}
