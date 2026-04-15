using System;
using System.Collections.Generic;
using System.Linq;
using Social.Core.Entities;
using Social.Core.Repositories;

namespace Social.Core.Services
{
    /// <summary>
    /// User (хэрэглэгч)-тэй холбоотой бизнес логикийг удирдах service class.
    /// 
    /// Энэ class нь:
    /// - Хэрэглэгч үүсгэх (CreateUser)
    /// - Бүх хэрэглэгчийг авах (GetUsers)
    /// - Дагах / дагахаа болих (Follow, Unfollow)
    /// - Дагаж байгаа эсэхийг шалгах (IsFollowing)
    /// - Username-аар хайх (FindByUsername)
    /// - ID-р хэрэглэгч авах (GetById)
    /// - Хэрэглэгч хайх (SearchUsers)
    /// 
    /// IRepository ашиглан хэрэглэгчийн өгөгдөлтэй ажиллана.
    /// </summary>
    public class UserService
    {
        private readonly IRepository<User> userRepository;

        /// <summary>
        /// UserService-ийг repository-тэй холбож үүсгэнэ.
        /// </summary>
        /// <param name="userRepository">User хадгалах repository</param>
        public UserService(IRepository<User> userRepository)
        {
            this.userRepository = userRepository;
        }

        /// <summary>
        /// Шинэ хэрэглэгч үүсгэн хадгална.
        /// </summary>
        /// <param name="user">Үүсгэх хэрэглэгч</param>
        public void CreateUser(User user)
        {
            userRepository.Add(user);
        }

        /// <summary>
        /// Бүх хэрэглэгчдийн жагсаалтыг буцаана.
        /// </summary>
        /// <returns>User жагсаалт</returns>
        public List<User> GetUsers()
        {
            return userRepository.GetAll();
        }

        /// <summary>
        /// Нэг хэрэглэгч нөгөөг дагаж байгаа эсэхийг шалгана.
        /// </summary>
        /// <param name="follower">Дагаж буй хэрэглэгч</param>
        /// <param name="target">Дагагдаж буй хэрэглэгч</param>
        /// <returns>Дагаж байвал true, үгүй бол false</returns>
        public bool IsFollowing(User follower, User target)
        {
            if (follower == null || target == null) return false;
            return follower.Following.Contains(target.Id);
        }

        /// <summary>
        /// Нэг хэрэглэгчийг нөгөөг дагах үйлдэл.
        /// 
        /// Давхар дагах болон өөрийгөө дагахаас хамгаална.
        /// </summary>
        /// <param name="follower">Дагаж буй хэрэглэгч</param>
        /// <param name="target">Дагагдаж буй хэрэглэгч</param>
        public void Follow(User follower, User target)
        {
            if (follower == null || target == null) return;
            if (follower.Id == target.Id) return;

            if (!follower.Following.Contains(target.Id))
            {
                follower.Following.Add(target.Id);
            }

            if (!target.Followers.Contains(follower.Id))
            {
                target.Followers.Add(follower.Id);
            }
        }

        /// <summary>
        /// Дагаж буй хэрэглэгчийг болиулах үйлдэл.
        /// </summary>
        /// <param name="follower">Дагаж буй хэрэглэгч</param>
        /// <param name="target">Дагагдаж буй хэрэглэгч</param>
        public void Unfollow(User follower, User target)
        {
            if (follower == null || target == null) return;
            if (follower.Id == target.Id) return;

            if (follower.Following.Contains(target.Id))
            {
                follower.Following.Remove(target.Id);
            }

            if (target.Followers.Contains(follower.Id))
            {
                target.Followers.Remove(follower.Id);
            }
        }

        /// <summary>
        /// Username-аар хэрэглэгч хайна.
        /// </summary>
        /// <param name="username">Хайх username</param>
        /// <returns>Олдвол User, эс бөгөөс null</returns>
        public User FindByUsername(string username)
        {
            foreach (var user in userRepository.GetAll())
            {
                if (user.Username.Equals(username, StringComparison.OrdinalIgnoreCase))
                {
                    return user;
                }
            }

            return null;
        }

        /// <summary>
        /// ID-р хэрэглэгч хайж олно.
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>Олдвол User, эс бөгөөс null</returns>
        public User GetById(Guid id)
        {
            foreach (var user in userRepository.GetAll())
            {
                if (user.Id == id)
                {
                    return user;
                }
            }

            return null;
        }

        /// <summary>
        /// Username эсвэл DisplayName-аар хэрэглэгч хайна.
        /// </summary>
        /// <param name="keyword">Хайх түлхүүр үг</param>
        /// <returns>Таарах хэрэглэгчдийн жагсаалт</returns>
        public List<User> SearchUsers(string keyword)
        {
            keyword = keyword ?? "";

            return userRepository.GetAll()
                .Where(u =>
                    u.Username.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    u.DisplayName.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();
        }
    }
}