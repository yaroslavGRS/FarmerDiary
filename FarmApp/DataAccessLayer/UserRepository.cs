using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.Security.Cryptography;
using System.Text;

namespace DataAccessLayer
{
    public class UserRepository
    {
        private readonly string connectionString;

        public UserRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        // Реєстрація нового користувача
        public bool RegisterUser(string email, string password)
        {
            string passwordHash = HashPassword(password);

            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "INSERT INTO User (Email, PasswordHash) VALUES (@Email, @PasswordHash)";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);

                    try
                    {
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                    catch (MySqlException ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                        return false;
                    }
                }
            }
        }

        // Логін користувача
        public bool LoginUser(string email, string password)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT PasswordHash FROM User WHERE Email = @Email";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Email", email);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string storedHash = reader.GetString("PasswordHash");
                            return VerifyPassword(password, storedHash);
                        }
                    }
                }
            }

            return false;
        }

        // Хешування пароля
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public int GetUserId(string email)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT UserID FROM User WHERE Email = @Email";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Email", email);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return Convert.ToInt32(reader["UserID"]);
                        }
                        else
                        {
                            throw new Exception("Користувач не знайдений.");
                        }
                    }
                }
            }
        }


        // Перевірка хешу пароля
        private bool VerifyPassword(string password, string storedHash)
        {
            string passwordHash = HashPassword(password);
            return passwordHash == storedHash;
        }
    }
}
