using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BCSH2_BDAS2_Armadni_Informacni_System.Helpers
{
    public static class PasswordHasher
    {
        // Vytvoření SHA-256 hashe z hesla
        public static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2")); // Převod na hexadecimální řetězec
                }
                return builder.ToString();
            }
        }

        // Verifikace hesla: zahashuje zadané heslo a porovná s uloženým hashem
        public static bool VerifyPassword(string enteredPassword, string storedHash)
        {
            string enteredHash = HashPassword(enteredPassword); // Získání hashe zadaného hesla
            return enteredHash == storedHash; // Porovnání s hashem v databázi
        }
    }
}