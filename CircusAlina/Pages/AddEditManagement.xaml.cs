using CircusAlina.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CircusAlina.Pages
{
    /// <summary>
    /// Логика взаимодействия для AddEditManagement.xaml
    /// </summary>
    public partial class AddEditManagement : Page
    {
        private Shows _currentShow;

        public AddEditManagement(Shows existingShow = null)
        {
            InitializeComponent();

            if (existingShow != null)
            {
                _currentShow = existingShow;
                ShowNameTextBox.Text = existingShow.ShowName;
                DatePickerShow.SelectedDate = existingShow.Date;
                TimeTextBox.Text = existingShow.Time.ToString(@"hh\:mm");
                LocationTextBox.Text = existingShow.Location;
            }
            else
            {
                _currentShow = new Shows();
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ShowNameTextBox.Text) ||
                DatePickerShow.SelectedDate == null ||
                string.IsNullOrWhiteSpace(TimeTextBox.Text) ||
                string.IsNullOrWhiteSpace(LocationTextBox.Text))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!TimeSpan.TryParse(TimeTextBox.Text, out TimeSpan parsedTime))
            {
                MessageBox.Show("Некорректный формат времени. Используйте ЧЧ:ММ.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _currentShow.ShowName = ShowNameTextBox.Text;
            _currentShow.Date = DatePickerShow.SelectedDate.Value;
            _currentShow.Time = parsedTime;
            _currentShow.Location = LocationTextBox.Text;

            if (_currentShow.ShowID == 0)
            {
                App.db.Shows.Add(_currentShow);
            }

            try
            {
                App.db.SaveChanges();
                MessageBox.Show("Данные успешно сохранены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
