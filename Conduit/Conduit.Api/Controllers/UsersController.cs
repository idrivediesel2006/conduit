using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Conduit.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private ILogger<UsersController> Logger;

        public UsersController(ILogger<UsersController> logger)
        {
            Logger = logger;
        }
    }
}
