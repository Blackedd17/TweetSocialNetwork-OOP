using System;
using Social.Core.Entities;
using Social.Core.Repositories;

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

            string hash = PasswordHasher.Hash(password);

            var user = new User(username, displayName, age, hash);
            userRepository.Add(user);
            return user;
        }
        public User Login(string username, string password)
        {
            var user = FindByUsername(username);
            if (user == null) return null;

            string hash = PasswordHasher.Hash(password);
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
    }
}