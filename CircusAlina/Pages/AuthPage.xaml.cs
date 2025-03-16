using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace CircusAlina.Pages
{
    public partial class AuthPage : Page
    {
        private MainWindow _mainWindow;

        public AuthPage(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string login = UsernameTextBox.Text.Trim();
            string password = PasswordBox.Password;

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Введите логин и пароль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var employee = App.db.Employees.FirstOrDefault(emp => emp.Login == login);

            if (employee == null || employee.Passwords != password)
            {
                MessageBox.Show("Неверный логин или пароль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBox.Show($"Добро пожаловать, {employee.FirstName}!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

            // Показываем меню в MainWindow
            _mainWindow.ShowMenu();

            // Переходим в главную рабочую страницу
            _mainWindow.MainFrame.Navigate(new InventoryManagement());
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        private void GoToRegistrationButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RegistrationPage());
        }
    }
}
