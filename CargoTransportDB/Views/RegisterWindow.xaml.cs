using System;
using System.Text.RegularExpressions;
using System.Windows;
using CargoTransportation.Services;

namespace CargoTransportation.Views
{
    public partial class RegisterWindow : Window
    {
        private AuthService _authService;

        public RegisterWindow()
        {
            InitializeComponent();
            _authService = new AuthService();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UsernameTextBox.Focus();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Валидация логина
                if (string.IsNullOrWhiteSpace(UsernameTextBox.Text))
                {
                    ShowError("Введите логин!");
                    UsernameTextBox.Focus();
                    return;
                }

                if (UsernameTextBox.Text.Length < 3)
                {
                    ShowError("Логин должен содержать минимум 3 символа!");
                    UsernameTextBox.Focus();
                    return;
                }

                if (UsernameTextBox.Text.Length > 50)
                {
                    ShowError("Логин не должен превышать 50 символов!");
                    UsernameTextBox.Focus();
                    return;
                }

                if (!Regex.IsMatch(UsernameTextBox.Text, @"^[a-zA-Z0-9_]+$"))
                {
                    ShowError("Логин может содержать только буквы, цифры и знак подчеркивания!");
                    UsernameTextBox.Focus();
                    return;
                }

                // Валидация email
                if (string.IsNullOrWhiteSpace(EmailTextBox.Text))
                {
                    ShowError("Введите email!");
                    EmailTextBox.Focus();
                    return;
                }

                if (!IsValidEmail(EmailTextBox.Text))
                {
                    ShowError("Введите корректный email (например: user@example.com)!");
                    EmailTextBox.Focus();
                    return;
                }

                if (EmailTextBox.Text.Length > 100)
                {
                    ShowError("Email не должен превышать 100 символов!");
                    EmailTextBox.Focus();
                    return;
                }

                // Валидация телефона
                if (string.IsNullOrWhiteSpace(PhoneTextBox.Text))
                {
                    ShowError("Введите номер телефона!");
                    PhoneTextBox.Focus();
                    return;
                }

                if (PhoneTextBox.Text.Length < 10 || PhoneTextBox.Text.Length > 20)
                {
                    ShowError("Телефон должен содержать от 10 до 20 символов!");
                    PhoneTextBox.Focus();
                    return;
                }

                // Валидация пароля
                if (string.IsNullOrWhiteSpace(PasswordBox.Password))
                {
                    ShowError("Введите пароль!");
                    PasswordBox.Focus();
                    return;
                }

                if (PasswordBox.Password.Length < 6)
                {
                    ShowError("Пароль должен содержать минимум 6 символов!");
                    PasswordBox.Focus();
                    return;
                }

                if (PasswordBox.Password.Length > 100)
                {
                    ShowError("Пароль не должен превышать 100 символов!");
                    PasswordBox.Focus();
                    return;
                }

                // Проверка совпадения паролей
                if (PasswordBox.Password != ConfirmPasswordBox.Password)
                {
                    ShowError("Пароли не совпадают!");
                    ConfirmPasswordBox.Focus();
                    return;
                }

                // Регистрация
                var result = _authService.Register(
                    UsernameTextBox.Text.Trim(),
                    EmailTextBox.Text.Trim().ToLower(),
                    PasswordBox.Password,
                    PhoneTextBox.Text.Trim()
                );

                if (result)
                {
                    MessageBox.Show("Регистрация успешна! Теперь вы можете войти в систему.",
                                   "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
                else
                {
                    ShowError("Пользователь с таким логином или email уже существует!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка регистрации: {ex.Message}", "Ошибка",
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                return regex.IsMatch(email);
            }
            catch
            {
                return false;
            }
        }

        private void UsernameTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            CheckFields();

            if (UsernameTextBox.Text.Length > 0 && UsernameTextBox.Text.Length < 3)
            {
                UsernameError.Text = "⚠ Минимум 3 символа";
                UsernameError.Visibility = Visibility.Visible;
            }
            else if (UsernameTextBox.Text.Length > 50)
            {
                UsernameError.Text = "⚠ Максимум 50 символов";
                UsernameError.Visibility = Visibility.Visible;
            }
            else
            {
                UsernameError.Visibility = Visibility.Collapsed;
            }
        }

        private void EmailTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            CheckFields();

            if (!string.IsNullOrWhiteSpace(EmailTextBox.Text) && !IsValidEmail(EmailTextBox.Text))
            {
                EmailError.Text = "⚠ Неверный формат email";
                EmailError.Visibility = Visibility.Visible;
            }
            else
            {
                EmailError.Visibility = Visibility.Collapsed;
            }
        }

        private void PhoneTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            CheckFields();

            if (PhoneTextBox.Text.Length > 0 && (PhoneTextBox.Text.Length < 10 || PhoneTextBox.Text.Length > 20))
            {
                PhoneError.Text = "⚠ От 10 до 20 символов";
                PhoneError.Visibility = Visibility.Visible;
            }
            else
            {
                PhoneError.Visibility = Visibility.Collapsed;
            }
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            CheckFields();

            if (PasswordBox.Password.Length > 0 && PasswordBox.Password.Length < 6)
            {
                PasswordError.Text = "⚠ Минимум 6 символов";
                PasswordError.Visibility = Visibility.Visible;
            }
            else if (PasswordBox.Password.Length > 100)
            {
                PasswordError.Text = "⚠ Максимум 100 символов";
                PasswordError.Visibility = Visibility.Visible;
            }
            else
            {
                PasswordError.Visibility = Visibility.Collapsed;
            }

            CheckPasswordMatch();
        }

        private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            CheckFields();
            CheckPasswordMatch();
        }

        private void CheckPasswordMatch()
        {
            if (!string.IsNullOrWhiteSpace(ConfirmPasswordBox.Password) &&
                PasswordBox.Password != ConfirmPasswordBox.Password)
            {
                ConfirmPasswordError.Text = "⚠ Пароли не совпадают";
                ConfirmPasswordError.Visibility = Visibility.Visible;
            }
            else
            {
                ConfirmPasswordError.Visibility = Visibility.Collapsed;
            }
        }

        private void CheckFields()
        {
            bool isValid = !string.IsNullOrWhiteSpace(UsernameTextBox.Text) &&
                          UsernameTextBox.Text.Length >= 3 &&
                          UsernameTextBox.Text.Length <= 50 &&
                          !string.IsNullOrWhiteSpace(EmailTextBox.Text) &&
                          IsValidEmail(EmailTextBox.Text) &&
                          !string.IsNullOrWhiteSpace(PhoneTextBox.Text) &&
                          PhoneTextBox.Text.Length >= 10 &&
                          PhoneTextBox.Text.Length <= 20 &&
                          !string.IsNullOrWhiteSpace(PasswordBox.Password) &&
                          PasswordBox.Password.Length >= 6 &&
                          PasswordBox.Password.Length <= 100 &&
                          !string.IsNullOrWhiteSpace(ConfirmPasswordBox.Password) &&
                          PasswordBox.Password == ConfirmPasswordBox.Password;

            RegisterButton.IsEnabled = isValid;
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