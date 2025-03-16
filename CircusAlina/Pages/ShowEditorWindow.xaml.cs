using System;
using System.Windows;
using CircusAlina.Models;

namespace CircusAlina.Pages
{
    public partial class ShowEditorWindow : Window
    {
        public Shows ShowDetails { get; private set; }

        public ShowEditorWindow(string title, Shows existingShow = null)
        {
            InitializeComponent();
            Title = title;

            if (existingShow != null)
            {
                ShowDetails = new Shows
                {
                    ShowID = existingShow.ShowID,
                    ShowName = existingShow.ShowName,
                    Date = existingShow.Date,
                    Time = existingShow.Time,
                    Location = existingShow.Location
                };

                TitleTextBox.Text = ShowDetails.ShowName;
                DateTextBox.Text = ShowDetails.Date.ToString("yyyy-MM-dd");
                TimeTextBox.Text = ShowDetails.Time.ToString(@"hh\:mm");
                LocationTextBox.Text = ShowDetails.Location;
            }
            else
            {
                ShowDetails = new Shows();
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TitleTextBox.Text) ||
                string.IsNullOrWhiteSpace(LocationTextBox.Text) ||
                !DateTime.TryParse(DateTextBox.Text, out DateTime date) ||
                !TimeSpan.TryParse(TimeTextBox.Text, out TimeSpan time))
            {
                MessageBox.Show("Заполните все поля корректно.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            ShowDetails.ShowName = TitleTextBox.Text;
            ShowDetails.Date = date;
            ShowDetails.Time = time;
            ShowDetails.Location = LocationTextBox.Text;

            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
