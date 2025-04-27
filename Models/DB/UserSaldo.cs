using System.ComponentModel.DataAnnotations.Schema;

namespace EFENGSI_RAHMANTO_ZALUKHU.Models.DB
{
    public class UserSaldo
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Saldo { get; set; } = 0;
        public User User { get; set; }
        public DateTime LastUpdated { get; set; }

    }
}
