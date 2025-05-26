using OrderManagementApi.Models;
using BCrypt.Net;

namespace OrderManagementApi.Data
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            // Переконаємось, що база даних створена
            context.Database.EnsureCreated();

            // Якщо користувачі вже є, то не ініціалізуємо знову
            if (context.Users.Any())
                return;

            // Створимо адміна
            var admin = new User
            {
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                Role = "Admin"
            };

            // Звичайний користувач
            var user = new User
            {
                Username = "user1",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("user123"),
                Role = "User"
            };

            context.Users.AddRange(admin, user);
            context.SaveChanges();

            // Створимо кілька замовлень
            var orders = new Order[]
            {
                new Order { Title = "Order 1", Description = "First order description", UserId = admin.Id },
                new Order { Title = "Order 2", Description = "Second order description", UserId = user.Id }
            };

            context.Orders.AddRange(orders);
            context.SaveChanges();
        }
    }
}
