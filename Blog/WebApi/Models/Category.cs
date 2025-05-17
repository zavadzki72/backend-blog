using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebApi.Models
{
    public class Category
    {
        public Category() 
        {
            Name = string.Empty;
        }

        public Category(string name)
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            Name = name;
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

        public void Update(string name)
        {
            Name = name;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
