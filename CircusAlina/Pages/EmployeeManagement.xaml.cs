using CircusAlina.Models;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace CircusAlina.Pages
{
    public partial class EmployeeManagement : Page
    {
        public EmployeeManagement()
        {
            InitializeComponent();
            LoadEmployees();
        }

        private void LoadEmployees()
        {
            EmployeeDataGrid.ItemsSource = App.db.Employees.ToList();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            // Переход на страницу добавления сотрудника
            NavigationService?.Navigate(new AddEditEmployee(new Employees()));
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (EmployeeDataGrid.SelectedItem is Employees selectedEmployee)
            {
                // Переход на страницу редактирования сотрудника
                NavigationService?.Navigate(new AddEditEmployee(selectedEmployee));
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите сотрудника для редактирования.");
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (EmployeeDataGrid.SelectedItem is Employees selectedEmployee)
            {
                var result = MessageBox.Show("Вы уверены, что хотите удалить этого сотрудника?", "Подтверждение", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    App.db.Employees.Remove(selectedEmployee);
                    App.db.SaveChanges();
                    LoadEmployees();
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите сотрудника для удаления.");
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            EmployeeDataGrid.ItemsSource = App.db.Employees.ToList();


        }
    }
}
