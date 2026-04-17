using System;
using System.Windows;
using CargoTransportation.Services;

namespace CargoTransportation.Views
{
    public partial class LoginWindow : Window
    {
        private AuthService _authService;

        public LoginWindow()
        {
            InitializeComponent();
            _authService = new AuthService();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string username = UsernameTextBox.Text?.Trim();
                string password = PasswordBox.Password;

                if (string.IsNullOrWhiteSpace(username))
                {
                    ShowError("Введите логин!");
                    return;
                }

                if (string.IsNullOrWhiteSpace(password))
                {
                    ShowError("Введите пароль!");
                    return;
                }

                var user = _authService.Login(username, password);

                if (user != null)
                {
                    if (user.RoleID == 2) // Клиент
                    {
                        // Открываем окно с заказами клиента
                        var clientOrdersWindow = new ClientOrdersWindow(user);
                        clientOrdersWindow.Show();
                    }
                    else // Администратор
                    {
                        var mainWindow = new MainWindow(user);
                        mainWindow.Show();
                    }
                    this.Close();
                }
                else
                {
                    ShowError("Неверный логин или пароль!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            var registerWindow = new RegisterWindow();
            registerWindow.Owner = this;
            registerWindow.ShowDialog();
        }

        private void ShowError(string message)
        {
            ErrorTextBlock.Text = $"❌ {message}";
            ErrorTextBlock.Visibility = Visibility.Visible;

            var timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(3);
            timer.Tick += (s, args) =>
            {
                ErrorTextBlock.Visibility = Visibility.Collapsed;
                timer.Stop();
            };
            timer.Start();
        }
    }
}