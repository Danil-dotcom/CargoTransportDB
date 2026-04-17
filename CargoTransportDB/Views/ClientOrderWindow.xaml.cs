using System;
using System.Linq;
using System.Windows;
using CargoTransportation.Models;

namespace CargoTransportation.Views
{
    public partial class ClientOrderWindow : Window
    {
        private User _currentUser;
        private Client _currentClient;
        private Vehicle _selectedVehicle;

        public ClientOrderWindow(User user, Vehicle vehicle)
        {
            InitializeComponent();
            _currentUser = user;
            _selectedVehicle = vehicle;

            DataContext = this;

            LoadClientData();
            LoadCargoTypes();
        }

        public Vehicle Vehicle => _selectedVehicle;

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
                        VehicleID = _selectedVehicle.VehicleID,
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

                    var clientOrdersWindow = new ClientOrdersWindow(_currentUser);
                    clientOrdersWindow.Show();
                    this.Close();
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

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            var catalogWindow = new TruckCatalogWindow(_currentUser);
            catalogWindow.Show();
            this.Close();
        }
    }
}