﻿using static EFENGSI_RAHMANTO_ZALUKHU.Models.GeneralOrderStatus;

namespace EFENGSI_RAHMANTO_ZALUKHU.Models.DTO
{
    public class OrderDTO
    {
        public int Id { get; set; }
        public string OrderCode { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public DateTime OrderDate { get; set; }
        public GeneralOrderStatusData Status { get; set; }
    }
}
