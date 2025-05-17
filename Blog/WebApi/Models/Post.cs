using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using WebApi.Enumerators;

namespace WebApi.Models
{
    public class Post
    {
        public Post() 
        {
            Title = string.Empty;
            SubTitle = string.Empty;
            Content = string.Empty;
            CoverImageUrl = string.Empty;
            Tags = [];
            Categories = [];
        }

        public Post(string title, string subTitle, string content, Guid userId, string coverImageUrl, List<Category> categories, List<string> tags)
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;

            Title = title;
            SubTitle = subTitle;
            Content = content;
            CoverImageUrl = coverImageUrl;
            Tags = tags;

            Categories = [.. categories.Select(x => x.Id)];
            UserId = userId;

            Status = PostStatus.Published;
            Views = 1;
            UpVotes = 1;
        }

        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; private set; }

        [BsonElement("created_at")]
        public DateTime CreatedAt { get; private set; }

        [BsonElement("updated_at")]
        public DateTime UpdatedAt { get; private set; }

        [BsonElement("title")]
        public string Title { get; private set; }

        [BsonElement("sub_title")]
        public string SubTitle { get; private set; }

        [BsonElement("content")]
        public string Content { get; private set; }

        [BsonElement("user_id")]
        [BsonRepresentation(BsonType.String)]
        public Guid UserId { get; private set; }

        [BsonElement("status")]
        public PostStatus Status { get; private set; }

        [BsonElement("cover_image_url")]
        public string CoverImageUrl { get; private set; }

        [BsonElement("categories")]
        [BsonRepresentation(BsonType.String)]
        public List<Guid> Categories { get; private set; }

        [BsonElement("tags")]
        public List<string> Tags { get; private set; }

        [BsonElement("views")]
        public int Views { get; private set; }

        [BsonElement("up_votes")]
        public int UpVotes { get; private set; }

        public void Update(string title, string subTitle, string content, string coverImageUrl, List<Category> categories, List<string> tags)
        {
            Title = title;
            SubTitle = subTitle;
            Content = content;
            CoverImageUrl = coverImageUrl;
            Tags = tags;

            Categories = [.. categories.Select(x => x.Id)];

            UpdatedAt = DateTime.UtcNow;
        }

        public void View()
        {
            Views++;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpVote()
        {
            UpVotes++;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Archive()
        {
            Status = PostStatus.Archived;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Reactivate()
        {
            Status = PostStatus.Published;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
