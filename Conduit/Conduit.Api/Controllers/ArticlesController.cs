using Conduit.Models.Exceptions;
using Conduit.Models.Requests;
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
    public class ArticlesController : ControllerBase
    {
        private IArticlesRepository ArticlesRepo;
        private ILogger<ArticlesController> Logger;

        [HttpGet]
        public async Task<IActionResult> GetArticlesAsync(
            [FromQuery] string tag,
            [FromQuery] string author,
            [FromQuery] string favorited,
            [FromQuery] int limit = 20,
            [FromQuery] int offset = 0
            )
        {
            try
            {
                Article[] articles = await ArticlesRepo.GetArticlesAsync(tag, author, favorited, limit, offset);
                return Ok(new { articles, articlesCount = articles.Length });
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message, e);
                return StatusCode(500, e);
            }
        }

        [HttpGet("feed")]
        public async Task<IActionResult> GetArticleFeedAsync([FromQuery] int limit = 20, int offset = 0)
        {
            try
            {
                Article[] articles = await ArticlesRepo.GetArticlesFeedAsync(limit, offset);
                return Ok(new { articles, articlesCount = articles.Length });
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message, e);
                return StatusCode(500, e);
            }
        }

        [HttpGet("{slug}")]
        public async Task<IActionResult> GetArticleBySlugAsync([FromRoute] string slug)
        {
            try
            {
                Article article = await ArticlesRepo.GetArticleBySlugAsync(slug);
                return Ok(new { article });
            }
            catch (ArticleNotFoundException e)
            {
                Logger.LogWarning(e.Message, e);
                return StatusCode(422, new { error = e.ToDictionary() });
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message, e);
                return StatusCode(500, e);
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateArticleAsync([FromBody] ArticleRequest<ArticleCreateRequest> req)
        {
            try
            {
                Article article = await ArticlesRepo.CreateArticleAsync(req.Article);
                return Ok(new { article });
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message, e);
                return StatusCode(500, e);
            }
        }

        [HttpPut("{slug}")]
        [Authorize]
        public async Task<IActionResult> UpdateArticleAsync(
            [FromRoute] string slug,
            [FromBody] ArticleRequest<ArticleUpdateRequest> req
            )
        {
            try
            {
                Article article = await ArticlesRepo.UpdateArticleAsync(slug, req.Article);
                return Ok(new { article });
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message, e);
                return StatusCode(500, e);
            }
        }

        [HttpDelete("{slug}")]
        [Authorize]
        public async Task<IActionResult> DeleteArticleAsync([FromRoute] string slug)
        {
            try
            {
                await ArticlesRepo.DeleteArticleAsync(slug);
                return NoContent();
            }
            catch (ArticleNotFoundException e)
            {
                Logger.LogWarning(e.Message, e);
                return StatusCode(422, new { error = e.ToDictionary() });
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message, e);
                return StatusCode(500, e);
            }
        }

        [HttpPost("{slug}/comments")]
        [Authorize]
        public async Task<IActionResult> AddCommentToArticleAsync(
            [FromRoute] string slug,
            [FromBody] CommentRequest<CommentAddRequest> req)
        {
            try
            {
                Comment comment = await ArticlesRepo.AddCommenttoArticleAsync(slug, req.Comment);
                return Ok(new { comment });
            }
            catch (ArticleNotFoundException e)
            {
                Logger.LogWarning(e.Message, e);
                return StatusCode(422, new { error = e.ToDictionary() });
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message, e);
                return StatusCode(500, e);
            }
        }

        [HttpGet("{slug}/comments")]
        public async Task<IActionResult> GetCommentsFromArticleAsync([FromRoute] string slug)
        {
            try
            {
                Comment[] comments = await ArticlesRepo.GetCommentsFromArticleAsync(slug);
                return Ok(new { comments });
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message, e);
                return StatusCode(500, e);
            }
        }

        [HttpDelete("{slug}/comments/{id:int}")]
        [Authorize]
        public async Task<IActionResult> DeleteCommentsFromArticleAsync(
            [FromRoute] string slug,
            [FromRoute] int id)
        {
            try
            {
                await ArticlesRepo.DeleteCommentForArticleAsync(slug, id);
                return NoContent();
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message, e);
                return StatusCode(500, e);
            }
        }

        [HttpPost("{slug}/favorite")]
        [Authorize]
        public async Task<IActionResult> FavoriteArticleAsync([FromRoute] string slug)
        {
            try
            {
                Article article = await ArticlesRepo.FavoriteArticleAsync(slug);
                return Ok(new { article });
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message, e);
                return StatusCode(500, e);
            }
        }

        [HttpDelete("{slug}/favorite")]
        [Authorize]
        public async Task<IActionResult> UnfavoriteArticleAsync([FromRoute] string slug)
        {
            try
            {
                Article article = await ArticlesRepo.UnfavoriteArticleAsync(slug);
                return Ok(new { article });
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message, e);
                return StatusCode(500, e);
            }
        }

        public ArticlesController(IArticlesRepository articlesRepo, ILogger<ArticlesController> logger)
        {
            ArticlesRepo = articlesRepo;
            Logger = logger;
        }
    }
}
