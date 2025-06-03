using MongoDB.Bson;
using MongoDB.Driver;
using WebApi.Data;
using WebApi.Dtos;
using WebApi.Dtos.Responses;
using WebApi.Models;

namespace WebApi.Services
{
    public class PostService(MongoContext context, UserService userService)
    {
        private readonly MongoContext _context = context;
        private readonly UserService _userService = userService;

        public async Task<PostResponse> GetById(Guid id)
        {
            var post = await _context.Posts.Find(x => x.Id == id).FirstOrDefaultAsync()
                ?? throw new ArgumentException("O Post com o id informado não foi encontrado");

            var categories = await _context.Categories.Find(_ => true).ToListAsync();
            var user = await _context.Users.Find(x => x.Id == post.UserId).FirstOrDefaultAsync()
                ?? throw new ArgumentException("O Usuario do Post informado nao foi encontrado.");

            var postResponse = new PostResponse
            {
                Id = post.Id,
                CreatedAt = post.CreatedAt,
                UpdatedAt = post.UpdatedAt,
                Title = post.Title,
                TitleEnglish = post.TitleEnglish,
                SubTitle = post.SubTitle,
                SubTitleEnglish = post.SubTitleEnglish,
                Content = post.Content,
                ContentEnglish = post.ContentEnglish,
                CoverImageUrl = post.CoverImageUrl,
                Views = post.Views,
                UpVotes = post.UpVotes,
                User = new UserResponse
                {
                    Id = user.Id,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt,
                    Name = user.Name,
                    Description = user.Description,
                    PictureUrl = user.PictureUrl,
                    SiteUrl = user.SiteUrl
                },
                Tags = post.Tags,
                Categories = categories.Where(x => post.Categories.Contains(x.Id)).ToDictionary(x => x.Id, x => x.Name)
            };

            return postResponse;
        }

        public async Task<PaginatedPostResponse> GetPaginatedPostsAsync(GetPostPaginatedDto request)
        {
            request.Validate();

            var filterBuilder = Builders<Post>.Filter;
            var filter = filterBuilder.Empty;

            if (request.Titles.Count != 0)
            {
                var titleFilters = request.Titles
                    .Select(title => filterBuilder.Regex(p => p.Title, new BsonRegularExpression(title, "i")))
                    .ToList();

                filter &= filterBuilder.Or(titleFilters);
            }

            if (request.UsersId.Count != 0)
            {
                filter &= filterBuilder.In(p => p.UserId, request.UsersId);
            }

            if (request.Categories.Count != 0)
            {
                filter &= filterBuilder.ElemMatch(p => p.Categories, c => request.Categories.Contains(c));
            }

            if (request.Tags.Count != 0)
            {
                filter &= filterBuilder.AnyIn(p => p.Tags, request.Tags);
            }

            if (request.MinCreatedAt.HasValue)
            {
                filter &= filterBuilder.Gte(p => p.CreatedAt, request.MinCreatedAt.Value);
            }

            if (request.MaxCreatedAt.HasValue)
            {
                filter &= filterBuilder.Lte(p => p.CreatedAt, request.MaxCreatedAt.Value);
            }

            var sortBuilder = Builders<Post>.Sort;
            SortDefinition<Post> sort = request.OrderType switch
            {
                GetPostPaginatedOrder.Recents => sortBuilder.Descending(p => p.CreatedAt),
                GetPostPaginatedOrder.MostViews => sortBuilder.Descending(p => p.Views),
                GetPostPaginatedOrder.MostVoteds => sortBuilder.Descending(p => p.UpVotes),
                _ => sortBuilder.Descending(p => p.CreatedAt)
            };

            var skip = (request.Page - 1) * request.Size;

            var posts = await _context.Posts
                .Find(filter)
                .Sort(sort)
                .Skip(skip)
                .Limit(request.Size)
                .ToListAsync();

            var categoryIds = posts.SelectMany(p => p.Categories).Distinct().ToList();
            var categories = await _context.Categories
                .Find(c => categoryIds.Contains(c.Id))
                .ToListAsync();
            var categoryDict = categories.ToDictionary(c => c.Id, c => c.Name);

            var users = await _context.Users.Find(x => posts.Select(x => x.UserId).Contains(x.Id)).ToListAsync();

            var data = posts.Select(p => {
                var user = users.FirstOrDefault(x => x.Id == p.UserId)
                    ?? throw new ArgumentException("O Usuario do Post informado nao foi encontrado.");

                return new PostResponse
                {
                    Id = p.Id,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt,
                    Title = p.Title,
                    TitleEnglish = p.TitleEnglish,
                    SubTitle = p.SubTitle,
                    SubTitleEnglish = p.SubTitleEnglish,
                    Content = p.Content,
                    ContentEnglish = p.ContentEnglish,
                    User = new UserResponse
                    {
                        Id = user.Id,
                        CreatedAt = user.CreatedAt,
                        UpdatedAt = user.UpdatedAt,
                        Name = user.Name,
                        Description = user.Description,
                        PictureUrl = user.PictureUrl,
                        SiteUrl = user.SiteUrl
                    },
                    CoverImageUrl = p.CoverImageUrl,
                    Categories = p.Categories.Where(id => categoryDict.ContainsKey(id)).ToDictionary(id => id, id => categoryDict[id]),
                    Tags = p.Tags,
                    Views = p.Views,
                    UpVotes = p.UpVotes
                };
            }).ToList();

            var totalItems = await _context.Posts.CountDocumentsAsync(filter);
            return new PaginatedPostResponse(data, (int)totalItems, request.Page, request.Size);
        }

