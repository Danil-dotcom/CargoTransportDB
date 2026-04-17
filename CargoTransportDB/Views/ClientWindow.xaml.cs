using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using CargoTransportation.Models;

namespace CargoTransportation.Views
{
    public partial class ClientWindow : Window
    {
        private User _currentUser;
        private Client _currentClient;
        private Order _selectedOrder;
        private int _selectedVehicleId;

        public ClientWindow(User user)
        {
            InitializeComponent();
            _currentUser = user;

            WelcomeText.Text = "Добро пожаловать, " + user.Username;

            LoadClientData();
            LoadCargoTypes();
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

        private void LoadCargoTypes()
        {
            using (var context = new Data.CargoDbContext())
            {
                CargoTypeCombo.ItemsSource = context.CargoTypes.ToList();
                CargoTypeCombo.DisplayMemberPath = "TypeName";
                CargoTypeCombo.SelectedValuePath = "CargoTypeID";
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

        private void CalculateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(DistanceTextBox.Text))
                {
                    MessageBox.Show("Введите расстояние");
                    return;
                }

                double distance = double.Parse(DistanceTextBox.Text);
                double weight = string.IsNullOrWhiteSpace(WeightTextBox.Text) ? 0 : double.Parse(WeightTextBox.Text);

                decimal totalPrice = (decimal)(distance * 50) + (decimal)(weight * 10);
                if (totalPrice < 1000) totalPrice = 1000;

                PriceText.Text = "Стоимость: " + totalPrice.ToString("C");
                PriceResultPanel.Visibility = Visibility.Visible;
            }
            catch
            {
                MessageBox.Show("Ошибка ввода чисел");
            }
        }

        private void SelectVehicleButton_Click(object sender, RoutedEventArgs e)
        {
            var vehicleWindow = new VehicleSelectionWindow();
            vehicleWindow.Owner = this;
            vehicleWindow.ShowDialog();

            if (vehicleWindow.IsSelected)
            {
                _selectedVehicleId = vehicleWindow.SelectedVehicleId;

                using (var context = new Data.CargoDbContext())
                {
                    var vehicle = context.Vehicles.Find(_selectedVehicleId);
                    if (vehicle != null)
                    {
                        SelectedVehicleText.Text = vehicle.Brand + " " + vehicle.Model + " (" + vehicle.PlateNumber + ")";
                    }
                }
            }
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(PickupAddressTextBox.Text) ||
                    string.IsNullOrWhiteSpace(DeliveryAddressTextBox.Text) ||
                    string.IsNullOrWhiteSpace(DistanceTextBox.Text) ||
                    CargoTypeCombo.SelectedItem == null ||
                    string.IsNullOrWhiteSpace(CargoNameTextBox.Text))
                {
                    MessageBox.Show("Заполните все поля!");
                    return;
                }

                if (_selectedVehicleId == 0)
                {
                    MessageBox.Show("Выберите транспортное средство!");
                    return;
                }

                using (var context = new Data.CargoDbContext())
                {
                    var cargo = new Cargo
                    {
                        CargoTypeID = (int)CargoTypeCombo.SelectedValue,
                        Name = CargoNameTextBox.Text,
                        Weight = string.IsNullOrWhiteSpace(WeightTextBox.Text) ? 0 : decimal.Parse(WeightTextBox.Text),
                        Volume = 0,
                        DangerousGoods = false,
                        Description = ""
                    };
                    context.Cargos.Add(cargo);
                    context.SaveChanges();

                    double distance = double.Parse(DistanceTextBox.Text);
                    double weight = string.IsNullOrWhiteSpace(WeightTextBox.Text) ? 0 : double.Parse(WeightTextBox.Text);

                    decimal totalPrice = (decimal)(distance * 50) + (decimal)(weight * 10);
                    if (totalPrice < 1000) totalPrice = 1000;

                    var order = new Order
                    {
                        OrderNumber = "ORD-" + DateTime.Now.ToString("yyyyMMdd") + "-" + new Random().Next(1000, 9999),
                        ClientID = _currentClient.ClientID,
                        DriverID = GetAvailableDriver(),
                        VehicleID = _selectedVehicleId,
                        CargoID = cargo.CargoID,
                        StatusID = 1,
                        PickupAddress = PickupAddressTextBox.Text,
                        DeliveryAddress = DeliveryAddressTextBox.Text,
                        OrderDate = DateTime.Now,
                        Price = totalPrice,
                        Distance = (decimal)distance,
                        Description = ""
                    };
                    context.Orders.Add(order);
                    context.SaveChanges();

                    MessageBox.Show("Заказ оформлен! Номер: " + order.OrderNumber);

                    ClearForm();
                    LoadClientOrders();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        private int GetAvailableDriver()
        {
            using (var context = new Data.CargoDbContext())
            {
                var driver = context.Drivers.FirstOrDefault(d => d.Status == "Available");
                if (driver != null) return driver.DriverID;
                return context.Drivers.First().DriverID;
            }
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
                        .FirstOrDefault(x => x.OrderID == orderId);

                    if (fullOrder != null)
                    {
                        var detailsWindow = new OrderDetailsWindow(fullOrder, _currentClient.ClientID, LoadClientOrders);
                        detailsWindow.Owner = this;
                        detailsWindow.ShowDialog();
                    }
                    else
                    {
                        MessageBox.Show("Заказ не найден!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            PickupAddressTextBox.Text = "";
            DeliveryAddressTextBox.Text = "";
            DistanceTextBox.Text = "";
            CargoTypeCombo.SelectedItem = null;
            CargoNameTextBox.Text = "";
            WeightTextBox.Text = "";
            SelectedVehicleText.Text = "";
            _selectedVehicleId = 0;
            PriceResultPanel.Visibility = Visibility.Collapsed;
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