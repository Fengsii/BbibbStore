using EFENGSI_RAHMANTO_ZALUKHU.Models.DB;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFENGSI_RAHMANTO_ZALUKHU.Models.DTO
{
    public class UserSaldoDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        [Column(TypeName = "decimal(18,2)")]
        public decimal Saldo { get; set; } = 0;
        public decimal CurrentSaldo { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
