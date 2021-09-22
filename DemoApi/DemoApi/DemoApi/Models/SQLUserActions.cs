using System.Linq;

namespace DemoApi.Models
{
    public class SQLUserActions : IUserActions
    {
        private readonly ApiDbContext context;

        public SQLUserActions(ApiDbContext context)
        {
            this.context = context;
        }

        public bool IsValidUser(User user)
        {
            return context.Users.Any(x => x.Username == user.Username && x.Password == user.Password);
        }
    }
}
