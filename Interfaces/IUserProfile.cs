using EFENGSI_RAHMANTO_ZALUKHU.Models.DB;
using EFENGSI_RAHMANTO_ZALUKHU.Models.DTO;

namespace EFENGSI_RAHMANTO_ZALUKHU.Interfaces
{
    public interface IUserProfile
    {
        public User GetUserProfileById(int id);
        public bool EditUserProfile(UserProfileDTO userProfileDTO);
        public UserSaldo GetUserSaldoById(int id);
        public bool AddUserSaldo(UserSaldoDTO userSaldoDTO);
        public bool ReduceUserSaldo(int userId, decimal amount);
    }
}
