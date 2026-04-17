using System;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using System.IO;
using CargoTransportation.Models;
using System.Windows.Controls;

namespace CargoTransportation.Views
{
    public partial class VehicleSelectionWindow : Window
    {
        private int _selectedVehicleId;
        private bool _isSelected = false;

        public int SelectedVehicleId => _selectedVehicleId;
        public bool IsSelected => _isSelected;

        public VehicleSelectionWindow()
        {
            InitializeComponent();
            LoadVehicles();
        }

        private void LoadVehicles()
        {
            using (var context = new Data.CargoDbContext())
            {
                var vehicles = context.Vehicles.ToList();

                // Загружаем картинки для каждого транспорта
                foreach (var vehicle in vehicles)
                {
                    vehicle.ImagePath = GetImagePath(vehicle.ImagePath);
                }

                VehiclesList.ItemsSource = vehicles;
            }
        }

        private string GetImagePath(string imageName)
        {
            if (string.IsNullOrEmpty(imageName))
                return null;

            // Путь к папке Images в проекте
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string imagePath = Path.Combine(basePath, "Images", imageName);

            if (File.Exists(imagePath))
                return imagePath;

            // Если картинки нет, возвращаем заглушку
            string defaultPath = Path.Combine(basePath, "Images", "no_image.png");
            if (File.Exists(defaultPath))
                return defaultPath;

            return null;
        }

        private void SelectVehicle_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null && btn.Tag != null)
            {
                _selectedVehicleId = (int)btn.Tag;
                _isSelected = true;
                this.Close();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            _isSelected = false;
            this.Close();
        }
    }
}