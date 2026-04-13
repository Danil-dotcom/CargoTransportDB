using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using CargoTransportation.Data;
using CargoTransportation.Models;

namespace CargoTransportation.Services
{
    public class AuthService
    {
        private readonly CargoDbContext _context;

        public AuthService()
        {
            _context = new CargoDbContext();
        }

        public bool Register(string username, string email, string password, string phone)
        {
            try
            {
                // Дополнительные проверки на стороне сервера
                if (string.IsNullOrWhiteSpace(username) || username.Length < 3 || username.Length > 50)
                    return false;

                if (string.IsNullOrWhiteSpace(email) || !IsValidEmail(email) || email.Length > 100)
                    return false;

                if (string.IsNullOrWhiteSpace(password) || password.Length < 6 || password.Length > 100)
                    return false;

                if (string.IsNullOrWhiteSpace(phone) || phone.Length < 10 || phone.Length > 20)
                    return false;

                if (_context.Users.Any(u => u.Username == username || u.Email == email))
                    return false;

                var passwordHash = HashPassword(password);
                var user = new User
                {
                    Username = username,
                    Email = email,
                    PasswordHash = passwordHash,
                    Phone = phone,
                    RoleID = 2,
                    RegistrationDate = DateTime.Now,
                    IsActive = true
                };

                _context.Users.Add(user);
                _context.SaveChanges();

                var client = new Client
                {
                    UserID = user.UserID,
                    CompanyName = "",
                    ContactPerson = ""
                };
                _context.Clients.Add(client);
                _context.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public User Login(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || username.Length < 3 || username.Length > 50)
                return null;

            if (string.IsNullOrWhiteSpace(password) || password.Length < 6 || password.Length > 100)
                return null;

            var user = _context.Users.FirstOrDefault(u => u.Username == username && u.IsActive);
            if (user != null && VerifyPassword(password, user.PasswordHash))
            {
                return user;
            }
            return null;
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private bool VerifyPassword(string password, string hash)
        {
            var hashOfInput = HashPassword(password);
            return hashOfInput == hash;
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
    }
}