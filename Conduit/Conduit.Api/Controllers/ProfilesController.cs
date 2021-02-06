using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Conduit.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfilesController : ControllerBase
    {
        private ILogger<ProfilesController> Logger;

        public ProfilesController(ILogger<ProfilesController> logger)
        {
            Logger = logger;
        }
    }
}
