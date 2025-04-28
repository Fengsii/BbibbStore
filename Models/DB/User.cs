using System.ComponentModel.DataAnnotations.Schema;
using static EFENGSI_RAHMANTO_ZALUKHU.Models.GeneralStatus;

namespace EFENGSI_RAHMANTO_ZALUKHU.Models.DB
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Image { get; set; }
        [NotMapped]
        public IFormFile ImageFile { get; set; }
        public string Salt { get; set; }
        public string PasswordHash { get; set; } // Di-hash
        public string Role { get; set; } // "Admin" atau "User"
        public GeneralStatusData UserStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ICollection<UserSaldo> UserSaldos { get; set; } = new List<UserSaldo>();
        public ICollection<Order> Orders { get; set; } = new List<Order>();

    }
}
