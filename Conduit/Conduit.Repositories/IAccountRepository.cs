using Conduit.Models.Requests;
using Conduit.Models.Responses;

namespace Conduit.Repositories
{
    public interface IAccountRepository
    {
        User RegisterUser(Register register);
    }
}