using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Social.Core.Entities;
using Social.Core.Repositories;

namespace Social.Core.Services
{
    public class UserService
    {
        private readonly IRepository<User> userRepository;

        public UserService(IRepository<User> userRepository)
        {
            this.userRepository = userRepository;
        }
        public void CreateUser(User user)
        {
            userRepository.Add(user);
        }
        public List<User> GetUsers()
        {
            return userRepository.GetAll();
        }
        public bool IsFollowing(User follower, User target)
        {
            if (follower == null || target == null) return false;
            return follower.Following.Contains(target.Id);
        }
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
