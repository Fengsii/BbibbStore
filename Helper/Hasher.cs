using System.Security.Cryptography;
using System.Text;

namespace EFENGSI_RAHMANTO_ZALUKHU.Helper
{
    public class Hasher
    {
        public static string ComputeHash(string pwd, string salt, string pepper, int iteration)
        {
            if (iteration <= 0) return pwd;

            using var sha512 = SHA512.Create();
            var saltPepper = $"{pwd}{salt}{pepper}";
            var byteValue = Encoding.UTF8.GetBytes(saltPepper);
            var byteHash = sha512.ComputeHash(byteValue);
            var hash = Convert.ToBase64String(byteHash);
            return ComputeHash(hash, salt, pepper, iteration - 1);
        }

        public static string GenerateSalt()
        {
            using var rng = RandomNumberGenerator.Create();
            var byteSalt = new byte[16];
            rng.GetBytes(byteSalt);
            var salt = Convert.ToBase64String(byteSalt);
            var length = salt.Length;
            return salt;

        }
    }
}