        public async Task CreatePost(AddPostDto request)
        {
            request.Validate();

            var categories = await _context.Categories.Find(x => request.Categories.Contains(x.Id)).ToListAsync();
            if(categories.Count != request.Categories.Count)
                throw new ArgumentException("Categorias invalidas.");

            var user = await _userService.GetLoggedUser();
            var post = new Post(request.Title, request.TitleEnglish, request.SubTitle, request.SubTitleEnglish, request.Content, request.ContentEnglish, user.Id, request.CoverImageUrl, categories, request.Tags);

            await _context.Posts.InsertOneAsync(post);
        }

        public async Task UpdatePost(Guid id, UpdatePostDto request)
        {
            request.Validate();

            var post = await _context.Posts.Find(x => x.Id == id).FirstOrDefaultAsync()
                ?? throw new ArgumentException("O Post informado nao foi encontrado.");

            var categories = await _context.Categories.Find(x => request.Categories.Contains(x.Id)).ToListAsync();
            if (categories.Count != request.Categories.Count)
                throw new ArgumentException("Categorias invalidas.");

            post.Update(request.Title, request.TitleEnglish, request.SubTitle, request.SubTitleEnglish, request.Content, request.ContentEnglish, request.CoverImageUrl, categories, request.Tags);

            await _context.Posts.ReplaceOneAsync(x => x.Id == id, post);
        }

        public async Task RemovePost(Guid id)
        {
            var result = await _context.Posts.DeleteOneAsync(x => x.Id == id);

            if (result.DeletedCount == 0)
                throw new ArgumentException("O Post informado nao foi encontrado.");

        }

        public async Task ArchivePost(Guid id)
        {
            var post = await _context.Posts.Find(x => x.Id == id).FirstOrDefaultAsync()
                ?? throw new ArgumentException("O Post informado nao foi encontrado.");

            post.Archive();

            await _context.Posts.ReplaceOneAsync(x => x.Id == id, post);
        }

        public async Task UpVote(Guid id)
        {
            var post = await _context.Posts.Find(x => x.Id == id).FirstOrDefaultAsync()
                ?? throw new ArgumentException("O Post informado nao foi encontrado.");

            post.UpVote();

            await _context.Posts.ReplaceOneAsync(x => x.Id == id, post);
        }

        public async Task View(Guid id)
        {
            var post = await _context.Posts.Find(x => x.Id == id).FirstOrDefaultAsync()
                ?? throw new ArgumentException("O Post informado nao foi encontrado.");

            post.View();

            await _context.Posts.ReplaceOneAsync(x => x.Id == id, post);
        }

        public async Task ReactivatePost(Guid id)
        {
            var post = await _context.Posts.Find(x => x.Id == id).FirstOrDefaultAsync()
                ?? throw new ArgumentException("O Post informado nao foi encontrado.");

            post.Reactivate();

            await _context.Posts.ReplaceOneAsync(x => x.Id == id, post);
        }
    }
}
