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

    }
}
