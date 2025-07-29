using EFENGSI_RAHMANTO_ZALUKHU.Interfaces;
using EFENGSI_RAHMANTO_ZALUKHU.Models;
using EFENGSI_RAHMANTO_ZALUKHU.Models.DB;
using EFENGSI_RAHMANTO_ZALUKHU.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace EFENGSI_RAHMANTO_ZALUKHU.Services
{
    public class CartService : ICart
    {
        private readonly ApplicationContext _context;
        private readonly IOrder _orderService;

        public CartService(ApplicationContext context, IOrder orderService)
        {
            _context = context;
            _orderService = orderService;
        }

        public bool AddToCart(int productId, string size, int quantity, int userId)
        {
            try
            {
                // Cek apakah produk ada
                var product = _context.Products.Find(productId);
                if (product == null)
                {
                    Console.WriteLine("Produk tidak ditemukan");
                    return false;
                }

                // Cek stok
                var productSize = _context.ProductSizes
                    .FirstOrDefault(ps => ps.ProductId == productId && ps.Size == size);

                if (productSize == null || productSize.Stock < quantity)
                {
                    Console.WriteLine("Stok tidak mencukupi atau ukuran tidak tersedia");
                    return false;
                }

                // Cek apakah user sudah punya cart
                var cart = _context.Carts
                    .Include(c => c.CartItems)
                    .FirstOrDefault(c => c.UserId == userId);

                if (cart == null)
                {
                    cart = new Cart { UserId = userId };
                    _context.Carts.Add(cart);
                    _context.SaveChanges();
                    Console.WriteLine("Cart baru dibuat untuk user");
                }

                // Cek apakah item sudah ada di cart
                var existingItem = cart.CartItems.FirstOrDefault(ci =>
                    ci.ProductId == productId && ci.SelectedSize == size);

                if (existingItem != null)
                {
                    existingItem.Quantity += quantity;
                    Console.WriteLine("Item sudah ada, jumlah diperbarui");
                }
                else
                {
                    cart.CartItems.Add(new CartItem
                    {
                        ProductId = productId,
                        Quantity = quantity,
                        SelectedSize = size,
                        Image = product.Image
                    });
                    Console.WriteLine("Item baru ditambahkan ke cart");
                }

                _context.SaveChanges();
                Console.WriteLine("Perubahan disimpan ke database");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }
        public List<CartItemDTO> GetCartItems(int userId)
        {
            return _context.Carts
                .Where(c => c.UserId == userId)
                .SelectMany(c => c.CartItems)
                .Include(ci => ci.Product)
                .Select(ci => new CartItemDTO
                {
                    Id = ci.Id,
                    ProductId = ci.ProductId,
                    ProductName = ci.Product.Name,
                    Quantity = ci.Quantity,
                    SelectedSize = ci.SelectedSize,
                    Image = ci.Image,
                    Price = ci.Product.Price
                })
                .ToList();
        }

        public bool RemoveFromCart(int cartItemId)
        {
            var cartItem = _context.CartItems.Find(cartItemId);
            if (cartItem == null)
                return false;

            _context.CartItems.Remove(cartItem);
            _context.SaveChanges();
            return true;
        }

        public bool UpdateCartItemQuantity(int cartItemId, int quantity)
        {
            if (quantity <= 0)
                return false;

            var cartItem = _context.CartItems
                .Include(ci => ci.Product)
                .ThenInclude(p => p.ProductSizes)
                .FirstOrDefault(ci => ci.Id == cartItemId);

            if (cartItem == null)
                return false;

            var productSize = cartItem.Product.ProductSizes
                .FirstOrDefault(ps => ps.Size == cartItem.SelectedSize);

            if (productSize == null || productSize.Stock < quantity)
                return false;

            cartItem.Quantity = quantity;
            _context.SaveChanges();
            return true;
        }

        public CheckoutResult Checkout(int userId)
        {
            // Dapatkan cart items
            var cartItems = GetCartItems(userId);
            if (!cartItems.Any())
                return new CheckoutResult { Success = false, Message = "Keranjang kosong" };

            // Buat order
            var orderResult = _orderService.CreateOrderFromCart(userId, cartItems);
            if (!orderResult.Success)
                return orderResult;

            // Kosongkan keranjang
            var cart = _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefault(c => c.UserId == userId);

            if (cart != null)
            {
                _context.CartItems.RemoveRange(cart.CartItems);
                _context.SaveChanges();
            }

            return orderResult;
        }
    }

}
