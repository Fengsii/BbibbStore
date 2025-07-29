using EFENGSI_RAHMANTO_ZALUKHU.Interfaces;
using EFENGSI_RAHMANTO_ZALUKHU.Models;
using EFENGSI_RAHMANTO_ZALUKHU.Models.DB;
using EFENGSI_RAHMANTO_ZALUKHU.Models.DTO;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using static EFENGSI_RAHMANTO_ZALUKHU.Models.GeneralOrderStatus;
using static EFENGSI_RAHMANTO_ZALUKHU.Models.GeneralPaymentStatus;
using static EFENGSI_RAHMANTO_ZALUKHU.Models.GeneralStatus;

namespace EFENGSI_RAHMANTO_ZALUKHU.Services
{
    public class OrderService : IOrder
    {
        private readonly ApplicationContext _context;
        private readonly IUserSaldo _userSaldo;

        public OrderService(ApplicationContext context, IUserSaldo userSaldo)
        {
            _context = context;
            _userSaldo = userSaldo;
        }

        public List<OrderDTO> GetListOrderan()
        {
            var data = _context.Orders.Include(a => a.User).Select(x => new OrderDTO
            {
                Id = x.Id,
                UserName = x.User.Name,
                OrderCode = x.OrderCode,
                Status = x.Status,
               

            }).ToList();
            return data;

        }

        public Order GetOrderanById(int id)
        {
            var data = _context.Orders.FirstOrDefault(x => x.Id == id);
            if (data == null)
            {
                return new Order();
            }
            return data;
        }

        public bool EditOrderan(OrderDTO orderanDTO)
        {
            var data = _context.Orders.FirstOrDefault(x => x.Id == orderanDTO.Id);
            if (data == null)
            {
                return false;
            }

            data.UserId = orderanDTO.UserId;
            data.OrderCode = orderanDTO.OrderCode;
            data.OrderDate = DateTime.Now;
            data.Status = orderanDTO.Status;

            _context.Orders.Update(data);
            _context.SaveChanges();
            return true;
        }

        public bool DeleteOrderan(int id)
        {
            var data = _context.Orders.FirstOrDefault(x => x.Id == id);
            if (data == null)
            {
                return false;
            }

            _context.Orders.Remove(data);
            _context.SaveChanges();
            return true;
        }

        public bool AddOrderan(OrderDTO orderanDTO)
        {

            var data = new Order();

            data.UserId = orderanDTO.UserId;
            data.OrderCode = orderanDTO.OrderCode;
            data.Status = orderanDTO.Status;
            data.OrderDate = DateTime.Now;

            _context.Orders.Add(data);
            _context.SaveChanges();
            return true;

        }

        public List<SelectListItem> Orders()
        {
            var datas = _context.Orders
                .Select(x => new SelectListItem
                {
                    Text = x.OrderCode,
                    Value = x.Id.ToString()
                }).ToList();

            return datas;
        }


        public int CreateNewOrder(int userId, int productId, int productSizeId, int quantity)
        {
            try
            {
                // Generate order code
                string orderCode = $"ORD-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 4).ToUpper()}";

               
                var product = _context.Products.Find(productId);
                var productSize = _context.ProductSizes
                    .Include(ps => ps.Product)
                    .FirstOrDefault(ps => ps.Id == productSizeId);

                if (product == null || productSize == null)
                {
                    throw new Exception("Produk atau ukuran produk tidak ditemukan");
                }

               
                if (productSize.Stock < quantity)
                {
                    
                    throw new Exception($"Stok tidak mencukupi untuk ukuran {productSize.Size}. Stok tersedia: {productSize.Stock}");
                }

                // Create new order
                var order = new Order
                {
                    UserId = userId,
                    OrderCode = orderCode,
                    OrderDate = DateTime.Now,
                    Status = GeneralOrderStatusData.Unpaid
                };

                _context.Orders.Add(order);
                _context.SaveChanges(); // Simpan untuk mendapatkan ID order

                // Create order detail
                var orderDetail = new OrderDetail
                {
                    OrderId = order.Id,
                    ProductId = productId,
                    Quantity = quantity,
                    SelectedSize = productSize.Size, 
                    PriceAtPurchase = product.Price,
                    Image = product.Image
                };

                _context.OrderDetails.Add(orderDetail);

                // Update stock
                productSize.Stock -= quantity;
                _context.ProductSizes.Update(productSize);

                _context.SaveChanges();

                return order.Id;
            }
            catch (Exception ex)
            {
                // Log error jika diperlukan
                throw new Exception("Gagal membuat order: " + ex.Message);
            }
        }





