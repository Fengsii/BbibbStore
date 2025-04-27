using EFENGSI_RAHMANTO_ZALUKHU.Helper;
using EFENGSI_RAHMANTO_ZALUKHU.Models.DB;
using EFENGSI_RAHMANTO_ZALUKHU.Models.DTO;
using EFENGSI_RAHMANTO_ZALUKHU.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using static EFENGSI_RAHMANTO_ZALUKHU.Models.GeneralStatus;
using Microsoft.EntityFrameworkCore;
using EFENGSI_RAHMANTO_ZALUKHU.Interfaces;

namespace EFENGSI_RAHMANTO_ZALUKHU.Services
{
    public class AuthenticationService : IAuthentication
    {
        private readonly ApplicationContext _context;
        private readonly string _paper;
        private readonly string _iteration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthenticationService (ApplicationContext conteks, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _paper = configuration.GetSection("Security:Papper").Value ?? "";
            _iteration = configuration.GetSection("Security:Iteration").Value ?? "";
            _context = conteks;
            _httpContextAccessor = httpContextAccessor;
        }

      
        public async Task<bool> Register(RegisterDTO registerDTO)
        {
            try
            {
                if (await _context.Users.AnyAsync(u => u.Username == registerDTO.Username || u.Email == registerDTO.Email))
                    return false;

                var salt = Hasher.GenerateSalt();

                var user = new User
                {
                    Name = registerDTO.Name,
                    Username = registerDTO.Username,
                    Email = registerDTO.Email,
                    PhoneNumber = "",
                    Address = "",
                    Image = "",
                    Salt = salt,
                    PasswordHash = Hasher.ComputeHash(registerDTO.Password, salt, _paper, Convert.ToInt32(_iteration)),
                    Role = "User", // Default role
                    CreatedAt = DateTime.UtcNow,
                    UserStatus = GeneralStatus.GeneralStatusData.Published
                };


                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Simpan session
                _httpContextAccessor.HttpContext?.Session.SetInt32("UserId", user.Id);
                _httpContextAccessor.HttpContext?.Session.SetString("UserRole", user.Role);
                _httpContextAccessor.HttpContext?.Session.SetString("UserName", user.Name);

                // Tambah saldo user berdasarkan user.Id
                _context.UserSaldos.Add(new UserSaldo { UserId = user.Id });
                await _context.SaveChangesAsync();

                //_context.UserSaldos.Add(new UserSaldo { UserId = user.Id });
                //await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error di AuthService.Register: {ex.Message}");
                if (ex.InnerException != null)
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");

                throw; 
            }
        }

        public async Task<(bool Success, string Role, int UserId)> Login(LoginDTO loginDTO)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.Username == loginDTO.Username &&
                    u.UserStatus == GeneralStatus.GeneralStatusData.Published);

            if (user == null)
            {
                return (false, null, 0); 
            }
            System.Diagnostics.Debug.WriteLine($"Percobaan login untuk: {loginDTO.Username}");
            System.Diagnostics.Debug.WriteLine($"Hash tersimpan: {user.PasswordHash}");
            System.Diagnostics.Debug.WriteLine($"Salt: {user.Salt}");
            System.Diagnostics.Debug.WriteLine($"Pepper: {_paper}");
            System.Diagnostics.Debug.WriteLine($"Iteration: {_iteration}");

            var hashResult = Hasher.ComputeHash(loginDTO.Password, user.Salt, _paper, Convert.ToInt32(_iteration));

            if (hashResult == user.PasswordHash && loginDTO.Username == user.Username)
            {
                return (true, user.Role, user.Id); 
            }
            else
            {
                return (false, null, 0);
            }
        }

        public List<RegisterDTO> GetAllUser()
        {
            var data = _context.Users.Where(x => x.UserStatus != GeneralStatusData.delete).Select(x => new RegisterDTO
            {
                Id = x.Id,
                Name = x.Name,
                Username = x.Username,
                Email = x.Email,
                PhoneNumber = x.PhoneNumber,
                Address = x.Address,
                Password = "*******",
                ConfirmPassword = "*******",
                UserStatus = x.UserStatus

            }).ToList();
            return data;
        }

        public User GetUserById(int id)
        {
            var data = _context.Users.Where(x => x.Id == id && x.UserStatus != GeneralStatusData.delete).FirstOrDefault();
            if (data == null)
            {
                return new User();
            }

            return data;
        }

        public bool EditUser(RegisterDTO registerDTO)
        {
            var data = _context.Users.FirstOrDefault(x => x.Id == registerDTO.Id);
            if (data == null)
            {
                return false;
            }

            data.CreatedAt = DateTime.Now;
            data.UserStatus = registerDTO.UserStatus;


            _context.Users.Update(data);
            _context.SaveChanges();
            return true;
        }

        public bool EditUserProfilee(UserProfileDTO userProfileDTO)
        {
            var data = _context.Users.FirstOrDefault(x => x.Id == userProfileDTO.Id);
            if (data == null)
            {
                return false;
            }
            data.Name = userProfileDTO.Name;
            data.Username = userProfileDTO.Username;
            data.Email = userProfileDTO.Email;
            data.PhoneNumber = userProfileDTO.PhoneNumber;
            data.Address = userProfileDTO.Address;
            data.Image = userProfileDTO.Image;
            // Perbaiki: Jangan set CreatedAt di sini, tapi set UpdatedAt
            // data.CreatedAt = DateTime.Now;  <- Hapus ini
            data.UpdatedAt = userProfileDTO.UpdatedAt; // Gunakan nilai dari DTO

            _context.Users.Update(data);
            _context.SaveChanges();
            return true;
        }

        public User GetUserProfileeById(int id)
        {
            var data = _context.Users.Where(x => x.Id == id && x.UserStatus != GeneralStatusData.delete).FirstOrDefault();
            if (data == null)
            {
                return new User();
            }

            return data;
        }



        public bool DeleteUser(int id)
        {
            var data = _context.Users.FirstOrDefault(x => x.Id == id);
            if (data == null)
            {
                return false;
            }

            data.UserStatus = GeneralStatusData.delete;
            _context.SaveChanges();
            return true;
        }

        public List<SelectListItem> Users()
        {
            var datas = _context.Users
                .Where(x => x.UserStatus == GeneralStatusData.Published)
                .Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }).ToList();


            return datas;
        }
    }
}
