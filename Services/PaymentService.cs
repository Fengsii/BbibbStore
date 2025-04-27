using EFENGSI_RAHMANTO_ZALUKHU.Models.DB;
using EFENGSI_RAHMANTO_ZALUKHU.Models.DTO;
using EFENGSI_RAHMANTO_ZALUKHU.Models;
using Microsoft.EntityFrameworkCore;
using EFENGSI_RAHMANTO_ZALUKHU.Interfaces;
using static EFENGSI_RAHMANTO_ZALUKHU.Models.GeneralPaymentStatus;
using static EFENGSI_RAHMANTO_ZALUKHU.Models.GeneralOrderStatus;

namespace EFENGSI_RAHMANTO_ZALUKHU.Services
{
    public class PaymentService : IPayment
    {
        private readonly ApplicationContext _context;
        private readonly IUserSaldo _userSaldo;
        private readonly IUserProfile _userProfile;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public PaymentService(ApplicationContext context, IWebHostEnvironment hostingEnvironment, IUserSaldo userSaldo, IUserProfile userProfile)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _userSaldo = userSaldo;
            _userProfile = userProfile;
        }

        public List<PaymentDTO> GetListPayment()
        {
            var data = _context.Payments.Include(a => a.Order).Select(x => new PaymentDTO
            {
                Id = x.Id,
                OrderCode = x.Order.OrderCode,
                PaymentMethod = x.PaymentMethod,
                TotalAmount = x.TotalAmount,
                PaymentStatus = x.PaymentStatus,

            }).ToList();
            return data;

        }

        public Payment GetPaymentlById(int id)
        {
            var data = _context.Payments.FirstOrDefault(x => x.Id == id);
            if (data == null)
            {
                return new Payment();
            }
            return data;
        }

        public bool EditPayment(PaymentDTO paymentDTO)
        {
            var data = _context.Payments.FirstOrDefault(x => x.Id == paymentDTO.Id);
            if (data == null)
            {
                return false;
            }

            data.OrderId = paymentDTO.OrderId;
            data.PaymentMethod = paymentDTO.PaymentMethod;
            data.TotalAmount = paymentDTO.TotalAmount;
            data.PaymentStatus = paymentDTO.PaymentStatus;

            _context.Payments.Update(data);
            _context.SaveChanges();
            return true;
        }

        public bool DeletePayment(int id)
        {
            var data = _context.Payments.FirstOrDefault(x => x.Id == id);
            if (data == null)
            {
                return false;
            }

            _context.Payments.Remove(data);
            _context.SaveChanges();
            return true;
        }

        public bool AddPayment(PaymentDTO paymentDTO)
        {

            var data = new Payment();

            data.OrderId = paymentDTO.OrderId;
            data.PaymentMethod = paymentDTO.PaymentMethod;
            data.TotalAmount = paymentDTO.TotalAmount;
            data.PaymentStatus = paymentDTO.PaymentStatus;
            data.PaymentDate = DateTime.Now;

            _context.Payments.Add(data);
            _context.SaveChanges();
            return true;

        }

       
        public async Task<bool> ConfirmPayment(int orderId, string paymentMethod, string selectedBank, string proofImage)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var order = await _context.Orders
                    .Include(o => o.OrderDetails)
                    .FirstOrDefaultAsync(o => o.Id == orderId);

                if (order == null)
                {
                    System.Diagnostics.Debug.WriteLine("Order tidak ditemukan");
                    return false;
                }

                var totalAmount = order.OrderDetails.Sum(od => od.PriceAtPurchase * od.Quantity);
                System.Diagnostics.Debug.WriteLine($"Total pembayaran: {totalAmount}");

                // Dapatkan saldo user
                var userSaldo = await _context.UserSaldos
                    .FirstOrDefaultAsync(s => s.UserId == order.UserId);

                if (userSaldo == null)
                {
                    System.Diagnostics.Debug.WriteLine("Saldo user tidak ditemukan");
                    return false;
                }

                System.Diagnostics.Debug.WriteLine($"Saldo sebelum: {userSaldo.Saldo}");

                // Validasi saldo cukup
                if (userSaldo.Saldo < totalAmount)
                {
                    System.Diagnostics.Debug.WriteLine("Saldo tidak mencukupi");
                    return false;
                }

                // Kurangi saldo untuk SEMUA metode pembayaran
                userSaldo.Saldo -= totalAmount;
                _context.UserSaldos.Update(userSaldo);
                System.Diagnostics.Debug.WriteLine($"Saldo setelah: {userSaldo.Saldo}");

                // Simpan bukti pembayaran jika diperlukan
                string proofImagePath = null;
                if (paymentMethod != "cod" && !string.IsNullOrEmpty(proofImage))
                {
                    proofImagePath = await SaveProofImage(proofImage, orderId);
                    if (proofImagePath == null) return false;
                }

                // Buat record pembayaran
                var payment = new Payment
                {
                    OrderId = orderId,
                    PaymentMethod = paymentMethod,
                    BankName = paymentMethod == "bank-transfer" ? selectedBank : null,
                    PaymentDate = DateTime.Now,
                    PaymentStatus = paymentMethod == "cod"
                        ? GeneralPaymentStatusData.Pending
                        : GeneralPaymentStatusData.Completed,
                    TotalAmount = totalAmount,
                    ProofImage = proofImagePath
                };

                _context.Payments.Add(payment);

                // Update status order
                order.Status = paymentMethod == "cod"
                    ? GeneralOrderStatusData.Processing
                    : GeneralOrderStatusData.Shipped;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                System.Diagnostics.Debug.WriteLine("Pembayaran berhasil diproses");
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }

        private async Task<string> SaveProofImage(string base64Image, int orderId)
        {
            try
            {
                var base64Data = base64Image.Substring(base64Image.IndexOf(',') + 1);
                var bytes = Convert.FromBase64String(base64Data);

                var fileName = $"payment_proof_{orderId}_{Guid.NewGuid()}.jpg";
                var uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "uploads", "payment-proofs");

                // Pastikan folder ada
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var filePath = Path.Combine(uploadsFolder, fileName);
                await File.WriteAllBytesAsync(filePath, bytes);

                return $"/uploads/payment-proofs/{fileName}";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving proof: {ex.Message}");
                return null;
            }
        }
    }
}