        //=========== BARU DITAMBAHKAN ==============\\

        public CheckoutResult CreateOrderFromCart(int userId, List<CartItemDTO> cartItems)
        {
            using var transaction = _context.Database.BeginTransaction();

            try
            {
                // Hitung total amount
                decimal totalAmount = cartItems.Sum(ci => ci.Quantity * ci.Price);

                // Cek saldo user jika metode pembayaran adalah saldo
                var userSaldo = _userSaldo.GetUserSaldoById(userId);
                if (userSaldo.Saldo < totalAmount)
                {
                    return new CheckoutResult
                    {
                        Success = false,
                        Message = "Saldo tidak mencukupi"
                    };
                }

                // Buat order
                var order = new Order
                {
                    UserId = userId,
                    OrderCode = GenerateOrderCode(),
                    OrderDate = DateTime.Now,
                    Status = GeneralOrderStatusData.Processing,
                };

                _context.Orders.Add(order);
                _context.SaveChanges();

                // Tambahkan order details
                foreach (var item in cartItems)
                {
                    var product = _context.Products.Find(item.ProductId);
                    if (product == null)
                        continue;

                    // Kurangi stok
                    var productSize = _context.ProductSizes
                        .FirstOrDefault(ps => ps.ProductId == item.ProductId && ps.Size == item.SelectedSize);

                    if (productSize == null || productSize.Stock < item.Quantity)
                    {
                        transaction.Rollback();
                        return new CheckoutResult
                        {
                            Success = false,
                            Message = $"Stok untuk produk {item.ProductName} ukuran {item.SelectedSize} tidak mencukupi"
                        };
                    }

                    productSize.Stock -= item.Quantity;

                    _context.OrderDetails.Add(new OrderDetail
                    {
                        OrderId = order.Id,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        SelectedSize = item.SelectedSize,
                        PriceAtPurchase = item.Price,
                        Image = item.Image
                    });
                }

                // Buat payment
                _context.Payments.Add(new Payment
                {
                    OrderId = order.Id,
                    PaymentMethod = "Saldo",
                    PaymentStatus = GeneralPaymentStatusData.Pending,
                    TotalAmount = totalAmount,
                    PaymentDate = DateTime.Now
                });

                _context.SaveChanges();
                transaction.Commit();

                return new CheckoutResult
                {
                    Success = true,
                    Message = "Order berhasil dibuat",
                    OrderId = order.Id
                };
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return new CheckoutResult
                {
                    Success = false,
                    Message = $"Error: {ex.Message}"
                };
            }
        }

        public List<OrderDTO> GetUserOrders(int userId)
        {
            return _context.Orders
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new OrderDTO
                {
                    Id = o.Id,
                    OrderCode = o.OrderCode,
                    OrderDate = o.OrderDate,
                    Status = o.Status
                })
                .ToList();
        }

        public OrderDetailDTO GetOrderDetails(int orderId, int userId)
        {
            return _context.Orders
                .Where(o => o.Id == orderId && o.UserId == userId)
                .Select(o => new OrderDetailDTO
                {
                    OrderId = o.Id,
                    OrderCode = o.OrderCode,
                    OrderDate = o.OrderDate,
                    Status = o.Status,
                    Items = o.OrderDetails.Select(od => new CartItemDTO
                    {
                        ProductId = od.ProductId,
                        ProductName = od.Product.Name,
                        Quantity = od.Quantity,
                        SelectedSize = od.SelectedSize,
                        Image = od.Image,
                        Price = od.PriceAtPurchase
                    }).ToList()
                })
                .FirstOrDefault();
        }

        public bool ProcessPayment(int orderId, string paymentMethod, string proofImage)
        {
            var payment = _context.Payments
                .Include(p => p.Order)
                .FirstOrDefault(p => p.OrderId == orderId);

            if (payment == null)
                return false;

            payment.PaymentMethod = paymentMethod;
            payment.PaymentStatus = GeneralPaymentStatusData.Completed;
            payment.ProofImage = proofImage;
            payment.Order.Status = GeneralOrderStatusData.Processing;

            _context.SaveChanges();
            return true;
        }

        private string GenerateOrderCode()
        {
            return "ORD" + DateTime.Now.ToString("yyyyMMddHHmmss") + new Random().Next(100, 999);
        }

    }
}
