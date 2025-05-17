namespace WebApi.Dtos.Responses
{
    public record UserResponse
    {
        public Guid Id { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime UpdatedAt { get; init; }
        public required string Name { get; init; }
        public required string Description { get; init; }
        public string? SiteUrl { get; init; }
        public string? PictureUrl { get; init; }
    }
}
