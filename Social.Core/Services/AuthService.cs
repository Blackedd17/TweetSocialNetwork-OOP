using Social.Core.Entities;
using Social.Core.Repositories;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Social.Core.Services
{
    /// <summary>
    /// Хэрэглэгчийн бүртгэл болон нэвтрэх үйлдлийг удирдах service class.
    /// 
    /// Энэ class нь:
    /// - Шинэ хэрэглэгч бүртгэх (Register)
    /// - Хэрэглэгч нэвтрэх (Login)
    /// - Username-аар хэрэглэгч хайх (FindByUsername)
    /// - Нууц үгийг hash болгон хувиргах (PasswordHash)
    /// 
    /// Бүх хэрэглэгчийн мэдээлэл IRepository ашиглан хадгалагдана.
    /// </summary>
    public class AuthService
    {
        private readonly IRepository<User> userRepository;

        /// <summary>
        /// AuthService-ийг repository-тай холбож үүсгэнэ.
        /// </summary>
        /// <param name="userRepository">User хадгалах repository</param>
        public AuthService(IRepository<User> userRepository)
        {
            this.userRepository = userRepository;
        }

        /// <summary>
        /// Шинэ хэрэглэгч бүртгэнэ.
        /// 
        /// Username давхардах эсэхийг шалгаж,
        /// нууц үгийг hash болгон хадгална.
        /// </summary>
        /// <param name="username">Хэрэглэгчийн нэр</param>
        /// <param name="displayName">Дэлгэцэнд харагдах нэр</param>
        /// <param name="age">Нас</param>
        /// <param name="password">Нууц үг</param>
        /// <returns>Үүсгэсэн хэрэглэгч</returns>
        public User Register(string username, string displayName, byte age, string password)
        {
            var existing = FindByUsername(username);
            if (existing != null)
                throw new InvalidOperationException("Username already exists.");

            string hash = PasswordHash(password);

            var user = new User(username, displayName, age, hash);
            userRepository.Add(user);
            return user;
        }

        /// <summary>
        /// Хэрэглэгчийг нэвтрүүлнэ.
        /// 
        /// Username болон password-ийг шалгаж,
        /// зөв бол хэрэглэгчийг буцаана.
        /// </summary>
        /// <param name="username">Хэрэглэгчийн нэр</param>
        /// <param name="password">Нууц үг</param>
        /// <returns>Амжилттай бол User, эс бөгөөс null</returns>
        public User Login(string username, string password)
        {
            var user = FindByUsername(username);
            if (user == null) return null;

            string hash = PasswordHash(password);
            if (user.PasswordHash != hash) return null;

            return user;
        }

        /// <summary>
        /// Username-аар хэрэглэгч хайна.
        /// 
        /// Repository доторх бүх хэрэглэгчийг шалгаж,
        /// таарах хэрэглэгчийг буцаана.
        /// </summary>
        /// <param name="username">Хайх username</param>
        /// <returns>Олдвол User, эс бөгөөс null</returns>
        public User FindByUsername(string username)
        {
            foreach (var u in userRepository.GetAll())
            {
                if (string.Equals(u.Username, username, StringComparison.OrdinalIgnoreCase))
                    return u;
            }
            return null;
        }

        /// <summary>
        /// Нууц үгийг SHA256 алгоритмаар hash болгон хувиргана.
        /// 
        /// Энэ нь password-ийг шууд хадгалахгүй,
        /// аюулгүй байдлыг хангана.
        /// </summary>
        /// <param name="password">Эх нууц үг</param>
        /// <returns>Hash болсон string</returns>
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