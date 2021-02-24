using Conduit.Models.Exceptions;
using Conduit.Models.Requests;
using Conduit.Models.Responses;
using Conduit.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Conduit.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private IAccountRepository AccountRepo;
        private ILogger<UsersController> Logger;

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] UserRequest<Login> req)
        {
            try
            {
                User user = await AccountRepo.LoginAsync(req.User).ConfigureAwait(false);
                return Ok(new { user });
            }
            catch (LoginFailedException ex)
            {
                Logger.LogWarning(ex.Message, ex);
                return StatusCode(422, ex.ToDictionary());
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message, e);
                return StatusCode(500, e.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> RegisterUserAsync([FromBody]UserRequest<Register> req)
        {
            try
            {
                User user = await AccountRepo.RegisterUserAsync(req.User);
                return Ok(new { user });
            }
            catch (UserExistException ex)
            {
                Logger.LogWarning(ex.Message, ex);
                return StatusCode(422, new { error = ex.ToDictionary() });
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message, e);
                return StatusCode(500, e.Message);
            }
        }

        public UsersController(IAccountRepository accountRepo, ILogger<UsersController> logger)
        {
            AccountRepo = accountRepo;
            Logger = logger;
        }
    }
}
