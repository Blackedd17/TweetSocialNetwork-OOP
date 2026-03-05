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
        public User(string username, string displayName, byte age)
        {
            Username = username;
            DisplayName = displayName;
            Age = age;
        }
    }
}
