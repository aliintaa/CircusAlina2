using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using CircusAlina.Models;

namespace CircusAlina.Pages
{
    public partial class RepertoireManagement : Page
    {
        public RepertoireManagement()
        {
            InitializeComponent();
            LoadRepertoire();
        }

        private void LoadRepertoire()
        {
            try
            {
                if (App.db == null)
                {
                    MessageBox.Show("Ошибка: база данных недоступна.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                RepertoireDataGrid.ItemsSource = App.db.Shows.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки репертуара: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddShowButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var editor = new ShowEditorWindow("Добавление нового шоу");
                if (editor.ShowDialog() == true)
                {
                    App.db.Shows.Add(editor.ShowDetails);
                    App.db.SaveChanges();
                    LoadRepertoire();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении шоу: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditShowButton_Click(object sender, RoutedEventArgs e)
        {
            if (RepertoireDataGrid.SelectedItem is Shows selectedShow)
            {
                try
                {
                    var editor = new ShowEditorWindow("Редактирование шоу", selectedShow);
                    if (editor.ShowDialog() == true)
                    {
                        App.db.Entry(selectedShow).CurrentValues.SetValues(editor.ShowDetails);
                        App.db.SaveChanges();
                        LoadRepertoire();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при редактировании шоу: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Выберите шоу для редактирования.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeleteShowButton_Click(object sender, RoutedEventArgs e)
        {
            if (RepertoireDataGrid.SelectedItem is Shows selectedShow)
            {
                var result = MessageBox.Show($"Вы уверены, что хотите удалить шоу '{selectedShow.ShowName}'?",
                                             "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        App.db.Shows.Remove(selectedShow);
                        App.db.SaveChanges();
                        LoadRepertoire();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении шоу: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите шоу для удаления.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
