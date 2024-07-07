using System.Security.Cryptography;
using System.Text;

namespace Task2.Services
{
    public static class PasswordHasher
    {
        public static byte[] HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

                // Ensure the hashedBytes is exactly 50 bytes long
                if (hashedBytes.Length < 50)
                {
                    Array.Resize(ref hashedBytes, 50);
                }
                else if (hashedBytes.Length > 50)
                {
                    Array.Resize(ref hashedBytes, 50);
                }

                return hashedBytes;
            }
        }

        public static bool VerifyPassword(string enteredPassword, byte[] storedHash)
        {
            byte[] enteredHash = HashPassword(enteredPassword);
            return AreByteArraysEqual(enteredHash, storedHash);
        }

        // Constant-time comparison to prevent timing attacks
        private static bool AreByteArraysEqual(byte[] a, byte[] b)
        {
            if (a == null || b == null || a.Length != b.Length)
            {
                return false;
            }
            var areSame = true;
            for (var i = 0; i < a.Length; i++)
            {
                areSame &= (a[i] == b[i]);
            }
            return areSame;
        }
    }

}
