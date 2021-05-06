using Conduit.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace Conduit.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagsController : ControllerBase
    {
        private ITagsRepository TagsRepo;
        private ILogger<TagsController> Logger;

        [HttpGet]
        public IActionResult GetTags()
        {
            try
            {
                string[] tags = TagsRepo.GetTags();
                return Ok(new { tags });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message, ex);
                return StatusCode(500, ex.Message);
            }
        }

        public TagsController(ITagsRepository tagsRepo, ILogger<TagsController> logger)
        {
            TagsRepo = tagsRepo;
            Logger = logger;
        }
    }
}
