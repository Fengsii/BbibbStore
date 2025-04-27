using EFENGSI_RAHMANTO_ZALUKHU.Models.DB;
using EFENGSI_RAHMANTO_ZALUKHU.Models.DTO;
using EFENGSI_RAHMANTO_ZALUKHU.Models;
using Microsoft.EntityFrameworkCore;
using EFENGSI_RAHMANTO_ZALUKHU.Interfaces;

namespace EFENGSI_RAHMANTO_ZALUKHU.Services
{
    public class OrderDetailService : IOrderDetail
    {
        private readonly ApplicationContext _context;

        public OrderDetailService (ApplicationContext context)
        {
            _context = context;
        }

        public List<OrderDetailDTO> GetListOrderDetail()
        {
            var data = _context.OrderDetails.Include(a => a.Order).Include(a => a.Product).Select(x => new OrderDetailDTO
            {
                Id = x.Id,
                OrderCode = x.Order.OrderCode,
                ProductName = x.Product.Name,
                Image = x.Image,
                Quantity = x.Quantity,
                SelectedSize = x.SelectedSize,
                PriceAtPurchase = x.PriceAtPurchase,


            }).ToList();
            return data;

        }

        public OrderDetail GetOrderDetailById(int id)
        {
            var data = _context.OrderDetails.FirstOrDefault(x => x.Id == id);
            if (data == null)
            {
                return new OrderDetail();
            }
            return data;
        }

        public bool EditOrderDetail(OrderDetailDTO orderDetailDTO)
        {
            var data = _context.OrderDetails.FirstOrDefault(x => x.Id == orderDetailDTO.Id);
            if (data == null)
            {
                return false;
            }

            data.OrderId = orderDetailDTO.OrderId;
            data.ProductId = orderDetailDTO.ProductId;
            data.Image = orderDetailDTO.Image;
            data.Quantity = orderDetailDTO.Quantity;
            data.SelectedSize = orderDetailDTO.SelectedSize;
            data.PriceAtPurchase = orderDetailDTO.PriceAtPurchase;

            _context.OrderDetails.Update(data);
            _context.SaveChanges();
            return true;
        }

        public bool DeleteOrderDetail(int id)
        {
            var data = _context.OrderDetails.FirstOrDefault(x => x.Id == id);
            if (data == null)
            {
                return false;
            }

            _context.OrderDetails.Remove(data);
            _context.SaveChanges();
            return true;
        }

        public bool AddOrderDetail(OrderDetailDTO orderDetailDTO)
        {

            var data = new OrderDetail();

            data.OrderId = orderDetailDTO.OrderId;
            data.ProductId = orderDetailDTO.ProductId;
            data.Image = orderDetailDTO.Image;
            data.Quantity = orderDetailDTO.Quantity;
            data.SelectedSize = orderDetailDTO.SelectedSize;
            data.PriceAtPurchase = orderDetailDTO.PriceAtPurchase;

            _context.OrderDetails.Add(data);
            _context.SaveChanges();
            return true;

        }
    }
}
