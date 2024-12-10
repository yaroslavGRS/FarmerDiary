using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace BusinessLogic
{
    public class DataManager
    {
        private readonly string connectionString;

        public DataManager(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public List<Dictionary<string, object>> GetTableData(string tableName)
        {
            var result = new List<Dictionary<string, object>>();

            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = $"SELECT * FROM {tableName}";

                using (var cmd = new MySqlCommand(query, connection))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var row = new Dictionary<string, object>();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            row[reader.GetName(i)] = reader.GetValue(i);
                        }
                        result.Add(row);
                    }
                }
            }

            return result;
        }

        public double CalculateGrain(double desiredDensity, double thousandSeedWeight, double area)
        {
            double seedRatePerHa = (desiredDensity * thousandSeedWeight) / 1000;
            double grainAmount = seedRatePerHa * area / 10000;

            return Math.Round(grainAmount, 2);
        }

        public double CalculateNitrogenFertilizer(double area, double yieldGoal, double nitrogenContentInSoil, double nitrogenUtilizationFactor)
        {
            // Формула розрахунку азотного добрива
            double nitrogenNeededPerHa = yieldGoal * 30 - nitrogenContentInSoil; // 30 - коефіцієнт витрати
            if (nitrogenNeededPerHa < 0)
            {
                nitrogenNeededPerHa = 0; // Якщо азоту достатньо в ґрунті
            }

            double fertilizerAmountPerHa = nitrogenNeededPerHa / nitrogenUtilizationFactor;
            double fertilizerAmount = fertilizerAmountPerHa * area / 10000;

            return Math.Round(fertilizerAmount, 2);
        }

        public void InsertCropRecord(string cropName, double area, double quantity, int userId)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "INSERT INTO Crop (Name, Area, Quantity, UserID) VALUES (@Name, @Area, @Quantity, @UserID)";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Name", cropName);
                    cmd.Parameters.AddWithValue("@Area", area);
                    cmd.Parameters.AddWithValue("@Quantity", quantity);
                    cmd.Parameters.AddWithValue("@UserID", userId);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void InsertFertilizerRecord(string cropName, double area, double quantity, int userId)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // Отримання CropID із таблиці Crop для зв'язку
                string cropQuery = "SELECT CropID FROM Crop WHERE Name = @Name AND UserID = @UserID ORDER BY CropID DESC LIMIT 1";
                int cropId = 0;

                using (var cropCmd = new MySqlCommand(cropQuery, connection))
                {
                    cropCmd.Parameters.AddWithValue("@Name", cropName);
                    cropCmd.Parameters.AddWithValue("@UserID", userId);

                    var result = cropCmd.ExecuteScalar();
                    if (result != null)
                    {
                        cropId = Convert.ToInt32(result);
                    }
                    else
                    {
                        throw new Exception("CropID не знайдено для заданого користувача.");
                    }
                }

                // Вставка у таблицю Fertilizer
                string fertilizerQuery = "INSERT INTO Fertilizer (CropID, Type, Amount) VALUES (@CropID, @Type, @Amount)";
                using (var cmd = new MySqlCommand(fertilizerQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@CropID", cropId);
                    cmd.Parameters.AddWithValue("@Type", "Азотне добриво");
                    cmd.Parameters.AddWithValue("@Amount", quantity);

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
