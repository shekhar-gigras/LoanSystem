using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace Gigras.Software.General.Helper
{
    public static class PasswordHelper
    {
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("Loan-2024-2030"); // Replace with a secret key
        public static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        public static string GenerateVerificationToken(DateTime expirationTime)
        {
            var tokenPayload = new
            {
                Token = Guid.NewGuid().ToString(), // Unique token
                Expiry = expirationTime.Ticks // Store expiry timestamp
            };

            var payloadJson = JsonConvert.SerializeObject(tokenPayload);
            var payloadBytes = Encoding.UTF8.GetBytes(payloadJson);

            using (var hmac = new HMACSHA256(Key))
            {
                var hash = hmac.ComputeHash(payloadBytes);
                var signature = Convert.ToBase64String(hash);

                var finalToken = Convert.ToBase64String(payloadBytes) + "." + signature;
                return finalToken;
            }
        }

        public static bool ValidateVerificationToken(string token)
        {
            try
            {
                var parts = token.Split('.');
                if (parts.Length != 2) return false;

                var payloadBytes = Convert.FromBase64String(parts[0]);
                var signature = parts[1];

                using (var hmac = new HMACSHA256(Key))
                {
                    var computedHash = hmac.ComputeHash(payloadBytes);
                    var computedSignature = Convert.ToBase64String(computedHash);

                    if (computedSignature != signature)
                        return false; // Invalid signature
                }

                var payloadJson = Encoding.UTF8.GetString(payloadBytes);
                var tokenPayload = JsonConvert.DeserializeObject<dynamic>(payloadJson);

                long expiryTicks = tokenPayload!.Expiry;
                var expiryDate = new DateTime(expiryTicks, DateTimeKind.Utc);

                return DateTime.UtcNow <= expiryDate; // Check expiry
            }
            catch
            {
                return false; // Invalid token format
            }
        }
    }
}