using Conduit.Models.Responses;
using System.Threading.Tasks;

namespace Conduit.Repositories
{
    public interface IProfileRepository
    {
        Task<UserProfile> GetProfileAsync(string userName);
        Task<UserProfile> FollowAsync(string userName);
        Task<UserProfile> UnfollowAsync(string userName);
    }
}
