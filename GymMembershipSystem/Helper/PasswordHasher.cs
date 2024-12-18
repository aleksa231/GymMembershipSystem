using System;
using System.Security.Cryptography;
using System.Text;

public class PasswordHasher
{
    public static string HashPassword(string password)
    {
        if (string.IsNullOrEmpty(password))
        {
            throw new ArgumentException("Password cannot be null or empty.", nameof(password));
        }

        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] hashBytes = sha256.ComputeHash(passwordBytes);

            StringBuilder hashStringBuilder = new StringBuilder();

            foreach (byte b in hashBytes)
            {
                hashStringBuilder.Append(b.ToString("x2")); // Converts to lowercase hex
            }

            return hashStringBuilder.ToString();
        }
    }
}
