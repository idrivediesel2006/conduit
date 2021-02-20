using AutoMapper;
using Conduit.Data;
using Conduit.Models.Exceptions;
using Conduit.Models.Requests;
using Conduit.Models.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Conduit.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private ConduitContext Context;
        private IMapper Mapper;
        private IConfiguration Configuration;
        private ILogger<AccountRepository> Logger;

        private async Task<Account> CreateAccountAsync(Register register)
        {
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(register.Password, out passwordHash, out passwordSalt);
            var result = await Context.Accounts.AddAsync(new Account
            {
                Email = register.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Person = new Person
                {
                    UserName = register.UserName
                }
            }).ConfigureAwait(false);
            await Context.SaveChangesAsync().ConfigureAwait(false);
            return result.Entity;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        private async Task<bool> UserExistAsync(Register register)
        {
            var result = await Context
                                .Accounts
                                .AnyAsync(e => e.Email == register.Email || e.Person.UserName == register.UserName)
                                .ConfigureAwait(false);
            return result;
        }

        private User CreateUser(Account account)
        {
            User user = new User();
            Mapper.Map(account, user);
            Mapper.Map(account.Person, user);
            return user;
        }

        private string CreateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Configuration.GetSection("AppSettings:Token").Value);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserName),
                    new Claim(ClaimTypes.Name, user.Email)
                }),
                NotBefore = DateTime.Now,
                Expires = DateTime.Now.AddMinutes(20),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha512Signature
                )
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<User> RegisterUserAsync(Register register)
        {
            bool userExist = await UserExistAsync(register);
            if (userExist)
            {
                throw new UserExistException("The email or user name is already in use.");
            }
            Account account = await CreateAccountAsync(register);
            User user = CreateUser(account);
            user.Token = CreateToken(user);
            return user;
        }

        public AccountRepository(ConduitContext context, IMapper mapper, IConfiguration configuration, ILogger<AccountRepository> logger)
        {
            Context = context;
            Mapper = mapper;
            Configuration = configuration;
            Logger = logger;
        }
    }
}
