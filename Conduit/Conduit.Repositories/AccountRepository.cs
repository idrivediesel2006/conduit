using Conduit.Data;
using Conduit.Models.Exceptions;
using Conduit.Models.Requests;
using Conduit.Models.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Conduit.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private ConduitContext Context;
        private ILogger<AccountRepository> Logger;

        private async Task<bool> UserExistAsync(Register register)
        {
            var result = await Context
                                .Accounts
                                .AnyAsync(e => e.Email == register.Email || e.Person.UserName == register.UserName)
                                .ConfigureAwait(false);
            return result;
        }

        public async Task<User> RegisterUserAsync(Register register)
        {
            bool userExist = await UserExistAsync(register);
            if (userExist)
            {
                throw new UserExistException("The email or user name is already in use.");
            }
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

        public AccountRepository(ConduitContext context, ILogger<AccountRepository> logger)
        {
            Context = context;
            Logger = logger;
        }
    }
}
