using EFENGSI_RAHMANTO_ZALUKHU.Models;
using EFENGSI_RAHMANTO_ZALUKHU.Models.DB;
using EFENGSI_RAHMANTO_ZALUKHU.Models.DTO;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EFENGSI_RAHMANTO_ZALUKHU.Interfaces
{
    public interface IAuthentication
    {
        Task<bool> Register(RegisterDTO registerDTO);
        Task<(bool Success, string Role, int UserId)> Login(LoginDTO loginDTO);
        public List<RegisterDTO> GetAllUser();
        public User GetUserById(int id);
        public bool EditUser(RegisterDTO registerDTO);
        public bool DeleteUser(int id);
        public List<SelectListItem> Users();

        public bool EditUserProfilee(UserProfileDTO userProfileDTO);
        public User GetUserProfileeById(int id);
    }
}
