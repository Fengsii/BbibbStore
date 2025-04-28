using EFENGSI_RAHMANTO_ZALUKHU.Models.DTO;
using EFENGSI_RAHMANTO_ZALUKHU.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using EFENGSI_RAHMANTO_ZALUKHU.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EFENGSI_RAHMANTO_ZALUKHU.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly IAuthentication _authentication;
        private readonly ApplicationContext _context;
        public AuthenticationController (IAuthentication authentication, ApplicationContext context)
        {
            _authentication = authentication;
            _context = context;
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }
        public IActionResult Index()
        {
            var data = _authentication.GetAllUser();
            return View(data);
        }

        public IActionResult Edit(int id)
        {
            var data = _authentication.GetUserById(id);
            return View(data);
        }

        [HttpPost]
        public IActionResult Edit(RegisterDTO registerDTO)
        {
            var data = _authentication.EditUser(registerDTO);
            if (data)
            {
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var data = _authentication.DeleteUser(id);
            if (data)
            {
                return RedirectToAction(nameof(Index));
            }
            return BadRequest("Gagal menghapus User.");
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            var (success, role, userId) = await _authentication.Login(loginDTO);

            if (success)
            {
                var user = await _context.Users.FindAsync(userId);

                
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.NameIdentifier, user.Username),
                    new Claim(ClaimTypes.Role, user.Role),
                    new Claim("UserId", user.Id.ToString())
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                // Konfigurasi properti autentikasi berdasarkan pilihan "Remember Me"
                var authProperties = new AuthenticationProperties();

                if (loginDTO.RememberMe)
                {
                  
                    authProperties.IsPersistent = true;
                    authProperties.ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7);

                    Console.WriteLine("Remember Me diaktifkan. Cookie akan bertahan 7 hari.");
                }
                else
                {
                   
                    authProperties.IsPersistent = false;
                    authProperties.ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(1);

                    Console.WriteLine("Remember Me tidak diaktifkan. Cookie akan bertahan selama sesi browser.");
                }

               
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal,
                    authProperties
                );

            
                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetString("UserRole", user.Role);
                HttpContext.Session.SetString("UserName", user.Name);

              
                if (role == "Admin")
                {
                    return RedirectToAction("Index", "DashboardAdmin");
                }
                else if (role == "User")
                {
                    return RedirectToAction("Index", "DashboardUser");
                }
            }

            TempData["ErrorMessage"] = "Username atau Password salah!";
            return View(loginDTO);
        }

        public async Task<IActionResult> Logout()
        {
           
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

           
            HttpContext.Session.Clear();

            return RedirectToAction("Login", "Authentication");
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {
            //if (registerDTO.Password.Length < 7)
            //{
            //    TempData["ErrorMessage"] = "Password minimal 7 karakter";
            //    return View(registerDTO);
            //}

            if (registerDTO.Password != registerDTO.ConfirmPassword)
            {
                TempData["ErrorMessage"] = "Password dan Konfirmasi Password harus sama!";
                return View(registerDTO);
            }

            var existingUser = await _context.Users
                .FirstOrDefaultAsync(x => x.Username == registerDTO.Username || x.Email == registerDTO.Email);

            if (existingUser != null)
            {
                TempData["ErrorMessage"] = "Username atau Email sudah digunakan. Silakan pilih yang lain.";
                return View(registerDTO);
            }

            try
            {
                var result = await _authentication.Register(registerDTO);
                if (result)
                {
                    TempData["SuccessMessage"] = "Registrasi berhasil! Silakan login.";
                    return RedirectToAction("Login");
                }

                TempData["ErrorMessage"] = "Gagal mendaftarkan user. Silakan coba lagi.";
                return View(registerDTO);
            }
            catch (Exception ex)
            {
                // Log exception untuk debugging
                Console.WriteLine($"Error registrasi: {ex.Message}");
                // Log juga inner exception jika ada
                if (ex.InnerException != null)
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");

                TempData["ErrorMessage"] = $"Terjadi kesalahan saat mendaftarkan user: {ex.Message}";
                return View(registerDTO);
            }
        }
    }
}
