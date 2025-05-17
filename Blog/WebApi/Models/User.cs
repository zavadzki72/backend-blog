using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebApi.Models
{
    public class User
    {
        public User()
        {
            Name = string.Empty;
            Description = string.Empty;
            Email = string.Empty;
            Password = string.Empty;
        }

        public User(string name, string description, string email, string password, string? siteUrl = null, string? pictureUrl = null)
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            
            Name = name;
            Description = description;
            Email = email;
            Password = password;

            SiteUrl = siteUrl;
            PictureUrl = pictureUrl;
        }

        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; private set; }

        [BsonElement("created_at")]
        public DateTime CreatedAt { get; private set; }

        [BsonElement("updated_at")]
        public DateTime UpdatedAt { get; private set; }

        [BsonElement("name")]
        public string Name { get; private set; }

        [BsonElement("description")]
        public string Description { get; private set; }

        [BsonElement("site_url")]
        public string? SiteUrl { get; private set; }

        [BsonElement("picture_url")]
        public string? PictureUrl { get; private set; }

        [BsonElement("email")]
        public string Email { get; private set; }

        [BsonElement("password")]
        public string Password { get; private set; }

        public void Update(string name, string description, string? siteUrl = null, string? pictureUrl = null)
        {
            Name = name;
            Description = description;
            
            SiteUrl = siteUrl;
            PictureUrl = pictureUrl;
            
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
