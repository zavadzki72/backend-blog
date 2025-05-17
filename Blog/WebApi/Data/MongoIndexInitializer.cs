using MongoDB.Driver;
using WebApi.Models;

namespace WebApi.Data
{
    public static class MongoIndexInitializer
    {
        public static async Task CreateIndexesAsync(MongoContext context)
        {
            var postIndexModels = new List<CreateIndexModel<Post>>
            {
                new(Builders<Post>.IndexKeys.Descending(p => p.CreatedAt)),
                new(Builders<Post>.IndexKeys.Descending(p => p.Views)),
                new(Builders<Post>.IndexKeys.Descending(p => p.UpVotes)),
                new(Builders<Post>.IndexKeys.Ascending(p => p.UserId).Descending(p => p.CreatedAt)),
                new(Builders<Post>.IndexKeys.Ascending(p => p.Categories)),
                new(Builders<Post>.IndexKeys.Ascending(p => p.Tags)),
                new(Builders<Post>.IndexKeys.Text(p => p.Title)),
            };

            await context.Posts.Indexes.CreateManyAsync(postIndexModels);
        }
    }

}
