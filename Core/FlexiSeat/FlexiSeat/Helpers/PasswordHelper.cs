using System.Security.Cryptography;
using System.Text;

namespace FlexiSeat.Helpers
{
    public static class PasswordHelper
    {
        public static string HashPassword(string password)
        {
          using var sha = SHA256.Create();
          var bytes = Encoding.UTF8.GetBytes(password);
          return Convert.ToBase64String(sha.ComputeHash(bytes));
        }

        public static bool VerifyPassword(string password, string hash)
        {
          return HashPassword(password) == hash;
        }

        public static string Generate(int length = 16)
        {
          string Upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
          string Lower = "abcdefghijklmnopqrstuvwxyz";
          string Digits = "0123456789";
          string Special = "!@#$%^&*_-+=<>?";


          if (length < 8)
            throw new ArgumentException("Password length should be at least 8 characters.");

          string allChars = Upper + Lower + Digits + Special;
          var password = new StringBuilder();
          var randomBytes = new byte[length];

          using (var rng = RandomNumberGenerator.Create())
          {
            rng.GetBytes(randomBytes);
          }

          for (int i = 0; i < length; i++)
          {
            var index = randomBytes[i] % allChars.Length;
            password.Append(allChars[index]);
          }

          return password.ToString();
        }
     }
}
