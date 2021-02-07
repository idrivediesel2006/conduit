using Conduit.Models.Requests;
using Conduit.Models.Responses;
using Microsoft.Extensions.Logging;

namespace Conduit.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private ILogger<AccountRepository> Logger;

        public User RegisterUser(Register register)
        {
            User user = new User
            {
                Bio = "no bio yet",
                Email = register.Email,
                Image = "no image yet",
                Token = "no token yet",
                UserName = register.UserName
            };
            return user;
        }

        public AccountRepository(ILogger<AccountRepository> logger)
        {
            Logger = logger;
        }
    }
}
