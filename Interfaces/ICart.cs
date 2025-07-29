using EFENGSI_RAHMANTO_ZALUKHU.Models;
using EFENGSI_RAHMANTO_ZALUKHU.Models.DTO;

namespace EFENGSI_RAHMANTO_ZALUKHU.Interfaces
{
    public interface ICart
    {
        bool AddToCart(int productId, string size, int quantity, int userId);
        public List<CartItemDTO> GetCartItems(int userId);
        public bool RemoveFromCart(int cartItemId);
        public bool UpdateCartItemQuantity(int cartItemId, int quantity);
        public CheckoutResult Checkout(int userId);
    }
}
