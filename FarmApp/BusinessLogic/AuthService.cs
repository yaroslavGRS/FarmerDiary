using MySql.Data.MySqlClient;
using System;
using System.Security.Cryptography;
using System.Text;

namespace BusinessLogic
{
    public class AuthService
    {
        private string connectionString;

        public AuthService(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public bool Register(string email, string password)
        {
            string hashedPassword = HashPassword(password);
            string query = "INSERT INTO user (Email, PasswordHash) VALUES (@Email, @PasswordHash)";

            try
            {
                using (var connection = new MySql.Data.MySqlClient.MySqlConnection(connectionString))
                {
                    connection.Open();
                    using (var cmd = new MySql.Data.MySqlClient.MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@PasswordHash", hashedPassword);
                        cmd.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (MySql.Data.MySqlClient.MySqlException ex) when (ex.Number == 1062) // Duplicate entry
            {
                return false; // Користувач із таким email уже існує
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }

        public bool Login(string email, string password)
        {
            string hashedPassword = HashPassword(password);
            string query = "SELECT COUNT(*) FROM user WHERE Email = @Email AND PasswordHash = @PasswordHash";

            try
            {
                using (var connection = new MySql.Data.MySqlClient.MySqlConnection(connectionString))
                {
                    connection.Open();
                    using (var cmd = new MySql.Data.MySqlClient.MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@PasswordHash", hashedPassword);

                        int userCount = Convert.ToInt32(cmd.ExecuteScalar());
                        return userCount > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
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
                    }
                }
            }

            throw new Exception("Користувач не знайдений.");
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
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
    }
}
