using System.Collections.Generic;
using System.Linq;

namespace DemoApi.Models
{
    public class UserActions : IUserActions
    {
        List<User> users;
        public UserActions()
        {
            users = new List<User>
            {
                new User{Username = "User1",Password="Pass@1"},
                new User{Username = "ApiUser",Password="User@123"},
                new User{Username = "AuthUser",Password="User@111"},
            };
        }
        public bool IsValidUser(User user)
        {
            return users.Any(x => x.Username == user.Username && x.Password == user.Password);  
        }
    }
}
