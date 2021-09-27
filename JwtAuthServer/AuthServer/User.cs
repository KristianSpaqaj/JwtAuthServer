using System;
using System.Collections.Generic;

namespace AuthServer
{
    public class User
    {
        public Guid ID { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }
    }

    public static class UserStorage
    {
        public static List<User> Users { get; set; } = new List<User> {
            new User {ID=Guid.NewGuid(),Username="user1",Password = "pass1" },
            new User {ID=Guid.NewGuid(),Username="user2",Password = "pass2" },
            new User {ID=Guid.NewGuid(),Username="user3",Password = "pass3" }
        };
    }
}
