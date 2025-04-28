using EFENGSI_RAHMANTO_ZALUKHU.Models.DTO;
using EFENGSI_RAHMANTO_ZALUKHU.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EFENGSI_RAHMANTO_ZALUKHU.Interfaces;
using EFENGSI_RAHMANTO_ZALUKHU.Models.DB;
using static EFENGSI_RAHMANTO_ZALUKHU.Models.GeneralOrderStatus;

namespace EFENGSI_RAHMANTO_ZALUKHU.Controllers
{
    public class PaymentDetailController : BaseController
    {
        private readonly IOrder _order;
        private readonly IProduct _product;
        private readonly IProductSize _productSize;
        private readonly IPayment _payment;
        private readonly ApplicationContext _context;

        public PaymentDetailController(
            IOrder order,
            IProduct product,
            IProductSize productSize,
            IPayment payment,
            ApplicationContext context)
        {
            _order = order;
            _product = product;
            _productSize = productSize;
            _payment = payment;
            _context = context;
        }

        public IActionResult Index(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                // Get order by ID
                var order = _order.GetOrderanById(id);
                var userSaldo = _context.UserSaldos
           .FirstOrDefault(us => us.UserId == userId); 

                if (order == null)
                {
                    TempData["Error"] = "Order tidak ditemukan";
                    return RedirectToAction("Index", "DashboardUser");
                }

               
                var orderDetails = _context.OrderDetails
                    .Include(od => od.Product)
                    .Where(od => od.OrderId == id)
                    .ToList();

                var paymentViewModel = new PaymentViewDTO
                {
                    OrderId = order.Id,
                    OrderCode = order.OrderCode,
                    OrderDetails = orderDetails,
                    TotalAmount = orderDetails.Sum(od => od.PriceAtPurchase * od.Quantity),
                    saldo = userSaldo.Saldo


                };

                return View(paymentViewModel);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Terjadi kesalahan saat memuat detail pembayaran: " + ex.Message;
                return RedirectToAction("Index", "DashboardUser");
            }
        }

        public IActionResult OrderHistory()
        {
            try
            {
                int userId = GetCurrentUserId(); 

               
                var orders = _context.Orders
                    .Where(o => o.UserId == userId)
                    .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                    .OrderByDescending(o => o.OrderDate)
                    .ToList();

               
                var viewModel = new OrderHistoryViewDTO
                {
                    AllOrders = orders,
                    CanceledOrders = orders.Where(o => o.Status == GeneralOrderStatusData.Canceled).ToList(),
                    UnpaidOrders = orders.Where(o => o.Status == GeneralOrderStatusData.Unpaid).ToList(),
                    ProcessingOrders = orders.Where(o => o.Status == GeneralOrderStatusData.Processing).ToList(),
                    ShippedOrders = orders.Where(o => o.Status == GeneralOrderStatusData.Shipped).ToList(),
                    DeliveredOrders = orders.Where(o => o.Status == GeneralOrderStatusData.Delivered).ToList()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Terjadi kesalahan saat memuat riwayat pesanan: " + ex.Message;
                return RedirectToAction("Index", "DashboardUser");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            try
            {
                var order = await _context.Orders.FindAsync(orderId);
                if (order == null)
                {
                    TempData["ErrorMessage"] = "Pesanan tidak ditemukan";
                    return RedirectToAction(nameof(OrderHistory));
                }

                // Hanya bisa cancel jika status Unpaid atau Processing
                if (order.Status != GeneralOrderStatus.GeneralOrderStatusData.Unpaid &&
                    order.Status != GeneralOrderStatus.GeneralOrderStatusData.Processing)
                {
                    TempData["ErrorMessage"] = "Pesanan tidak dapat dibatalkan pada status ini";
                    return RedirectToAction(nameof(OrderHistory));
                }

                order.Status = GeneralOrderStatus.GeneralOrderStatusData.Canceled;
                _context.Orders.Update(order);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Pesanan berhasil dibatalkan";
                return RedirectToAction(nameof(OrderHistory));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Gagal membatalkan pesanan: " + ex.Message;
                return RedirectToAction(nameof(OrderHistory));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Confirm(int orderId, string paymentMethod, string selectedBank, string proofImage)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"Memproses pembayaran OrderId: {orderId}");

                // Validasi input
                if (string.IsNullOrEmpty(paymentMethod))
                {
                    TempData["Error"] = "Silakan pilih metode pembayaran";
                    return RedirectToAction("Index", new { id = orderId });
                }

                // Validasi khusus transfer bank
                if (paymentMethod == "bank-transfer" && string.IsNullOrEmpty(selectedBank))
                {
                    TempData["Error"] = "Silakan pilih bank untuk transfer";
                    return RedirectToAction("Index", new { id = orderId });
                }

                // Validasi bukti pembayaran
                if (paymentMethod != "cod" && string.IsNullOrEmpty(proofImage))
                {
                    TempData["Error"] = "Silakan upload bukti pembayaran";
                    return RedirectToAction("Index", new { id = orderId });
                }

                // Proses pembayaran
                var result = await _payment.ConfirmPayment(orderId, paymentMethod, selectedBank, proofImage);

                if (result)
                {
                    TempData["Success"] = "Pembayaran berhasil dikonfirmasi";
                    return RedirectToAction("Index", "DashboardUser");
                }

                TempData["Error"] = "Gagal mengkonfirmasi pembayaran. Saldo mungkin tidak mencukupi";
                return RedirectToAction("Index", new { id = orderId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Terjadi kesalahan: {ex.Message}";
                return RedirectToAction("Index", new { id = orderId });
            }
        }




    }
}
