using CircusAlina.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace CircusAlina.Pages
{
    public partial class AddEditEmployee : Page
    {
        Employees employees;
        public AddEditEmployee(Employees _employees)
        {
            InitializeComponent();

            employees = _employees ?? new Employees(); // Гарантируем, что employees не будет null
            DataContext = employees;

            FirstNameTextBox.Text = employees.FirstName;
            LastNameTextBox.Text = employees.LastName;
            RoleTextBox.Text = employees.Role;
            HireDatePicker.SelectedDate = employees.HireDate != DateTime.MinValue ? employees.HireDate : DateTime.Today;
            SalaryTextBox.Text = employees.Salary.ToString();
            LoginTextBox.Text = employees.Login;
            PasswordBox.Password = employees.Passwords;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FirstNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(LastNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(RoleTextBox.Text) ||
                string.IsNullOrWhiteSpace(LoginTextBox.Text) ||
                string.IsNullOrWhiteSpace(PasswordBox.Password) ||
                !decimal.TryParse(SalaryTextBox.Text, out decimal salary) ||
                HireDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Пожалуйста, заполните все поля корректно.");
                return;
            }

            var existingEmployee = App.db.Employees.FirstOrDefault(emp => emp.Login == LoginTextBox.Text);

            if (existingEmployee != null && existingEmployee.EmployeeID != employees.EmployeeID)
            {
                MessageBox.Show("Логин уже используется другим сотрудником.");
                return;
            }

            if (employees.EmployeeID == 0)
            {
                employees.FirstName = FirstNameTextBox.Text;
                employees.LastName = LastNameTextBox.Text;
                employees.Role = RoleTextBox.Text;
                employees.HireDate = HireDatePicker.SelectedDate.Value;
                employees.Salary = salary;
                employees.Login = LoginTextBox.Text;
                employees.Passwords = PasswordBox.Password;

                App.db.Employees.Add(employees);
            }
            else
            {
                var employeeToUpdate = App.db.Employees.Find(employees.EmployeeID);
                if (employeeToUpdate != null)
                {
                    employeeToUpdate.FirstName = FirstNameTextBox.Text;
                    employeeToUpdate.LastName = LastNameTextBox.Text;
                    employeeToUpdate.Role = RoleTextBox.Text;
                    employeeToUpdate.HireDate = HireDatePicker.SelectedDate.Value;
                    employeeToUpdate.Salary = salary;
                    employeeToUpdate.Login = LoginTextBox.Text;
                    employeeToUpdate.Passwords = PasswordBox.Password;
                }
            }

            App.db.SaveChanges();
            MessageBox.Show("Сотрудник сохранен.");
            NavigationService.GoBack();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}