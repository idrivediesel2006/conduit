using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Conduit.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private ILogger<UserController> Logger;

        public UserController(ILogger<UserController> logger)
        {
            Logger = logger;
        }
    }
}
