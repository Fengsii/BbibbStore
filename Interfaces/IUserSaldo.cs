using EFENGSI_RAHMANTO_ZALUKHU.Models.DB;
using EFENGSI_RAHMANTO_ZALUKHU.Models.DTO;

namespace EFENGSI_RAHMANTO_ZALUKHU.Interfaces
{
    public interface IUserSaldo
    {
        public List<UserSaldoDTO> GetListUserSaldo();
        public UserSaldo GetUserSaldoById(int id);
        //public UserSaldo GetUserSaldoByUser(User user);
        public bool EditUserSaldo(UserSaldoDTO userSaldoDTO);
        public bool AddUserSaldo(UserSaldoDTO userSaldoDTO);
        public bool DeleteUserSaldo(int id);

        //public UserSaldo GetUsersSaldoById(int userId);
        //public bool AddUsersSaldo(UserSaldoDTO userSaldoDTO);
    }
}
