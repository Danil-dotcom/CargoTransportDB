using System;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using System.IO;
using CargoTransportation.Services;
using CargoTransportation.Models;

namespace CargoTransportation.Views
{
    public partial class MainWindow : Window
    {
        private OrderService _orderService;
        private User _currentUser;

        public MainWindow(User user)
        {
            InitializeComponent();
            _orderService = new OrderService();
            _currentUser = user;

            WelcomeText.Text = $"👋 Добро пожаловать, {user.Username}!";
            LoadData();
        }

        private void LoadData()
        {
            LoadOrders();
            LoadComboBoxes();
            LoadStatusFilter();
        }

        private void LoadOrders()
        {
            try
            {
                var orders = _orderService.GetAllOrders();
                OrdersDataGrid.ItemsSource = orders;
                StatusText.Text = $"✅ Загружено заказов: {orders.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка",
                               MessageBoxButton.OK, MessageBoxImage.Error);
                StatusText.Text = "❌ Ошибка загрузки";
            }
        }

        private void LoadComboBoxes()
        {
            try
            {
                using (var context = new Data.CargoDbContext())
                {
                    ClientCombo.ItemsSource = context.Clients.ToList();
                    DriverCombo.ItemsSource = context.Drivers.Where(d => d.Status == "Available").ToList();
                    VehicleCombo.ItemsSource = context.Vehicles.Where(v => v.Status == "Available").ToList();
                    CargoCombo.ItemsSource = context.Cargos.ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка",
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadStatusFilter()
        {
            try
            {
                using (var context = new Data.CargoDbContext())
                {
                    StatusFilterCombo.ItemsSource = context.OrderStatuses.ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки статусов: {ex.Message}", "Ошибка",
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int? statusId = null;
                if (StatusFilterCombo.SelectedItem is OrderStatus status)
                    statusId = status.StatusID;

                var filtered = _orderService.FilterOrders(
                    FromDatePicker.SelectedDate,
                    ToDatePicker.SelectedDate,
                    statusId,
                    SearchTextBox.Text
                );
                OrdersDataGrid.ItemsSource = filtered;
                StatusText.Text = $"🔍 Найдено: {filtered.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ResetFilterButton_Click(object sender, RoutedEventArgs e)
        {
            FromDatePicker.SelectedDate = null;
            ToDatePicker.SelectedDate = null;
            StatusFilterCombo.SelectedItem = null;
            SearchTextBox.Text = "";
            LoadOrders();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ClientCombo.SelectedItem == null || DriverCombo.SelectedItem == null ||
                    VehicleCombo.SelectedItem == null || CargoCombo.SelectedItem == null ||
                    string.IsNullOrWhiteSpace(PickupAddressTextBox.Text) ||
                    string.IsNullOrWhiteSpace(DeliveryAddressTextBox.Text) ||
                    string.IsNullOrWhiteSpace(PriceTextBox.Text) ||
                    string.IsNullOrWhiteSpace(DistanceTextBox.Text))
                {
                    MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var order = new Order
                {
                    ClientID = ((Client)ClientCombo.SelectedItem).ClientID,
                    DriverID = ((Driver)DriverCombo.SelectedItem).DriverID,
                    VehicleID = ((Vehicle)VehicleCombo.SelectedItem).VehicleID,
                    CargoID = ((Cargo)CargoCombo.SelectedItem).CargoID,
                    PickupAddress = PickupAddressTextBox.Text,
                    DeliveryAddress = DeliveryAddressTextBox.Text,
                    Price = decimal.Parse(PriceTextBox.Text),
                    Distance = decimal.Parse(DistanceTextBox.Text)
                };

                if (_orderService.AddOrder(order))
                {
                    MessageBox.Show("Заказ добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadOrders();
                    ClearForm();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersDataGrid.SelectedItem is Order selected)
            {
                if (MessageBox.Show($"Удалить заказ {selected.OrderNumber}?", "Подтверждение",
                    MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    _orderService.DeleteOrder(selected.OrderID);
                    LoadOrders();
                    ClearForm();
                }
            }
        }

        // =============================================
        // ГЕНЕРАЦИЯ QR-КОДА
        // =============================================
        private void GenerateQRButton_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersDataGrid.SelectedItem is Order selectedOrder)
            {
                try
                {
                    // Формируем данные для QR-кода
                    string qrData = $"Номер заказа: {selectedOrder.OrderNumber}\n" +
                                   $"Клиент: {selectedOrder.Client?.CompanyName ?? "Не указан"}\n" +
                                   $"Откуда: {selectedOrder.PickupAddress}\n" +
                                   $"Куда: {selectedOrder.DeliveryAddress}\n" +
                                   $"Цена: {selectedOrder.Price:C}\n" +
                                   $"Расстояние: {selectedOrder.Distance} км\n" +
                                   $"Дата: {selectedOrder.OrderDate:dd.MM.yyyy HH:mm}\n" +
                                   $"Статус: {selectedOrder.Status?.StatusName ?? "Новый"}";

                    // Сохраняем QR-код во временный файл
                    string qrPath = Path.Combine(Path.GetTempPath(), $"QR_{selectedOrder.OrderNumber}.png");

                    // Используем простой способ - показываем сообщение с данными
                    // (так как библиотека QRCoder может быть не установлена)
                    MessageBox.Show($"QR-код для заказа {selectedOrder.OrderNumber}:\n\n{qrData}\n\nСохранен в: {qrPath}\n\n(Для полноценной генерации QR-кода установите пакет QRCoder)",
                                   "QR-код", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Создаем текстовый файл с данными вместо QR (как запасной вариант)
                    string txtPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"QR_Данные_{selectedOrder.OrderNumber}.txt");
                    File.WriteAllText(txtPath, qrData);

                    StatusText.Text = $"✅ Данные для QR-кода сохранены: {txtPath}";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Выберите заказ!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // =============================================
        // ГЕНЕРАЦИЯ PDF (TXT) ОТЧЕТА
        // =============================================
        private void GeneratePDFButton_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersDataGrid.SelectedItem is Order selectedOrder)
            {
                try
                {
                    // Путь для сохранения
                    string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    string fileName = $"Заказ_{selectedOrder.OrderNumber}_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
                    string filePath = Path.Combine(desktopPath, fileName);

                    // Формируем содержание отчета
                    string report = GenerateOrderReport(selectedOrder);

                    // Сохраняем файл
                    File.WriteAllText(filePath, report, System.Text.Encoding.UTF8);

                    StatusText.Text = $"✅ Отчет сохранен: {filePath}";

                    // Спрашиваем, открыть ли файл
                    if (MessageBox.Show($"Отчет успешно создан!\n\n{filePath}\n\nОткрыть файл?",
                                       "Успех", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                    {
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(filePath) { UseShellExecute = true });
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при создании отчета: {ex.Message}", "Ошибка",
                                   MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Выберите заказ!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // =============================================
        // ФОРМИРОВАНИЕ ОТЧЕТА
        // =============================================
        private string GenerateOrderReport(Order order)
        {
            string separator = new string('=', 50);
            string line = new string('-', 50);

            return $@"{separator}
                    ЗАКАЗ НА ГРУЗОПЕРЕВОЗКУ
{separator}

ИНФОРМАЦИЯ О ЗАКАЗЕ:
{line}
Номер заказа: {order.OrderNumber}
Дата создания: {order.OrderDate:dd.MM.yyyy HH:mm:ss}
Статус: {order.Status?.StatusName ?? "Новый"}

МАРШРУТ:
{line}
Пункт погрузки: {order.PickupAddress}
Пункт доставки: {order.DeliveryAddress}
Расстояние: {order.Distance} км

ФИНАНСОВАЯ ИНФОРМАЦИЯ:
{line}
Стоимость перевозки: {order.Price:C}

ИНФОРМАЦИЯ О КЛИЕНТЕ:
{line}
Компания: {order.Client?.CompanyName ?? "Не указано"}
Контактное лицо: {order.Client?.ContactPerson ?? "Не указано"}
Телефон: {order.Client?.ContactPhone ?? "Не указано"}

ИНФОРМАЦИЯ О ВОДИТЕЛЕ:
{line}
ФИО: {order.Driver?.FullName ?? "Не назначен"}
Телефон: {order.Driver?.Phone ?? "Не указан"}

ИНФОРМАЦИЯ О ТРАНСПОРТЕ:
{line}
Госномер: {order.Vehicle?.PlateNumber ?? "Не назначен"}
Марка/Модель: {order.Vehicle?.Brand ?? ""} {order.Vehicle?.Model ?? ""}
Грузоподъемность: {order.Vehicle?.LoadCapacity ?? 0} кг

ИНФОРМАЦИЯ О ГРУЗЕ:
{line}
Наименование: {order.Cargo?.Name ?? "Не указано"}
Вес: {order.Cargo?.Weight ?? 0} кг
Объем: {order.Cargo?.Volume ?? 0} м³

{separator}
Документ создан: {DateTime.Now:dd.MM.yyyy HH:mm:ss}
{separator}";
        }

        private void ClearForm()
        {
            ClientCombo.SelectedItem = null;
            DriverCombo.SelectedItem = null;
            VehicleCombo.SelectedItem = null;
            CargoCombo.SelectedItem = null;
            PickupAddressTextBox.Text = "";
            DeliveryAddressTextBox.Text = "";
            PriceTextBox.Text = "";
            DistanceTextBox.Text = "";
            QRCodeImage.Source = null;
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
            StatusText.Text = "🔄 Форма очищена";
        }

        private void OrdersDataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (OrdersDataGrid.SelectedItem is Order selected)
            {
                ClientCombo.SelectedValue = selected.ClientID;
                DriverCombo.SelectedValue = selected.DriverID;
                VehicleCombo.SelectedValue = selected.VehicleID;
                CargoCombo.SelectedValue = selected.CargoID;
                PickupAddressTextBox.Text = selected.PickupAddress;
                DeliveryAddressTextBox.Text = selected.DeliveryAddress;
                PriceTextBox.Text = selected.Price.ToString();
                DistanceTextBox.Text = selected.Distance.ToString();
                StatusText.Text = $"📋 Выбран заказ: {selected.OrderNumber}";
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Вы уверены, что хотите выйти?", "Выход",
                                        MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                var login = new LoginWindow();
                login.Show();
                this.Close();
            }
        }
    }
}