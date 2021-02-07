using Conduit.Models.Requests;
using Conduit.Models.Responses;
using Conduit.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Conduit.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private IAccountRepository AccountRepo;
        private ILogger<UsersController> Logger;

        [HttpPost]
        public IActionResult RegisterUser([FromBody]UserRequest<Register> req)
        {
            User user = AccountRepo.RegisterUser(req.User);
            return Ok(new { user });
        }

        public UsersController(IAccountRepository accountRepo, ILogger<UsersController> logger)
        {
            AccountRepo = accountRepo;
            Logger = logger;
        }
    }
}
