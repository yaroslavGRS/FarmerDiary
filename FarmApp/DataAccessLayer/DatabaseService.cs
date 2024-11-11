using MySql.Data.MySqlClient;

namespace DataAccessLayer
{
    public class DatabaseService
    {
        // Рядок підключення до бази даних MySQL
        private string connectionString = "Server=localhost;Database=FarmerDiary;User ID=root;Password=yarik1311;";

        // Метод, який повертає підключення до бази даних
        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(connectionString);
        }
    }
}
