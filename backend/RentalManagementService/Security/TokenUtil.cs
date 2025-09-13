// Security/TokenUtil.cs
using System.Security.Cryptography;
using System.Text;

namespace RentalManagementService.Security
{
    public static class TokenUtil
    {
        public static string GenerateResetToken(int bytes = 32)
        {
            var buf = RandomNumberGenerator.GetBytes(bytes);
            return Base64UrlEncode(buf);
        }

        public static string HashToken(string token)
        {
            // SHA256 is fine for reset tokens (random, not passwords)
            using var sha = SHA256.Create();
            var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(token));
            return Convert.ToHexString(hash); // uppercase hex
        }

        private static string Base64UrlEncode(byte[] bytes) =>
            Convert.ToBase64String(bytes).Replace("+", "-").Replace("/", "_").TrimEnd('=');
    }
}
