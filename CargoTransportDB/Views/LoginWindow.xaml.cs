using System;
using System.Text.RegularExpressions;
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UsernameTextBox.Focus();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Ограничения и проверки
                if (string.IsNullOrWhiteSpace(UsernameTextBox.Text))
                {
                    ShowError("Введите логин!");
                    return;
                }

                if (UsernameTextBox.Text.Length < 3)
                {
                    ShowError("Логин должен содержать минимум 3 символа!");
                    return;
                }

                if (UsernameTextBox.Text.Length > 50)
                {
                    ShowError("Логин не должен превышать 50 символов!");
                    return;
                }

                if (string.IsNullOrWhiteSpace(PasswordBox.Password))
                {
                    ShowError("Введите пароль!");
                    return;
                }

                if (PasswordBox.Password.Length < 6)
                {
                    ShowError("Пароль должен содержать минимум 6 символов!");
                    return;
                }

                if (PasswordBox.Password.Length > 100)
                {
                    ShowError("Пароль не должен превышать 100 символов!");
                    return;
                }

                var user = _authService.Login(UsernameTextBox.Text.Trim(), PasswordBox.Password);

                if (user != null)
                {
                    var mainWindow = new MainWindow(user);
                    mainWindow.Show();
                    this.Close();
                }
                else
                {
                    ShowError("Неверное имя пользователя или пароль!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка входа: {ex.Message}", "Ошибка",
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            var registerWindow = new RegisterWindow();
            registerWindow.Owner = this;
            registerWindow.ShowDialog();
        }

        private void UsernameTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            CheckFields();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            CheckFields();
        }

        private void CheckFields()
        {
            bool isValid = !string.IsNullOrWhiteSpace(UsernameTextBox.Text) &&
                          !string.IsNullOrWhiteSpace(PasswordBox.Password) &&
                          UsernameTextBox.Text.Length >= 3 &&
                          PasswordBox.Password.Length >= 6;

            LoginButton.IsEnabled = isValid;
        }

        private void ShowError(string message)
        {
            ErrorTextBlock.Text = $"❌ {message}";
            ErrorTextBlock.Visibility = Visibility.Visible;

            // Скрыть ошибку через 3 секунды
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