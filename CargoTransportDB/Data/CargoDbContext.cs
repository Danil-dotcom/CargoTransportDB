using System.Data.Entity;
using CargoTransportation.Models;

namespace CargoTransportation.Data
{
    public class CargoDbContext : DbContext
    {
        public CargoDbContext() : base("name=CargoDbContext")
        {
            // Эта строка создаст базу данных, если её нет
            Database.SetInitializer(new CargoDbInitializer());

            // Принудительно создаем базу данных при первом обращении
            // Database.Initialize(true);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Cargo> Cargos { get; set; }
        public DbSet<CargoType> CargoTypes { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderStatus> OrderStatuses { get; set; }
        public DbSet<Tariff> Tariffs { get; set; }
        public DbSet<Insurance> Insurances { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Route> Routes { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Отключаем каскадное удаление (опционально)
            modelBuilder.Conventions.Remove<System.Data.Entity.ModelConfiguration.Conventions.OneToManyCascadeDeleteConvention>();
        }
    }
}