using BusinessLogic;
using System.Windows;

namespace FarmApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataManager dataManager = new DataManager();

            // Заповнення бази даних тестовими даними
            dataManager.PopulateDatabase();

            // Отримуємо дані з таблиць
            var userData = dataManager.ShowTableData("User");
            var cropData = dataManager.ShowTableData("Crop");
            var fertilizerData = dataManager.ShowTableData("Fertilizer");
            var eventData = dataManager.ShowTableData("Event");

            // Виводимо дані у TextBox (OutputTextBox)
            OutputTextBox.Text = $"User Data:\n{userData}\n\n" +
                                 $"Crop Data:\n{cropData}\n\n" +
                                 $"Fertilizer Data:\n{fertilizerData}\n\n" +
                                 $"Event Data:\n{eventData}";
        }
    }



}
