using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
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
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.Username == username && u.IsActive);

                if (user != null && VerifyPassword(password, user.PasswordHash))
                {
                    return user;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        // SHA256 хэширование в HEX строку (как в базе данных)
        public string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

                // Преобразуем в HEX строку (как в базе данных)
                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashedBytes)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }

        public bool VerifyPassword(string password, string hash)
        {
            string hashOfInput = HashPassword(password);
            return string.Equals(hashOfInput, hash, StringComparison.OrdinalIgnoreCase);
        }
    }
}