using EFENGSI_RAHMANTO_ZALUKHU.Models.DB;
using EFENGSI_RAHMANTO_ZALUKHU.Models.DTO;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EFENGSI_RAHMANTO_ZALUKHU.Interfaces
{
    public interface IOrder
    {
        public bool AddOrderan(OrderDTO orderanDTO);
        public bool EditOrderan(OrderDTO orderanDTO);
        public Order GetOrderanById(int id);
        public List<OrderDTO> GetListOrderan();
        public bool DeleteOrderan(int id);
        public List<SelectListItem> Orders();
        public int CreateNewOrder(int userId, int productId, int productSizeId, int quantity);

    }
}
