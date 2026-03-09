using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Core.Entities
{
    public class User : BaseEntity
    {
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public byte Age { get; }
        public string PasswordHash { get; }
        public string Bio { get; set; }
        public List<Guid> Followers { get; } = new List<Guid>();
        public List<Guid> Following { get; } = new List<Guid>();
        public User(string username, string displayName, byte age, string passwordHash)
        {
            Username = username;
            DisplayName = displayName;
            Age = age;
            PasswordHash = passwordHash;
        }
    }
}
