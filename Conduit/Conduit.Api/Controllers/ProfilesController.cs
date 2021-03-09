using Conduit.Models.Exceptions;
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
    public class ProfilesController : ControllerBase
    {
        private IProfileRepository ProfileRepo;
        private ILogger<ProfilesController> Logger;

        [HttpGet("{userName}")]
        public async Task<IActionResult> GetProfileAsync([FromRoute]string userName)
        {
            try
            {
                UserProfile userProfile = await ProfileRepo.GetProfileAsync(userName);
                return Ok(new { userProfile });
            }
            catch (ProfileNotFoundException e)
            {
                Logger.LogWarning(e.Message, e);
                return StatusCode(422, e.ToDictionary());
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message, ex);
                return StatusCode(500, ex);
            }
        }

        [HttpPost("{userName}/follow")]
        public async Task<IActionResult> FollowAsync([FromRoute]string userName)
        {
            try
            {
                UserProfile userProfile = await ProfileRepo.FollowAsync(userName);
                return Ok(new { userProfile });
            }
            catch (ProfileNotFoundException e)
            {
                Logger.LogWarning(e.Message, e);
                return StatusCode(422, e.ToDictionary());
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message, ex);
                return StatusCode(500, ex);
            }
        }

        [HttpDelete("{userName}/follow")]
        public async Task<IActionResult> UnfollowAsync([FromRoute] string userName)
        {
            try
            {
                UserProfile userProfile = await ProfileRepo.UnfollowAsync(userName);
                return Ok(new { userProfile });
            }
            catch (ProfileNotFoundException e)
            {
                Logger.LogWarning(e.Message, e);
                return StatusCode(422, e.ToDictionary());
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message, ex);
                return StatusCode(500, ex);
            }
        }

        public ProfilesController(IProfileRepository profileRepo, ILogger<ProfilesController> logger)
        {
            ProfileRepo = profileRepo;
            Logger = logger;
        }
    }
}
