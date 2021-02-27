using Conduit.Models.Exceptions;
using Conduit.Models.Responses;
using Conduit.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Conduit.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IAccountRepository AccountRepo;
        private ILogger<UserController> Logger;

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> CurrentUserAsync()
        {
            try
            {
                User user = await AccountRepo.GetCurrentUserAsync();
                return Ok(new { user });
            }
            catch (InvalidCredentialsException e)
            {
                Logger.LogWarning(e.Message, e);
                return StatusCode(422, e.ToDictionary());
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message, ex);
                return StatusCode(500, ex.Message);
            }
        }

        public UserController(IAccountRepository accountRepo, ILogger<UserController> logger)
        {
            AccountRepo = accountRepo;
            Logger = logger;
        }
    }
}
