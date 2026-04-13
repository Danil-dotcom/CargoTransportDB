using CargoTransportation.Data;
using CargoTransportation.Views;
using System;
using System.Linq;
using System.Windows;

namespace CargoTransportation
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                // ПРИНУДИТЕЛЬНОЕ СОЗДАНИЕ БАЗЫ ДАННЫХ
                using (var context = new CargoDbContext())
                {
                    // Создает базу данных, если её нет
                    context.Database.CreateIfNotExists();

                    // Проверяем, что база создалась
                    if (context.Database.Exists())
                    {
                        System.Diagnostics.Debug.WriteLine("База данных успешно создана!");

                        // Проверяем количество таблиц
                        var tableCount = context.Database.SqlQuery<int>("SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'").FirstOrDefault();
                        System.Diagnostics.Debug.WriteLine($"Создано таблиц: {tableCount}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании базы данных:\n{ex.Message}\n\n{ex.InnerException?.Message}",
                               "Ошибка БД", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            var loginWindow = new LoginWindow();
            loginWindow.Show();
        }
    }
}