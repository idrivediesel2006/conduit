using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Conduit.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagsController : ControllerBase
    {
        private ILogger<TagsController> Logger;

        public TagsController(ILogger<TagsController> logger)
        {
            Logger = logger;
        }
    }
}
