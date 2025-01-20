using System.Security.Cryptography;
using System.Text;

namespace Gigras.Software.General.Helper
{
    public static class PasswordHelper
    {
        public static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        public static string GenerateVerificationToken()
        {
            using (var hmac = new HMACSHA256())
            {
                var tokenBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString()));
                return Convert.ToBase64String(tokenBytes);
            }
        }
    }
}