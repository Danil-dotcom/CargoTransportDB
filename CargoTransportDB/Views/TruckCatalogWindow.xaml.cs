using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using CargoTransportation.Models;

namespace CargoTransportation.Views
{
    public partial class TruckCatalogWindow : Window
    {
        private User _currentUser;

        public TruckCatalogWindow(User user)
        {
            InitializeComponent();
            _currentUser = user;
            LoadTrucks();
        }

        private void LoadTrucks()
        {
            using (var context = new Data.CargoDbContext())
            {
                var vehicles = context.Vehicles.ToList();
                TrucksList.ItemsSource = vehicles;
            }
        }

        private void SelectTruck_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null && btn.Tag != null)
            {
                int vehicleId = (int)btn.Tag;

                using (var context = new Data.CargoDbContext())
                {
                    var vehicle = context.Vehicles.Find(vehicleId);
                    if (vehicle != null)
                    {
                        var orderWindow = new ClientOrderWindow(_currentUser, vehicle);
                        orderWindow.Show();
                        this.Close();
                    }
                }
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Выйти?", "Выход", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                new LoginWindow().Show();
                this.Close();
            }
        }
    }
}