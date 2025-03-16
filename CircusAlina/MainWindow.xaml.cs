using CircusAlina.Pages;
using System.Windows;
using System.Windows.Navigation;

namespace CircusAlina
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Скрываем меню при запуске
            MainMenu.Visibility = Visibility.Collapsed;

            // Загружаем страницу авторизации
            MainFrame.Navigate(new AuthPage(this));
        }

        // Метод для показа меню после успешного входа
        public void ShowMenu()
        {
            MainMenu.Visibility = Visibility.Visible;
        }

        private void EmployeeManagement_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new EmployeeManagement());
        }

        private void RepertoireManagement_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new RepertoireManagement());
        }

        private void TicketSales_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new TicketSales());
        }

        private void Reports_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Reports());
        }

        private void InventoryManagement_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new InventoryManagement());
        }
    }
}
