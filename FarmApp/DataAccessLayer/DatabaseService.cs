using System;
using MySql.Data.MySqlClient;

namespace DataAccessLayer
{
    public class DatabaseService
    {
        private string connectionString;

        public DatabaseService()
        {
            
            connectionString = "Server=localhost;Database=farmerdiary;User=root;Password=yarik1311;";
        }

        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(connectionString);
        }
    }
}
