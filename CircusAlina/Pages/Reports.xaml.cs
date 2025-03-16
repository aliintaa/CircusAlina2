using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using CircusAlina.Models;

namespace CircusAlina.Pages
{
    public partial class Reports : Page
    {
        public Reports()
        {
            InitializeComponent();
        }

        private void GenerateReportButton_Click(object sender, RoutedEventArgs e)
        {
            if (ReportTypeComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                string reportType = selectedItem.Content.ToString();
                GenerateReport(reportType);
            }
            else
            {
                MessageBox.Show("Выберите тип отчета", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void GenerateReport(string reportType)
        {
            try
            {
                switch (reportType)
                {
                    case "Отчет по продажам билетов":
                        ReportsDataGrid.ItemsSource = App.db.Tickets
                            .Select(t => new
                            {
                                t.TicketID,
                                t.Shows.ShowName,
                                t.Price
                            }).ToList();
                        break;

                    case "Отчет по репертуару":
                        ReportsDataGrid.ItemsSource = App.db.Shows
                            .Select(s => new
                            {
                                s.ShowID,
                                s.ShowName,
                                s.Date,
                                s.Time,
                                s.Location
                            }).ToList();
                        break;

                    case "Отчет по доходам":
                        ReportsDataGrid.ItemsSource = App.db.Tickets
                            .GroupBy(t => t.Shows.ShowName)
                            .Select(g => new
                            {
                                Шоу = g.Key,
                                Доход = g.Sum(t => t.Price)
                            }).ToList();
                        break;

                    default:
                        MessageBox.Show("Неизвестный тип отчета", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка генерации отчета: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
