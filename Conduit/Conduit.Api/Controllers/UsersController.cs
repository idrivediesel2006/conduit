using Conduit.Models.Requests;
using Conduit.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Conduit.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private ILogger<UsersController> Logger;

        [HttpPost]
        public IActionResult RegisterUser([FromBody]UserRequest<Register> req)
        {
            User user = new User
            {
                Bio = "no bio yet",
                Email = req.User.Email,
                Image = "no image yet",
                Token = "no token yet",
                UserName = req.User.UserName
            };
            return Ok(new { user });
        }

        public UsersController(ILogger<UsersController> logger)
        {
            Logger = logger;
        }
    }
}
