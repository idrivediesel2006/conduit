using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Conduit.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        private ILogger<ArticlesController> Logger;

        public ArticlesController(ILogger<ArticlesController> logger)
        {
            Logger = logger;
        }
    }
}
