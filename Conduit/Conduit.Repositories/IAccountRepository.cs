using Conduit.Models.Requests;
using Conduit.Models.Responses;
using System.Threading.Tasks;

namespace Conduit.Repositories
{
    public interface IAccountRepository
    {
        Task<User> RegisterUserAsync(Register register);
        Task<User> LoginAsync(Login user);
    }
}