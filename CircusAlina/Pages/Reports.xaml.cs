using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using CircusAlina.Models;       // ваш namespace с моделями
using LiveCharts;              // для SeriesCollection, ChartValues и т.п.
using LiveCharts.Wpf;          // для ColumnSeries, LineSeries и т.д.
using ClosedXML.Excel;         // для работы с Excel
using Microsoft.Win32;         // для SaveFileDialog
using System.Collections.Generic; // возможно понадобится для List<T> и т.п.

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
                // Сбрасываем предыдущий график
                ReportChart.Series = new SeriesCollection();
                ReportChart.AxisX.Clear();
                ReportChart.AxisY.Clear();

                switch (reportType)
                {
                    case "Отчет по продажам билетов":
                        var ticketSales = App.db.Tickets
                            .Select(t => new
                            {
                                t.TicketID,
                                ShowName = t.Shows.ShowName,
                                t.Price
                            })
                            .ToList();

                        ReportsDataGrid.ItemsSource = ticketSales;

                        var salesByShow = App.db.Tickets
                            .GroupBy(t => t.Shows.ShowName)
                            .Select(g => new
                            {
                                Show = g.Key,
                                SumPrice = g.Sum(x => x.Price)
                            })
                            .ToList();

                        // Столбчатая диаграмма
                        ReportChart.Series.Add(new ColumnSeries
                        {
                            Title = "Сумма продаж",
                            Values = new ChartValues<decimal>(salesByShow.Select(x => x.SumPrice))
                        });

                        // Ось X – названия шоу
                        ReportChart.AxisX.Add(new Axis
                        {
                            Title = "Шоу",
                            Labels = salesByShow.Select(x => x.Show).ToArray()
                        });

                        // Ось Y – сумма продаж
                        ReportChart.AxisY.Add(new Axis
                        {
                            Title = "Сумма (руб.)"
                        });

                        break;

                    case "Отчет по репертуару":
                        var repertoire = App.db.Shows
                            .Select(s => new
                            {
                                s.ShowID,
                                s.ShowName,
                                s.Date,
                                s.Time,
                                s.Location
                            })
                            .ToList();

                        ReportsDataGrid.ItemsSource = repertoire;

                        // График: количество шоу по месяцам (пример)
                        var showsByMonth = App.db.Shows
                            .GroupBy(s => s.Date.Month)
                            .Select(g => new
                            {
                                Month = g.Key,
                                Count = g.Count()
                            })
                            .OrderBy(x => x.Month)
                            .ToList();

                        ReportChart.Series.Add(new ColumnSeries
                        {
                            Title = "Кол-во шоу",
                            Values = new ChartValues<int>(showsByMonth.Select(x => x.Count))
                        });

                        // Ось X – номер месяца
                        ReportChart.AxisX.Add(new Axis
                        {
                            Title = "Месяц",
                            Labels = showsByMonth.Select(x => x.Month.ToString()).ToArray()
                        });

                        // Ось Y – количество шоу
                        ReportChart.AxisY.Add(new Axis
                        {
                            Title = "Шоу (шт.)"
                        });

                        break;

                    case "Отчет по доходам":
                        var incomeReport = App.db.Tickets
                            .GroupBy(t => t.Shows.ShowName)
                            .Select(g => new
                            {
                                Шоу = g.Key,
                                Доход = g.Sum(t => t.Price)
                            })
                            .ToList();

                        ReportsDataGrid.ItemsSource = incomeReport;

                        // График: доход по каждому шоу (столбцы)
                        ReportChart.Series.Add(new ColumnSeries
                        {
                            Title = "Доход",
                            Values = new ChartValues<decimal>(incomeReport.Select(x => x.Доход))
                        });

                        // Ось X – названия шоу
                        ReportChart.AxisX.Add(new Axis
                        {
                            Title = "Шоу",
                            Labels = incomeReport.Select(x => x.Шоу).ToArray()
                        });

                        // Ось Y – доход
                        ReportChart.AxisY.Add(new Axis
                        {
                            Title = "Доход (руб.)"
                        });

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

        private void DownloadReportButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ReportsDataGrid.ItemsSource == null)
                {
                    MessageBox.Show("Нет данных для экспорта. Сначала сгенерируйте отчет.",
                        "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // Диалог сохранения файла
                var saveFileDialog = new SaveFileDialog
                {
                    Filter = "Excel Files|*.xlsx",
                    Title = "Сохранить отчет в Excel"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    using (var workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add("Отчет");

                        // Переводим ItemsSource в список объектов
                        var data = ReportsDataGrid.ItemsSource.Cast<object>().ToList();
                        if (data.Count == 0)
                        {
                            MessageBox.Show("Нет данных для экспорта.",
                                "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                            return;
                        }

                        // Получаем свойства (колонки) через reflection
                        var props = data[0].GetType().GetProperties();

                        // Записываем заголовки
                        for (int i = 0; i < props.Length; i++)
                        {
                            worksheet.Cell(1, i + 1).Value = props[i].Name;
                        }

                        // Записываем строки
                        for (int row = 0; row < data.Count; row++)
                        {
                            for (int col = 0; col < props.Length; col++)
                            {
                                var rawValue = props[col].GetValue(data[row], null);

                                if (rawValue == null)
                                {
                                    worksheet.Cell(row + 2, col + 1).SetValue(string.Empty);
                                }
                                else
                                {
                                    // Проверяем тип, чтобы Excel правильно сохранил даты/числа
                                    switch (rawValue)
                                    {
                                        case int intVal:
                                            worksheet.Cell(row + 2, col + 1).SetValue(intVal);
                                            break;
                                        case decimal decVal:
                                            worksheet.Cell(row + 2, col + 1).SetValue(decVal);
                                            break;
                                        case double doubleVal:
                                            worksheet.Cell(row + 2, col + 1).SetValue(doubleVal);
                                            break;
                                        case DateTime dateVal:
                                            worksheet.Cell(row + 2, col + 1).SetValue(dateVal);
                                            break;
                                        default:
                                            worksheet.Cell(row + 2, col + 1).SetValue(rawValue.ToString());
                                            break;
                                    }
                                }
                            }
                        }

                        // Сохраняем Excel (после цикла)
                        workbook.SaveAs(saveFileDialog.FileName);
                        MessageBox.Show("Отчет успешно сохранён!",
                            "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении отчета: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
