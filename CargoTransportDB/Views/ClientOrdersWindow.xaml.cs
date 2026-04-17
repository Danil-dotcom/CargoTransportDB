using System;
using System.Linq;
using System.Windows;
using System.IO;
using CargoTransportation.Models;
using System.Windows.Controls;

namespace CargoTransportation.Views
{
    public partial class ClientOrdersWindow : Window
    {
        private User _currentUser;
        private Client _currentClient;
        private Order _selectedOrder;

        public ClientOrdersWindow(User user)
        {
            InitializeComponent();
            _currentUser = user;

            WelcomeText.Text = "Добро пожаловать, " + user.Username;

            LoadClientData();
            LoadClientOrders();
        }

        private void LoadClientData()
        {
            using (var context = new Data.CargoDbContext())
            {
                _currentClient = context.Clients.FirstOrDefault(c => c.UserID == _currentUser.UserID);
                if (_currentClient == null)
                {
                    _currentClient = new Client
                    {
                        UserID = _currentUser.UserID,
                        CompanyName = _currentUser.Username,
                        ContactPerson = _currentUser.Username,
                        ContactPhone = _currentUser.Phone
                    };
                    context.Clients.Add(_currentClient);
                    context.SaveChanges();
                }
            }
        }

        private void LoadClientOrders()
        {
            using (var context = new Data.CargoDbContext())
            {
                var orders = context.Orders
                    .Include("Cargo")
                    .Include("Status")
                    .Include("Vehicle")
                    .Include("Client")
                    .Include("Driver")
                    .Where(x => x.ClientID == _currentClient.ClientID)
                    .ToList();

                OrdersDataGrid.ItemsSource = orders;

                if (orders.Count == 0)
                {
                    NoOrdersText.Visibility = Visibility.Visible;
                    OrdersDataGrid.Visibility = Visibility.Collapsed;
                }
                else
                {
                    NoOrdersText.Visibility = Visibility.Collapsed;
                    OrdersDataGrid.Visibility = Visibility.Visible;
                }
            }
        }

        private void NewOrderButton_Click(object sender, RoutedEventArgs e)
        {
            var catalogWindow = new TruckCatalogWindow(_currentUser);
            catalogWindow.Show();
            this.Close();
        }

        private void ViewOrderDetails_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null && btn.Tag != null)
            {
                int orderId = (int)btn.Tag;

                using (var context = new Data.CargoDbContext())
                {
                    var fullOrder = context.Orders
                        .Include("Cargo")
                        .Include("Status")
                        .Include("Vehicle")
                        .Include("Client")
                        .Include("Driver")
                        .FirstOrDefault(x => x.OrderID == orderId);

                    if (fullOrder != null)
                    {
                        string msg = "═══════════════════════════════════\n";
                        msg += "         ДЕТАЛИ ЗАКАЗА\n";
                        msg += "═══════════════════════════════════\n\n";
                        msg += $"Номер заказа: {fullOrder.OrderNumber}\n";
                        msg += $"Дата создания: {fullOrder.OrderDate:dd.MM.yyyy HH:mm}\n";
                        msg += $"Статус: {fullOrder.Status?.StatusName ?? "Новый"}\n\n";
                        msg += "МАРШРУТ:\n";
                        msg += $"  Откуда: {fullOrder.PickupAddress}\n";
                        msg += $"  Куда: {fullOrder.DeliveryAddress}\n";
                        msg += $"  Расстояние: {fullOrder.Distance} км\n\n";
                        msg += "ФИНАНСЫ:\n";
                        msg += $"  Стоимость: {fullOrder.Price:C}\n\n";
                        msg += "ГРУЗ:\n";
                        msg += $"  Наименование: {fullOrder.Cargo?.Name ?? "Не указано"}\n";
                        msg += $"  Вес: {fullOrder.Cargo?.Weight ?? 0} кг\n\n";
                        msg += "ТРАНСПОРТ:\n";
                        msg += $"  Госномер: {fullOrder.Vehicle?.PlateNumber ?? "Не назначен"}\n";
                        msg += $"  Марка: {fullOrder.Vehicle?.Brand ?? ""} {fullOrder.Vehicle?.Model ?? ""}\n\n";
                        msg += "ВОДИТЕЛЬ:\n";
                        msg += $"  ФИО: {fullOrder.Driver?.FullName ?? "Не назначен"}\n";
                        msg += $"  Телефон: {fullOrder.Driver?.Phone ?? "Не указан"}\n\n";
                        msg += "КЛИЕНТ:\n";
                        msg += $"  Компания: {fullOrder.Client?.CompanyName ?? "Не указано"}\n";
                        msg += $"  Контакт: {fullOrder.Client?.ContactPerson ?? "Не указано"}";

                        MessageBox.Show(msg, "Детали заказа", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
        }

        // ГЕНЕРАЦИЯ PDF С QR-КОДОМ
        private void DownloadPDF_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null && btn.Tag != null)
            {
                int orderId = (int)btn.Tag;

                using (var context = new Data.CargoDbContext())
                {
                    var order = context.Orders
                        .Include("Client")
                        .Include("Cargo")
                        .Include("Vehicle")
                        .Include("Status")
                        .Include("Driver")
                        .FirstOrDefault(x => x.OrderID == orderId);

                    if (order != null)
                    {
                        try
                        {
                            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                            string fileName = $"Заказ_{order.OrderNumber}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                            string filePath = Path.Combine(desktopPath, fileName);

                            // Генерируем PDF с QR-кодом
                            Helpers.PDFHelper.GenerateOrderPDF(order, filePath);

                            MessageBox.Show($"✅ PDF успешно создан!\n\n{filePath}", "Успех",
                                           MessageBoxButton.OK, MessageBoxImage.Information);

                            // Открываем PDF
                            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(filePath) { UseShellExecute = true });
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Ошибка создания PDF: {ex.Message}", "Ошибка",
                                           MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
        }

        private void DeleteOrder_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null && btn.Tag != null)
            {
                int orderId = (int)btn.Tag;

                var result = MessageBox.Show("Удалить заказ?\n\nЭто действие нельзя отменить!",
                                            "Подтверждение удаления",
                                            MessageBoxButton.YesNo,
                                            MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        using (var context = new Data.CargoDbContext())
                        {
                            var order = context.Orders.FirstOrDefault(x => x.OrderID == orderId);
                            if (order != null)
                            {
                                context.Orders.Remove(order);
                                context.SaveChanges();
                                MessageBox.Show("Заказ удален!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                                LoadClientOrders();
                            }
                            else
                            {
                                MessageBox.Show("Заказ не найден!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void OrdersDataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            _selectedOrder = OrdersDataGrid.SelectedItem as Order;
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Выйти?", "Выход", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                new LoginWindow().Show();
                this.Close();
            }
        }
    }
}