using System;
using System.Linq;
using System.Windows;
using CargoTransportation.Models;

namespace CargoTransportation.Views
{
    public partial class OrderDetailsWindow : Window
    {
        private Order _order;
        private int _clientId;
        private Action _onOrderUpdated;

        public OrderDetailsWindow(Order order, int clientId, Action onOrderUpdated)
        {
            InitializeComponent();
            _order = order;
            _clientId = clientId;
            _onOrderUpdated = onOrderUpdated;

            LoadOrderData();
            LoadStatuses();
        }

        private void LoadOrderData()
        {
            OrderNumberText.Text = _order.OrderNumber;
            OrderDateText.Text = _order.OrderDate.ToString("dd.MM.yyyy HH:mm");
            PickupAddressText.Text = _order.PickupAddress;
            DeliveryAddressText.Text = _order.DeliveryAddress;
            DistanceText.Text = _order.Distance.ToString();
            PriceText.Text = _order.Price.ToString();

            if (_order.Cargo != null)
            {
                CargoNameText.Text = _order.Cargo.Name;
            }
            else
            {
                CargoNameText.Text = "Не указан";
            }
        }

        private void LoadStatuses()
        {
            using (var context = new Data.CargoDbContext())
            {
                var statuses = context.OrderStatuses.ToList();
                StatusCombo.ItemsSource = statuses;
                StatusCombo.DisplayMemberPath = "StatusName";
                StatusCombo.SelectedValuePath = "StatusID";
                StatusCombo.SelectedValue = _order.StatusID;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(PickupAddressText.Text) ||
                    string.IsNullOrWhiteSpace(DeliveryAddressText.Text) ||
                    string.IsNullOrWhiteSpace(DistanceText.Text) ||
                    string.IsNullOrWhiteSpace(PriceText.Text))
                {
                    MessageBox.Show("Заполните все поля!", "Ошибка",
                                   MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                using (var context = new Data.CargoDbContext())
                {
                    var orderToUpdate = context.Orders.FirstOrDefault(x => x.OrderID == _order.OrderID);

                    if (orderToUpdate == null)
                    {
                        MessageBox.Show("Заказ не найден!", "Ошибка",
                                       MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    orderToUpdate.PickupAddress = PickupAddressText.Text;
                    orderToUpdate.DeliveryAddress = DeliveryAddressText.Text;
                    orderToUpdate.Distance = decimal.Parse(DistanceText.Text);
                    orderToUpdate.Price = decimal.Parse(PriceText.Text);

                    if (StatusCombo.SelectedValue != null)
                    {
                        orderToUpdate.StatusID = (int)StatusCombo.SelectedValue;
                    }

                    context.SaveChanges();

                    MessageBox.Show("Заказ успешно обновлен!", "Успех",
                                   MessageBoxButton.OK, MessageBoxImage.Information);

                    _onOrderUpdated?.Invoke();
                    this.Close();
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Проверьте правильность ввода чисел в полях Расстояние и Цена!",
                               "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка сохранения: " + ex.Message, "Ошибка",
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Удалить заказ №" + _order.OrderNumber + "?\n\nЭто действие нельзя отменить!",
                                        "Подтверждение удаления",
                                        MessageBoxButton.YesNo,
                                        MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (var context = new Data.CargoDbContext())
                    {
                        var orderToDelete = context.Orders.FirstOrDefault(x => x.OrderID == _order.OrderID);

                        if (orderToDelete == null)
                        {
                            MessageBox.Show("Заказ не найден!", "Ошибка",
                                           MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        context.Orders.Remove(orderToDelete);
                        context.SaveChanges();

                        MessageBox.Show("Заказ удален!", "Успех",
                                       MessageBoxButton.OK, MessageBoxImage.Information);

                        _onOrderUpdated?.Invoke();
                        this.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка удаления: " + ex.Message, "Ошибка",
                                   MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}