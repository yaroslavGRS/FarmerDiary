using System;
using System.Text;
using MySql.Data.MySqlClient;
using DataAccessLayer;

namespace BusinessLogic
{
    public class DataManager
    {
        private DatabaseService databaseService;

        public DataManager()
        {
            databaseService = new DatabaseService();
        }

        // Метод для виведення даних з таблиці
        public string ShowTableData(string tableName)
        {
            StringBuilder result = new StringBuilder();

            using (var connection = databaseService.GetConnection())
            {
                connection.Open();
                string query = $"SELECT * FROM {tableName}";
                using (var cmd = new MySqlCommand(query, connection))
                using (var reader = cmd.ExecuteReader())
                {
                    // Додаємо заголовок з назвами колонок
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        result.Append(reader.GetName(i) + "\t");
                    }
                    result.AppendLine();

                    // Додаємо рядки з даними
                    while (reader.Read())
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            result.Append(reader.GetValue(i) + "\t");
                        }
                        result.AppendLine();
                    }
                }
            }

            return result.ToString();
        }

        // Метод для заповнення таблиць випадковими даними
        public void PopulateDatabase()
        {
            using (var connection = databaseService.GetConnection())
            {
                connection.Open();

                Random random = new Random();

                // Заповнення таблиці User
                for (int i = 0; i < 30; i++)
                {
                    string email = $"user{i}@example.com";
                    string passwordHash = Guid.NewGuid().ToString();
                    string query = $"INSERT INTO User (Email, PasswordHash) VALUES ('{email}', '{passwordHash}')";
                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }

                // Заповнення таблиці Crop
                for (int i = 0; i < 30; i++)
                {
                    string name = $"Crop_{i}";
                    float area = (float)(random.NextDouble() * 100);
                    float quantity = (float)(random.NextDouble() * 1000);
                    int userId = random.Next(1, 31);
                    string query = $"INSERT INTO Crop (Name, Area, Quantity, UserID) VALUES ('{name}', {area}, {quantity}, {userId})";
                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }

                // Заповнення таблиці Fertilizer
                for (int i = 0; i < 30; i++)
                {
                    string type = $"Type_{i}";
                    float amount = (float)(random.NextDouble() * 50);
                    int cropId = random.Next(1, 31);
                    string query = $"INSERT INTO Fertilizer (Type, Amount, CropID) VALUES ('{type}', {amount}, {cropId})";
                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }

                // Заповнення таблиці Event
                for (int i = 0; i < 30; i++)
                {
                    string eventName = $"Event_{i}";
                    DateTime eventDate = DateTime.Now.AddDays(-random.Next(0, 365));
                    int cropId = random.Next(1, 31);
                    string query = $"INSERT INTO Event (EventName, EventDate, CropID) VALUES ('{eventName}', '{eventDate:yyyy-MM-dd}', {cropId})";
                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }

                Console.WriteLine("База даних успішно заповнена випадковими даними!");
            }
        }
    }
}
