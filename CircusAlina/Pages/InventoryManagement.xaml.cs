using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using CircusAlina.Models;

namespace CircusAlina.Pages
{
    public partial class InventoryManagement : Page
    {
        public InventoryManagement()
        {
            InitializeComponent();
            LoadInventory();
        }

        private void LoadInventory()
        {
            InventoryDataGrid.ItemsSource = App.db.Inventory.ToList();
        }

        private void AddInventoryButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string name = NameTextBox.Text;
                string quantityText = QuantityTextBox.Text;
                DateTime? lastUpdated = LastUpdatedDatePicker.SelectedDate;

                if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(quantityText) || lastUpdated == null)
                {
                    MessageBox.Show("Заполните все поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!int.TryParse(quantityText, out int quantity) || quantity < 0)
                {
                    MessageBox.Show("Количество должно быть положительным числом.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var newInventoryItem = new Inventory
                {
                    ItemName = name,
                    Quantity = quantity,
                    LastUpdated = lastUpdated.Value
                };

                App.db.Inventory.Add(newInventoryItem);
                App.db.SaveChanges();
                LoadInventory();

                // Очистка полей
                NameTextBox.Clear();
                QuantityTextBox.Clear();
                LastUpdatedDatePicker.SelectedDate = DateTime.Now;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteInventoryButton_Click(object sender, RoutedEventArgs e)
        {
            if (InventoryDataGrid.SelectedItem is Inventory selectedItem)
            {
                if (MessageBox.Show($"Удалить '{selectedItem.ItemName}'?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    App.db.Inventory.Remove(selectedItem);
                    App.db.SaveChanges();
                    LoadInventory();
                }
            }
            else
            {
                MessageBox.Show("Выберите элемент.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
