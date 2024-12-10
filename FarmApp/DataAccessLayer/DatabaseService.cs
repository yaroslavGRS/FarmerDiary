using System;
using MySql.Data.MySqlClient;

namespace DataAccessLayer
{
    public class DatabaseService
    {
        private string connectionString;

        public DatabaseService()
        {
            // Замініть на свій рядок підключення
            connectionString = "Server=localhost;Database=FarmerDiary;User=root;Password=yarik1311;";
        }

        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(connectionString);
        }
    }
}
