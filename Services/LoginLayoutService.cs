using EFENGSI_RAHMANTO_ZALUKHU.Interfaces;

namespace EFENGSI_RAHMANTO_ZALUKHU.Services
{
    public class LoginLayoutService : ILoginLayout
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LoginLayoutService (IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        // Method untuk mendapatkan layout berdasarkan peran pengguna
        public string GetLayout()
        {
            var user = _httpContextAccessor.HttpContext.User;

            if (user.IsInRole("Admin"))
            {
                return "~/Views/Shared/_LayoutAdmin.cshtml"; // Layout untuk Admin
            }
            else if (user.IsInRole("User"))
            {
                return "~/Views/Shared/_LayoutUser.cshtml"; // Layout untuk User
            }

            return "~/Views/Shared/_Layout.cshtml"; // Default layout jika tidak ada peran
        }
    }
}
