using EFENGSI_RAHMANTO_ZALUKHU.Interfaces;
using EFENGSI_RAHMANTO_ZALUKHU.Models;
using EFENGSI_RAHMANTO_ZALUKHU.Models.DB;
using EFENGSI_RAHMANTO_ZALUKHU.Models.DTO;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using static EFENGSI_RAHMANTO_ZALUKHU.Models.GeneralStatus;

namespace EFENGSI_RAHMANTO_ZALUKHU.Services
{
    public class UserProfileService : IUserProfile
    {

        private readonly ApplicationContext _context;
        public UserProfileService(ApplicationContext context)
        {
            _context = context;
        }

     
        public User GetUserProfileById(int userId)
        {
           
            return _context.Users
                .FirstOrDefault(u => u.Id == userId && u.UserStatus != GeneralStatus.GeneralStatusData.delete);
        }

        public bool EditUserProfile(UserProfileDTO userProfileDTO)
        {
            try
            {
               
                var user = _context.Users.Find(userProfileDTO.Id);

                System.Diagnostics.Debug.WriteLine($"[TopUp POST] User Id Saldo: {userProfileDTO.Id}");

                if (user == null)
                {
                    return false;
                }

                // Update user properties
                user.Name = userProfileDTO.Name;
                user.Username = userProfileDTO.Username;
                user.Email = userProfileDTO.Email;
                user.PhoneNumber = userProfileDTO.PhoneNumber ?? "";
                user.Address = userProfileDTO.Address ?? "";
                user.Image = userProfileDTO.Image;
                user.UpdatedAt = DateTime.Now;


                System.Diagnostics.Debug.WriteLine($"[TopUp POST] User Profie: {user.Name}");
                System.Diagnostics.Debug.WriteLine($"[TopUp POST] User Profie: {user.Username}");
                System.Diagnostics.Debug.WriteLine($"[TopUp POST] User Profie: {user.Email}");
                System.Diagnostics.Debug.WriteLine($"[TopUp POST] User Profie: {user.PhoneNumber}");
                System.Diagnostics.Debug.WriteLine($"[TopUp POST] User Profie: {user.Address}");
                System.Diagnostics.Debug.WriteLine($"[TopUp POST] User Profie: {user.Image}");
                // Save changes to database
                _context.Update(user);
                _context.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating user profile: {ex.Message}");
                return false;
            }
        }


        // ===================== SALDO SERVICE =====================
        public UserSaldo GetUserSaldoById(int userId)
        {
            var data = _context.UserSaldos.FirstOrDefault(s => s.UserId == userId);
            if (data == null)
            {
                return new UserSaldo();
            }
            return data;
        }

       
        public bool AddUserSaldo(UserSaldoDTO userSaldoDTO)
        {
            try
            {
                var existingSaldo = _context.UserSaldos.FirstOrDefault(s => s.UserId == userSaldoDTO.UserId);

                if (existingSaldo == null)
                {
                    // Validasi saldo tidak boleh negatif untuk record baru
                    if (userSaldoDTO.Saldo < 0)
                    {
                        System.Diagnostics.Debug.WriteLine("Saldo awal tidak boleh negatif");
                        return false;
                    }

                    var newSaldo = new UserSaldo
                    {
                        UserId = userSaldoDTO.UserId,
                        Saldo = userSaldoDTO.Saldo,
                        LastUpdated = DateTime.Now
                    };
                    _context.UserSaldos.Add(newSaldo);
                }
                else
                {
                    // Validasi saldo akhir tidak boleh negatif
                    if (existingSaldo.Saldo + userSaldoDTO.Saldo < 0)
                    {
                        System.Diagnostics.Debug.WriteLine("Saldo tidak mencukupi");
                        return false;
                    }

                    existingSaldo.Saldo += userSaldoDTO.Saldo;
                    existingSaldo.LastUpdated = DateTime.Now;
                    _context.UserSaldos.Update(existingSaldo);
                }

                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }

        // Method baru khusus untuk mengurangi saldo
        public bool ReduceUserSaldo(int userId, decimal amount)
        {
            try
            {
                if (amount <= 0)
                {
                    System.Diagnostics.Debug.WriteLine("Jumlah pengurangan harus positif");
                    return false;
                }

                var existingSaldo = _context.UserSaldos.FirstOrDefault(s => s.UserId == userId);
                if (existingSaldo == null)
                {
                    System.Diagnostics.Debug.WriteLine("Saldo user tidak ditemukan");
                    return false;
                }

                if (existingSaldo.Saldo < amount)
                {
                    System.Diagnostics.Debug.WriteLine("Saldo tidak mencukupi");
                    return false;
                }

                existingSaldo.Saldo -= amount;
                existingSaldo.LastUpdated = DateTime.Now;
                _context.UserSaldos.Update(existingSaldo);
                _context.SaveChanges();

                System.Diagnostics.Debug.WriteLine($"Saldo berhasil dikurangi. Saldo sekarang: {existingSaldo.Saldo}");
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error mengurangi saldo: {ex.Message}");
                return false;
            }
        }



    }
}
