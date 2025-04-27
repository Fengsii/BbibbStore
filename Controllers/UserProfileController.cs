using EFENGSI_RAHMANTO_ZALUKHU.Interfaces;
using EFENGSI_RAHMANTO_ZALUKHU.Models.DB;
using EFENGSI_RAHMANTO_ZALUKHU.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.Intrinsics.X86;

namespace EFENGSI_RAHMANTO_ZALUKHU.Controllers
{
    public class UserProfileController : BaseController
    {
        private readonly IUserProfile _userProfile;
        private readonly IAuthentication _authentication;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public UserProfileController(IUserProfile userProfile, IWebHostEnvironment hostingEnvironment, IAuthentication authentication)
        {
            _userProfile = userProfile;
            _hostingEnvironment = hostingEnvironment;
            _authentication = authentication;
        }

        // GET: /UserProfile
        public IActionResult Index()
        {
            // Get current user ID from session
            int userId = GetCurrentUserId();

            if (userId == 0)
            {
                return RedirectToAction("Login", "Authentication");
            }

            // Get user profile from database
            var user = _authentication.GetUserProfileeById(userId);

            if (user == null)
            {
                TempData["ErrorMessage"] = "User profile not found";
                return RedirectToAction("Login", "Authentication");
            }

            // Map user data to DTO for the view
            var userProfileDTO = new UserProfileDTO
            {
                Id = user.Id,
                Name = user.Name,
                Username = user.Username,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                Image = user.Image,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };

            return View(userProfileDTO);
        }


        [HttpPost]
        public async Task<IActionResult> Index(UserProfileDTO userProfileDTO, IFormFile ImageFile)
        {
           
            int userId = GetCurrentUserId();
            userProfileDTO.Id = userId;

            // Get current user data
            var currentUser = _authentication.GetUserProfileeById(userId);
            if (currentUser == null)
            {
                TempData["ErrorMessage"] = "User not found";
                return View(userProfileDTO);
            }

            // Process image upload if provided
            if (ImageFile != null && ImageFile.Length > 0)
            {
                // Validate file extension
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var fileExtension = Path.GetExtension(ImageFile.FileName).ToLower();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    TempData["ErrorMessage"] = "Only image files (jpg, jpeg, png, gif) are allowed";
                    return View(userProfileDTO);
                }

                // Validate file size (max 5MB)
                if (ImageFile.Length > 5 * 1024 * 1024)
                {
                    TempData["ErrorMessage"] = "File size exceeds the limit (5MB)";
                    return View(userProfileDTO);
                }

                try
                {
                    // Ensure uploads directory exists
                    var uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "uploads");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    // Create unique filename
                    var uniqueFileName = $"{userId}_{Guid.NewGuid()}{fileExtension}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Save file
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(fileStream);
                    }

                    // Delete old image if exists
                    if (!string.IsNullOrEmpty(currentUser.Image))
                    {
                        var oldImagePath = Path.Combine(_hostingEnvironment.WebRootPath, currentUser.Image.TrimStart('/'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    // Update image path in DTO
                    userProfileDTO.Image = "/uploads/" + uniqueFileName;
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Error uploading image: {ex.Message}";
                    return View(userProfileDTO);
                }
            }
            else
            {
                // Keep existing image if no new one is uploaded
                userProfileDTO.Image = currentUser.Image;
            }

            // Tambahkan UpdatedAt sebelum menyimpan
            userProfileDTO.UpdatedAt = DateTime.Now;

            // Save changes to database
            var success = _authentication.EditUserProfilee(userProfileDTO);
            System.Diagnostics.Debug.WriteLine($"[TopUp POST] UserId dari session: {success}");

            if (success)
            {
                TempData["SuccessMessage"] = "Profile updated successfully!";
                return RedirectToAction("Index", "UserProfile");
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to update profile. Please try again.";
                return View(userProfileDTO);  // Tambahkan return di sini agar tidak lanjut ke kode berikutnya
            }
        }



        // =================== SALDO CONTROL ===================
        public IActionResult TopUpSaldo()
        {
            int userLoginId = GetCurrentUserId();
            var userSaldo = _userProfile.GetUserSaldoById(userLoginId);

            // Create DTO for view
            var saldoDTO = new UserSaldoDTO
            {
                Id = userSaldo.Id,
                UserId = userLoginId,
                Saldo = 0, // Initial amount to add will be empty
                CurrentSaldo = userSaldo.Saldo // Pass current balance to view
            };

            return View(saldoDTO);
        }

        [HttpPost]
        public IActionResult TopUpSaldo(UserSaldoDTO userSaldoDTO)
        {
            int userLoginId = GetCurrentUserId();
            userSaldoDTO.UserId = userLoginId;

            System.Diagnostics.Debug.WriteLine($"[TopUp POST] UserId dari session: {userLoginId}");
            System.Diagnostics.Debug.WriteLine($"[TopUp POST] Nominal Saldo: {userSaldoDTO.Saldo}");
            if (ModelState.IsValid)
            {
                //int userLoginId = GetCurrentUserId();
                //userSaldoDTO.UserId = userLoginId;

                var result = _userProfile.AddUserSaldo(userSaldoDTO);
                System.Diagnostics.Debug.WriteLine($"User Id : {result}");
                if (result)
                {
                    System.Diagnostics.Debug.WriteLine($"berhasil : {result}");
                    TempData["SuccessMessage"] = $"Top-up sebesar Rp {userSaldoDTO.Saldo.ToString("N0")} berhasil dilakukan!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"gagal : {result}");
                    TempData["ErrorMessage"] = "Gagal melakukan top-up saldo. Silakan coba lagi.";
                }
            }
            System.Diagnostics.Debug.WriteLine($"Back : {userSaldoDTO.Id}");
            // If we get here, something went wrong
            return RedirectToAction(nameof(Index));
        }

       
    }
}
