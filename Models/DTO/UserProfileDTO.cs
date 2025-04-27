using System.ComponentModel.DataAnnotations.Schema;

namespace EFENGSI_RAHMANTO_ZALUKHU.Models.DTO
{
    public class UserProfileDTO
    {
        public int Id{ get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; } 
        public string Address { get; set; }
        public string Image { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
