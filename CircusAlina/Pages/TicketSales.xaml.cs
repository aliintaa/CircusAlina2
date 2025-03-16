using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using CircusAlina.Models;

namespace CircusAlina.Pages
{
    public partial class TicketSales : Page
    {
        public TicketSales()
        {
            InitializeComponent();
            LoadShows();
            LoadTicketSales();
        }

        private void LoadShows()
        {
            try
            {
                var shows = App.db.Shows.Select(s => new { s.ShowID, s.ShowName }).ToList();
                ShowComboBox.ItemsSource = shows;
                ShowComboBox.DisplayMemberPath = "ShowName";
                ShowComboBox.SelectedValuePath = "ShowID";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки шоу: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadTicketSales()
        {
            try
            {
                var ticketSales = App.db.Sales
                    .Select(s => new
                    {
                        s.SaleID,
                        ShowName = s.Tickets.Shows.ShowName,
                        s.Tickets.SeatNumber,
                        s.Tickets.Price,
                        s.SaleDate,
                        s.CustomerName,
                        s.PaymentMethod
                    })
                    .ToList()
                    .Select(s => new
                    {
                        s.SaleID,
                        s.ShowName,
                        s.SeatNumber,
                        s.Price,
                        SaleDate = s.SaleDate.ToString("dd.MM.yyyy HH:mm"),
                        s.CustomerName,
                        s.PaymentMethod
                    })
                    .ToList();

                SalesDataGrid.ItemsSource = ticketSales;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadAvailableTickets()
        {
            try
            {
                if (ShowComboBox.SelectedValue == null)
                {
                    MessageBox.Show("ShowComboBox.SelectedValue == null"); // Отладочный вывод
                    return;
                }

                int selectedShowId = (int)ShowComboBox.SelectedValue;
                var availableTickets = App.db.Tickets
                    .Where(t => t.Status == "Available" && t.ShowID == selectedShowId)
                    .Select(t => new
                    {
                        t.TicketID,
                        ShowName = t.Shows.ShowName,
                        t.SeatNumber,
                        t.Price
                    })
                    .ToList();

                MessageBox.Show($"Доступных билетов: {availableTickets.Count}"); // Отладочный вывод

                AvailableTicketsDataGrid.ItemsSource = availableTickets;
                TicketComboBox.ItemsSource = availableTickets;
                TicketComboBox.DisplayMemberPath = "SeatNumber";
                TicketComboBox.SelectedValuePath = "TicketID";

                if (availableTickets.Any())
                    TicketComboBox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки доступных билетов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void SellTicketButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ShowComboBox.SelectedValue == null || TicketComboBox.SelectedValue == null)
                {
                    MessageBox.Show("Выберите шоу и билет.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                int ticketId = (int)TicketComboBox.SelectedValue;
                string customerName = CustomerNameTextBox.Text.Trim();
                string paymentMethod = (PaymentMethodComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

                if (string.IsNullOrWhiteSpace(customerName) || string.IsNullOrWhiteSpace(paymentMethod))
                {
                    MessageBox.Show("Введите имя покупателя и выберите способ оплаты.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var ticket = App.db.Tickets.FirstOrDefault(t => t.TicketID == ticketId);
                if (ticket == null)
                {
                    MessageBox.Show("Выбранный билет не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var newSale = new Sales
                {
                    TicketID = ticket.TicketID,
                    SaleDate = DateTime.Now,
                    PaymentMethod = paymentMethod,
                    CustomerName = customerName
                };

                ticket.Status = "Sold";
                App.db.Sales.Add(newSale);
                App.db.SaveChanges();

                LoadTicketSales();
                LoadAvailableTickets();
                MessageBox.Show("Билет успешно продан!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при продаже билета: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ShowComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadAvailableTickets();
        }
    }
}