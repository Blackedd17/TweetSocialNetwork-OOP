using Social.Core.Entities;
using Social.Core.Repositories;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Social.Core.Services
{
    public class AuthService
    {
        private readonly IRepository<User> userRepository;

        public AuthService(IRepository<User> userRepository)
        {
            this.userRepository = userRepository;
        }
        public User Register(string username, string displayName, byte age, string password)
        {
            // username давхардах эсэхийг шалгана
            var existing = FindByUsername(username);
            if (existing != null)
                throw new InvalidOperationException("Username already exists.");

            string hash = PasswordHash(password);

            var user = new User(username, displayName, age, hash);
            userRepository.Add(user);
            return user;
        }
        public User Login(string username, string password)
        {
            var user = FindByUsername(username);
            if (user == null) return null;

            string hash = PasswordHash(password);
            if (user.PasswordHash != hash) return null;

            return user;
        }
        public User FindByUsername(string username)
        {
            foreach (var u in userRepository.GetAll())
            {
                if (string.Equals(u.Username, username, StringComparison.OrdinalIgnoreCase))
                    return u;
            }
            return null;
        }
        public static string PasswordHash(string password)
        {
            if (password == null) password = "";
            using (var sha = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = sha.ComputeHash(bytes);

                var sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }
    }
}