using AutoMapper;
using Conduit.Data;
using Conduit.Models.Exceptions;
using Conduit.Models.Requests;
using Conduit.Models.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Conduit.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private IHttpContextAccessor HttpContextAccessor;
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
            if (HttpContextAccessor.HttpContext.Request.Headers.ContainsKey("Authorization"))
            {
                user.Token = HttpContextAccessor
                                .HttpContext
                                .Request
                                .Headers["Authorization"]
                                .ToString()
                                .Split(" ")[1];
            }
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

        private bool VerifyPassword(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i])
                    {
                        return false;
                    }
                }
                return true;
            }
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

        public async Task<User> LoginAsync(Login login)
        {
            Account account = await Context
                                        .Accounts
                                        .Where(x => x.Email == login.Email)
                                        .Include(p => p.Person)
                                        .FirstOrDefaultAsync().ConfigureAwait(false);
            if (account is null)
            {
                throw new LoginFailedException("Login Failed - email not found.");
            }
            if (!VerifyPassword(login.Password, account.PasswordHash, account.PasswordSalt))
            {
                throw new LoginFailedException("Login Failed - password is incorrect.");
            }
            User user = CreateUser(account);
            user.Token = CreateToken(user);
            return user;
        }

        public async Task<User> GetCurrentUserAsync()
        {
            Account account = await GetLoggedInUser();
            if (account is null)
            {
                throw new InvalidCredentialsException("Invalid user - Please login using a valid email & password.");
            }
            return CreateUser(account);
        }

        public async Task<Account> GetLoggedInUser()
        {
            return await Context
                            .Accounts
                            .Where(u => u.Email == HttpContextAccessor.HttpContext.User.Identity.Name)
                            .Include(p => p.Person)
                            .FirstOrDefaultAsync().ConfigureAwait(false);
        }

        public AccountRepository(IHttpContextAccessor httpContextAccessor, ConduitContext context, IMapper mapper, IConfiguration configuration, ILogger<AccountRepository> logger)
        {
            HttpContextAccessor = httpContextAccessor;
            Context = context;
            Mapper = mapper;
            Configuration = configuration;
            Logger = logger;
        }
    }
}
