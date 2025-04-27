using EFENGSI_RAHMANTO_ZALUKHU.Models.DB;
using EFENGSI_RAHMANTO_ZALUKHU.Models.DTO;

namespace EFENGSI_RAHMANTO_ZALUKHU.Interfaces
{
    public interface IOrderDetail
    {
        public bool AddOrderDetail(OrderDetailDTO orderDetailDTO);
        public bool EditOrderDetail(OrderDetailDTO orderDetailDTO);
        public OrderDetail GetOrderDetailById(int id);
        public List<OrderDetailDTO> GetListOrderDetail();
        public bool DeleteOrderDetail(int id);

    }
}
