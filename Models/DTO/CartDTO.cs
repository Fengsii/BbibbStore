using EFENGSI_RAHMANTO_ZALUKHU.Models.DB;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFENGSI_RAHMANTO_ZALUKHU.Models.DTO
{
    public class CartDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Image { get; set; }

    }
}
