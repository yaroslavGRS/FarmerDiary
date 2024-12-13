using System;
using System.Windows;
using BusinessLogic;

namespace PresentationLayer
{
    public partial class MainWindow : Window
    {
        private readonly AuthService authService;
        private readonly DataManager dataManager;
        private bool isLoggedIn;

        public MainWindow()
        {
            InitializeComponent();
            string connectionString = "Server=localhost;Database=farmerdiary;User=root;Password=yarik1311;";
            authService = new AuthService(connectionString);
            dataManager = new DataManager(connectionString);

            // Початковий стан
            isLoggedIn = false;
            UpdateUI();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailTextBox.Text;
            string password = PasswordTextBox.Password;

            if (authService.Login(email, password))
            {
                MessageBox.Show("Вхід успішний!");
                isLoggedIn = true;

                // Показати email залогіненого користувача
                LoggedInEmail.Text = email;

                UpdateUI();
            }
            else
            {
                MessageBox.Show("Невірний логін або пароль.");
            }
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailTextBox.Text;
            string password = PasswordTextBox.Password;

            if (authService.Register(email, password))
            {
                MessageBox.Show("Реєстрація успішна!");
            }
            else
            {
                MessageBox.Show("Помилка реєстрації. Можливо, користувач із таким email вже існує.");
            }
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            isLoggedIn = false;
            LoggedInEmail.Text = string.Empty;
            MessageBox.Show("Ви успішно вийшли з акаунту.");
            UpdateUI();
        }
        private void ShowData_Click(object sender, RoutedEventArgs e)
        {
            var cropData = dataManager.GetTableData("Crop");

            string result = "Дані з таблиці Crop:\n";
            foreach (var row in cropData)
            {
                foreach (var column in row)
                {
                    result += $"{column.Key}: {column.Value}\t";
                }
                result += "\n";
            }

            MessageBox.Show(result, "Таблиця Crop");

        }


        private void CalculateGrainAndFertilizer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (isLoggedIn)
                {
                    if (double.TryParse(AreaTextBox.Text, out double area) &&
                        double.TryParse(ThousandSeedWeightTextBox.Text, out double thousandSeedWeight) &&
                        double.TryParse(DesiredDensityTextBox.Text, out double desiredDensity))
                    {
                        // Розрахунок зерна
                        double grainAmount = dataManager.CalculateGrain(desiredDensity, thousandSeedWeight, area);

                        // Розрахунок добрив (згідно вашої формули)
                        double yieldGoal = 6; // т/га
                        double nitrogenContentInSoil = 20; // кг/га
                        double nitrogenUtilizationFactor = 0.6;
                        double fertilizerAmount = dataManager.CalculateNitrogenFertilizer(area, yieldGoal, nitrogenContentInSoil, nitrogenUtilizationFactor);

                        // Запис результатів у базу
                        string cropName = "Пшениця";
                        int userId = authService.GetUserId(LoggedInEmail.Text);
                        dataManager.InsertCropRecord(cropName, area, grainAmount, userId);
                        dataManager.InsertFertilizerRecord(cropName, area, fertilizerAmount, userId);

                        MessageBox.Show($"Кількість зерна: {grainAmount} кг.\nКількість добрив: {fertilizerAmount} кг.\nРезультати записано в базу.", "Розрахунок зерна та добрив");
                    }
                    else
                    {
                        MessageBox.Show("Будь ласка, введіть коректні числові значення.", "Помилка введення");
                    }
                }
                else
                {
                    MessageBox.Show("Будь ласка, увійдіть у систему, щоб виконати розрахунок.", "Помилка");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка: {ex.Message}", "Помилка");
            }
        }

        private void UpdateUI()
        {
            if (isLoggedIn)
            {
                LoginContent.Visibility = Visibility.Collapsed;
                LogoutContent.Visibility = Visibility.Visible;
            }
            else
            {
                LoginContent.Visibility = Visibility.Visible;
                LogoutContent.Visibility = Visibility.Collapsed;
            }
        }
    }
}
