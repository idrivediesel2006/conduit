using AutoMapper;
using Conduit.Data;
using Conduit.Models.Exceptions;
using Conduit.Models.Requests;
using Conduit.Models.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Conduit.Repositories
{
    public class ArticlesRepository : IArticlesRepository
    {
        private IAccountRepository AccountRepo;
        private IProfileRepository ProfileRepo;
        private ConduitContext Context;
        private IMapper Mapper;
        private ILogger<ArticlesRepository> Logger;

        private async Task<List<Editorial>> GetEditorialsByTagAsync(string tag, int limit, int offset)
        {
            return await Context.Editorials
                            .Where(e => e.Tags.Any(t => t.DisplayName == tag))
                            .Include(f => f.Favorites)
                            .Include(p => p.Person)
                            .Include(t => t.Tags)
                            .OrderByDescending(e => e.CreatedAt)
                            .Skip(offset)
                            .Take(limit)
                            .ToListAsync();
        }

        private async Task<List<Editorial>> GetEditorialsByAuthorAsync(string author, int limit, int offset)
        {
            return await Context.Editorials
                            .Where(e => e.Person.UserName == author)
                            .Include(f => f.Favorites)
                            .Include(p => p.Person)
                            .Include(t => t.Tags)
                            .OrderByDescending(e => e.CreatedAt)
                            .Skip(offset)
                            .Take(limit)
                            .ToListAsync();
        }

        private async Task<List<Editorial>> GetEditorialsByFavoritedAsync(string favorited, int limit, int offset)
        {
            return await Context.Editorials
                            .Where(e => e.Favorites.Any(f => f.Person.UserName == favorited))
                            .Include(f => f.Favorites)
                            .Include(p => p.Person)
                            .Include(t => t.Tags)
                            .OrderByDescending(e => e.CreatedAt)
                            .Skip(offset)
                            .Take(limit)
                            .ToListAsync();
        }

        private async Task<List<Editorial>> GetEditorialsAsync(int limit, int offset)
        {
            return await Context.Editorials
                            .Include(f => f.Favorites)
                            .Include(p => p.Person)
                            .Include(t => t.Tags)
                            .OrderByDescending(e => e.CreatedAt)
                            .Skip(offset)
                            .Take(limit)
                            .ToListAsync();
        }

        private async Task<List<Article>> ArticleListMapper(List<Editorial> editorials)
        {
            List<Article> articles = new List<Article>();
            foreach (var editorial in editorials)
            {
                articles.Add(await ArticleMapper(editorial));
            }
            return articles;
        }

        private async Task<Article> ArticleMapper(Editorial editorial)
        {
            Article article = new Article { Author = new UserProfile() };
            Mapper.Map(editorial, article);
            Mapper.Map(editorial.Person, article.Author);
            article.TagList = (from t in editorial.Tags
                               select t.DisplayName).ToArray();
            article.Favorited = await IsFavorited(editorial);
            article.Author.Following = await ProfileRepo.IsFollowing(editorial.Person.UserName);
            return article;
        }

        private async Task<bool> IsFavorited(Editorial editorial)
        {
            if (editorial.Favorites.Count == 0)
                return false;

            Account account = await AccountRepo.GetLoggedInUserAsync();
            if (account is null)
                return false;

            return editorial.Favorites.Any(f => f.PersonId == account.Id);
        }

        private string CreateSlug(string title)
        {
            var random = new Random();
            const string chars = "abcdefghijklmnopqrstuvwxyz";
            var s = new string(Enumerable.Repeat(chars, 5).Select(s => s[random.Next(s.Length)]).ToArray());
            return $"{title.ToLower().Replace(" ", "-")}-{s}";
        }

        private async Task SaveTags(Editorial entity, string[] tagList)
        {
            foreach (var item in tagList)
            {
                Tag t = await Context.Tags.FirstOrDefaultAsync(d => d.DisplayName == item);
                if (t is null)
                {
                    t = new Tag { DisplayName = item };
                    var entry = await Context.Tags.AddAsync(t);
                    entity.Tags.Add(entry.Entity);
                }
                else
                {
                    entity.Tags.Add(t);
                }
                await Context.SaveChangesAsync();
            }
        }

        public async Task<Comment> AddCommenttoArticleAsync(string slug, CommentAddRequest comment)
        {
            Editorial editorial = await Context.Editorials.FirstOrDefaultAsync(e => e.Slug == slug);
            if (editorial is null)
                throw new ArticleNotFoundException($"the slug [{slug}] was not found.");
            Account account = await AccountRepo.GetLoggedInUserAsync();
            Commentary commentary = new Commentary
            {
                Body = comment.Body,
                PersonId = account.Id,
                EditorialId = editorial.ID
            };
            var entry = await Context.Commentaries.AddAsync(commentary);
            await Context.SaveChangesAsync();
            Comment retComment = Mapper.Map<Comment>(entry.Entity);
            Mapper.Map(account.Person, retComment.Author);
            return retComment;
        }

        public async Task<Article> CreateArticleAsync(ArticleCreateRequest article)
        {
            Account account = await AccountRepo.GetLoggedInUserAsync();
            Editorial editorial = Mapper.Map<Editorial>(article);
            editorial.PersonId = account.Id;
            editorial.Slug = CreateSlug(article.Title);
            var entry = Context.Editorials.Add(editorial);
            await Context.SaveChangesAsync();
            await SaveTags(entry.Entity, article.TagList);
            return await GetArticleBySlugAsync(entry.Entity.Slug);
        }

        public async Task DeleteArticleAsync(string slug)
        {
            Editorial editorial = await Context.Editorials.FirstOrDefaultAsync(e => e.Slug == slug);
            Account account = await AccountRepo.GetLoggedInUserAsync();
            if (editorial is null ||
                editorial.PersonId != account.Id)
                throw new ArticleNotFoundException($"The article with the slug [{slug}] was not found");
            Context.Editorials.Remove(editorial);
            await Context.SaveChangesAsync();
        }

        public async Task DeleteCommentForArticleAsync(string slug, int id)
        {
            Commentary commentary = await Context.Commentaries
                                        .Include(e => e.Editorial)
                                        .FirstOrDefaultAsync(e => e.Id == id && e.Editorial.Slug == slug);
            Account account = await AccountRepo.GetLoggedInUserAsync();
            if (commentary is not null
                && commentary.PersonId == account.Id)
            {
                Context.Commentaries.Remove(commentary);
                await Context.SaveChangesAsync();
            }
        }

        public async Task<Article> FavoriteArticleAsync(string slug)
        {
            Account account = await AccountRepo.GetLoggedInUserAsync();
            Editorial editorial = await Context.Editorials
                                            .Include(e => e.Favorites)
                                            .FirstOrDefaultAsync(e => e.Slug == slug);
            if (!await IsFavorited(editorial))
            {
                Favorite favorite = new Favorite { EditorialId = editorial.ID, PersonId = account.Id };
                await Context.Favorites.AddAsync(favorite);
                await Context.SaveChangesAsync();
            }
            return await GetArticleBySlugAsync(slug);
        }

        public async Task<Article> GetArticleBySlugAsync(string slug)
        {
            Editorial editorial = await Context.Editorials
                                    .Include(f => f.Favorites)
                                    .Include(p => p.Person)
                                    .Include(t => t.Tags)
                                    .FirstOrDefaultAsync(a => a.Slug == slug);
            if (editorial is null)
            {
                throw new ArticleNotFoundException($"The slug [{slug}] was not found");
            }
            Article article = await ArticleMapper(editorial);
            return article;
        }

        public async Task<Article[]> GetArticlesAsync(string tag, string author, string favorited, int limit, int offset)
        {
            List<Editorial> editorials = new List<Editorial>();
            if (tag is not null)
            {
                editorials = await GetEditorialsByTagAsync(tag, limit, offset);
            }
            else if (author is not null)
            {
                editorials = await GetEditorialsByAuthorAsync(author, limit, offset);
            }
            else if (favorited is not null)
            {
                editorials = await GetEditorialsByFavoritedAsync(favorited, limit, offset);
            }
            else
            {
                editorials = await GetEditorialsAsync(limit, offset);
            }
            List<Article> articles = await ArticleListMapper(editorials);
            return articles.ToArray();
        }

        public async Task<Article[]> GetArticlesFeedAsync(int limit, int offset)
        {
            Account account = await AccountRepo.GetLoggedInUserAsync();
            var accountId = account?.Id ?? 0;
            var editorials = await Context.Editorials
                                .Where(e => e.Person.FollowingNavigations.Any(f => f.Follower == accountId))
                                .Include(f => f.Favorites)
                                .Include(p => p.Person)
                                .Include(t => t.Tags)
                                .OrderByDescending(e => e.CreatedAt)
                                .Skip(offset)
                                .Take(limit)
                                .ToListAsync();
            List<Article> articles = await ArticleListMapper(editorials);
            return articles.ToArray();
        }

        public async Task<Comment[]> GetCommentsFromArticleAsync(string slug)
        {
            List<Commentary> commentaries = await Context.Commentaries
                                                .Include(p => p.Person)
                                                .Where(c => c.Editorial.Slug == slug)
                                                .ToListAsync();
            List<Comment> comments = new List<Comment>();
            foreach (var item in commentaries)
            {
                Comment comment = Mapper.Map<Comment>(item);
                Mapper.Map(item.Person, comment.Author);
                comment.Author.Following = await ProfileRepo.IsFollowing(item.Person.UserName);
                comments.Add(comment);
            }
            return comments.ToArray();
        }

        public async Task<Article> UnfavoriteArticleAsync(string slug)
        {
            Account account = await AccountRepo.GetLoggedInUserAsync();
            Editorial editorial = await Context.Editorials
                                            .Include(e => e.Favorites)
                                            .FirstOrDefaultAsync(e => e.Slug == slug);
            if (await IsFavorited(editorial))
            {
                Favorite favorite = editorial.Favorites.FirstOrDefault(e => e.PersonId == account.Id);
                Context.Favorites.Remove(favorite);
                await Context.SaveChangesAsync();
            }
            return await GetArticleBySlugAsync(slug);
        }

        public async Task<Article> UpdateArticleAsync(string slug, ArticleUpdateRequest article)
        {
            Editorial editorial = await Context.Editorials.FirstOrDefaultAsync(e => e.Slug == slug);
            if (article.Title is not null && article.Title != editorial.Title)
            {
                editorial.Title = article.Title;
                editorial.Slug = CreateSlug(article.Title);
            }
            if (article.Description is not null && article.Description != editorial.Description)
                editorial.Description = article.Description;
            if (article.Body is not null && article.Body != editorial.Body)
                editorial.Body = article.Body;
            var entry = Context.Update(editorial);
            await Context.SaveChangesAsync();
            return await GetArticleBySlugAsync(entry.Entity.Slug);
        }

        public ArticlesRepository(
            IAccountRepository accountRepo,
            IProfileRepository profileRepo,
            ConduitContext context,
            IMapper mapper,
            ILogger<ArticlesRepository> logger)
        {
            AccountRepo = accountRepo;
            ProfileRepo = profileRepo;
            Context = context;
            Mapper = mapper;
            Logger = logger;
        }
    }
}
