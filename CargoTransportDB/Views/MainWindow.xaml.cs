using CargoTransportation.Models;
using CargoTransportation.Services;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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

        private void GenerateQRButton_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersDataGrid.SelectedItem is Order selected)
            {
                MessageBox.Show($"QR-код для заказа {selected.OrderNumber}\n\nДанные заказа готовы для генерации QR-кода",
                    "QR-код", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void GeneratePDFButton_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersDataGrid.SelectedItem is Order selected)
            {
                MessageBox.Show($"PDF для заказа {selected.OrderNumber}\n\nФункция будет добавлена позже",
                    "PDF", MessageBoxButton.OK, MessageBoxImage.Information);
            }
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
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
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
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var login = new LoginWindow();
            login.Show();
            this.Close();
        }
    }
}