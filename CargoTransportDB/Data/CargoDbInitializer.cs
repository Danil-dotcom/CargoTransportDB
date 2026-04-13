using System.Data.Entity;
using CargoTransportation.Models;
using System.Linq;

namespace CargoTransportation.Data
{
    public class CargoDbInitializer : CreateDatabaseIfNotExists<CargoDbContext>
    {
        protected override void Seed(CargoDbContext context)
        {
            // 1. Роли
            if (!context.Roles.Any())
            {
                context.Roles.AddRange(new[]
                {
                    new Role { RoleName = "Admin" },
                    new Role { RoleName = "Client" },
                    new Role { RoleName = "Driver" },
                    new Role { RoleName = "Manager" }
                });
                context.SaveChanges();
            }

            // 2. Статусы заказов
            if (!context.OrderStatuses.Any())
            {
                context.OrderStatuses.AddRange(new[]
                {
                    new OrderStatus { StatusName = "Новый" },
                    new OrderStatus { StatusName = "В обработке" },
                    new OrderStatus { StatusName = "В пути" },
                    new OrderStatus { StatusName = "Доставлен" },
                    new OrderStatus { StatusName = "Отменен" }
                });
                context.SaveChanges();
            }

            // 3. Типы грузов
            if (!context.CargoTypes.Any())
            {
                context.CargoTypes.AddRange(new[]
                {
                    new CargoType { TypeName = "Обычный", RequiresSpecialCondition = false },
                    new CargoType { TypeName = "Хрупкий", RequiresSpecialCondition = true },
                    new CargoType { TypeName = "Опасный", RequiresSpecialCondition = true },
                    new CargoType { TypeName = "Скоропортящийся", RequiresSpecialCondition = true }
                });
                context.SaveChanges();
            }

            // 4. Тарифы
            if (!context.Tariffs.Any())
            {
                context.Tariffs.AddRange(new[]
                {
                    new Tariff { TariffName = "Стандартный", PricePerKm = 50, PricePerKg = 10, MinPrice = 1000 },
                    new Tariff { TariffName = "Экспресс", PricePerKm = 100, PricePerKg = 20, MinPrice = 2000 },
                    new Tariff { TariffName = "Премиум", PricePerKm = 150, PricePerKg = 30, MinPrice = 3000 }
                });
                context.SaveChanges();
            }

            // 5. Тестовый пользователь (пароль: 123456)
            if (!context.Users.Any())
            {
                var user = new User
                {
                    Username = "user",
                    Email = "user@test.com",
                    PasswordHash = "E10ADC3949BA59ABBE56E057F20F883E",
                    Phone = "+7(999)111-22-33",
                    RoleID = 2,
                    RegistrationDate = System.DateTime.Now,
                    IsActive = true
                };
                context.Users.Add(user);
                context.SaveChanges();

                // Клиент для пользователя
                var client = new Client
                {
                    UserID = user.UserID,
                    CompanyName = "ООО Тестовая компания",
                    ContactPerson = "Иванов Иван",
                    ContactPhone = "+7(999)111-22-33"
                };
                context.Clients.Add(client);
                context.SaveChanges();
            }

            // 6. Тестовый водитель
            if (!context.Drivers.Any())
            {
                context.Drivers.Add(new Driver
                {
                    FullName = "Петров Петр Петрович",
                    LicenseNumber = "1234567890",
                    Phone = "+7(999)888-77-66",
                    HireDate = System.DateTime.Now,
                    Salary = 50000,
                    Status = "Available"
                });
                context.SaveChanges();
            }

            // 7. Тестовый транспорт
            if (!context.Vehicles.Any())
            {
                context.Vehicles.Add(new Vehicle
                {
                    PlateNumber = "А123ВС",
                    Brand = "Volvo",
                    Model = "FH16",
                    Year = 2020,
                    Capacity = 20000,
                    LoadCapacity = 20000,
                    Status = "Available"
                });
                context.SaveChanges();
            }

            // 8. Тестовый груз
            if (!context.Cargos.Any())
            {
                context.Cargos.Add(new Cargo
                {
                    CargoTypeID = 1,
                    Name = "Строительные материалы",
                    Weight = 5000,
                    Volume = 10,
                    DangerousGoods = false,
                    Description = "Кирпич, цемент, песок"
                });
                context.SaveChanges();
            }
        }
    }
}