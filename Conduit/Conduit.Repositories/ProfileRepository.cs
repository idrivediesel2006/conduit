using AutoMapper;
using Conduit.Data;
using Conduit.Models.Exceptions;
using Conduit.Models.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Conduit.Repositories
{
    public class ProfileRepository : IProfileRepository
    {
        private IAccountRepository AccountRepo;
        private ConduitContext Context;
        private IMapper Mapper;
        private ILogger<ProfileRepository> Logger;

        private async Task<Person> GetPersonAndFollowersAsync(string userName)
        {
            return await Context
                            .People
                            .Where(p => p.UserName == userName)
                            .Include(p => p.FollowingNavigations)
                            .FirstOrDefaultAsync();
        }

        private async Task<UserProfile> CreateProfileAsync(Person person)
        {
            int loggedInUserId = 0;
            Account loggedInUser = await AccountRepo.GetLoggedInUserAsync();
            if (loggedInUser is not null)
            {
                loggedInUserId = loggedInUser.Id;
            }
            UserProfile userProfile = new UserProfile
            {
                Following = person.FollowingNavigations.Any(u => u.Follower == loggedInUserId)
            };
            Mapper.Map(person, userProfile);
            return userProfile;
        }

        public async Task<UserProfile> GetProfileAsync(string userName)
        {
            Person person = await GetPersonAndFollowersAsync(userName);
            if (person is null)
            {
                throw new ProfileNotFoundException($"Profile for [{userName}] is not found");
            }
            UserProfile userProfile = await CreateProfileAsync(person);
            return userProfile;
        }

        public async Task<UserProfile> FollowAsync(string userName)
        {
            Person person = await GetPersonAndFollowersAsync(userName);
            if (person is null)
            {
                throw new ProfileNotFoundException($"Profile for [{userName}] is not found");
            }
            Account loggedInUser = await AccountRepo.GetLoggedInUserAsync();
            UserProfile userProfile = await CreateProfileAsync(person);
            if (!userProfile.Following)
            {
                Follow follow = new Follow { Follower = loggedInUser.Id, Following = person.Id };
                Context.Follows.Add(follow);
                await Context.SaveChangesAsync();
                userProfile.Following = true;
            }
            return userProfile;
        }

        public async Task<UserProfile> UnfollowAsync(string userName)
        {
            Person person = await GetPersonAndFollowersAsync(userName);
            if (person is null)
            {
                throw new ProfileNotFoundException($"Profile for [{userName}] is not found");
            }
            Account loggedInUser = await AccountRepo.GetLoggedInUserAsync();
            UserProfile userProfile = await CreateProfileAsync(person);
            if (userProfile.Following)
            {
                Follow follow = await Context.Follows.FindAsync(loggedInUser.Id, person.Id);
                Context.Follows.Remove(follow);
                await Context.SaveChangesAsync();
                userProfile.Following = false;
            }
            return userProfile;
        }

        public async Task<bool> IsFollowing(string userName)
        {
            Person person = await GetPersonAndFollowersAsync(userName);
            Account account = await AccountRepo.GetLoggedInUserAsync();
            return person.FollowingNavigations.Any(p => p.Follower == (account is null ? 0 : account.Id));
        }

        public ProfileRepository(
            IAccountRepository accountRepo, 
            ConduitContext context, 
            IMapper mapper, 
            ILogger<ProfileRepository> logger)
        {
            AccountRepo = accountRepo;
            Context = context;
            Mapper = mapper;
            Logger = logger;
        }
    }
}
