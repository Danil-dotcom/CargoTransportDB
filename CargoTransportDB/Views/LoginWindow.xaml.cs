using CargoTransportation.Services;
using System;
using System.Linq;
using System.Windows;

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
                    var mainWindow = new MainWindow(user);
                    mainWindow.Show();
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

        private void DebugBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var context = new Data.CargoDbContext())
                {
                    var admin = context.Users.FirstOrDefault(u => u.Username == "admin");
                    var auth = new AuthService();
                    string testHash = auth.HashPassword("admin");

                    string message = "";
                    message += "=== ОТЛАДКА АВТОРИЗАЦИИ ===\n\n";

                    if (admin == null)
                    {
                        message += "❌ Пользователь 'admin' не найден в базе данных!\n\n";
                        message += "Выполните SQL запрос:\n";
                        message += "INSERT INTO Users (Username, Email, PasswordHash, Phone, RoleID, RegistrationDate, IsActive)\n";
                        message += "VALUES ('admin', 'admin@cargo.com', '8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918', '+7(999)000-00-00', 1, GETDATE(), 1);\n";
                    }
                    else
                    {
                        message += $"✅ Пользователь найден: {admin.Username}\n";
                        message += $"📧 Email: {admin.Email}\n";
                        message += $"🔐 Хэш в БД: {admin.PasswordHash}\n";
                        message += $"📏 Длина хэша в БД: {admin.PasswordHash?.Length ?? 0} символов\n\n";
                        message += $"🔧 Хэш для 'admin': {testHash}\n";
                        message += $"📏 Длина хэша: {testHash.Length} символов\n\n";

                        bool isValid = auth.VerifyPassword("admin", admin.PasswordHash);
                        message += $"🔍 Результат проверки пароля: {(isValid ? "✅ ВЕРНЫЙ" : "❌ НЕВЕРНЫЙ")}\n\n";

                        if (!isValid)
                        {
                            message += "⚠️ Хэши не совпадают!\n";
                            message += "Возможные причины:\n";
                            message += "1. Разные методы хэширования\n";
                            message += "2. Разная кодировка (UTF-8 vs ASCII)\n";
                            message += "3. Лишние пробелы или символы\n\n";
                            message += "Рекомендация: Обновите хэш в БД:\n";
                            message += $"UPDATE Users SET PasswordHash = '{testHash}' WHERE Username = 'admin';\n";
                        }
                    }

                    MessageBox.Show(message, "Диагностика", MessageBoxButton.OK,
                                   admin == null ? MessageBoxImage.Warning : MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка диагностики:\n{ex.Message}", "Ошибка",
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
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