using Conduit.Models.Requests;
using Conduit.Models.Responses;
using System.Threading.Tasks;

namespace Conduit.Repositories
{
    public interface IArticlesRepository
    {
        Task<Article[]> GetArticlesAsync(string tag, string author, string favorited, int limit, int offset);
        Task<Article> CreateArticleAsync(ArticleCreateRequest article);
        Task<Comment> AddCommenttoArticleAsync(string slug, CommentAddRequest comment);
        Task<Article[]> GetArticlesFeedAsync(int limit, int offset);
        Task<Article> GetArticleBySlugAsync(string slug);
        Task<Article> UpdateArticleAsync(string slug, ArticleUpdateRequest article);
        Task DeleteArticleAsync(string slug);
        Task<Comment[]> GetCommentsFromArticleAsync(string slug);
        Task DeleteCommentForArticleAsync(string slug, int id);
        Task<Article> UnfavoriteArticleAsync(string slug);
        Task<Article> FavoriteArticleAsync(string slug);
    }
}
