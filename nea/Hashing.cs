using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Data.SQLite;

namespace nea
{
    internal class Hashing
    {
        public static string HashPassword(string password)
        {
            byte[] salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create()) 
            {
                rng.GetBytes(salt);
            }

            var pbk = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = pbk.GetBytes(20);

            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            return Convert.ToBase64String(hashBytes);

        }

        public static bool PasswordValidation(string password, string storedHash) 
        {
            byte[] hashbytes = Convert.FromBase64String(storedHash);

            byte[] salt = new byte[16];
            Array.Copy(hashbytes, 0, salt, 0, 16);

            var pbk = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = pbk.GetBytes(20);

            for (int i = 0; i < 20; i++) 
            {
                if (hashbytes[i + 16] != hash[i]) 
                {
                    return false;
                }
            }
            return true;
        }

        public static void AddUser(string username, string password) 
        {
            string HashedPassword = HashPassword(password);

            using (var connection = Database.GetConnection())
            {
                connection.Open();
                string sql = "INSERT INTO Users (Username, PasswordHash) VALUES (@Username, @PasswordHash);";
                using (var cmd = new SQLiteCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@PasswordHash", HashedPassword);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static bool Login(string username, string password) 
        {
            using (var connection = new SQLiteConnection())
            {
                string sql = "SELECT PasswordHash FROM Users WHERE Username = @Username";
                using (var command = new SQLiteCommand(sql, connection)) 
                {
                    command.Parameters.AddWithValue("@Username", username);
                    
                }
            }   
            return true;
        }
    }
}
