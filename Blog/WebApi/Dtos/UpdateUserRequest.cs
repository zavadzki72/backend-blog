namespace WebApi.Dtos
{
    public record UpdateUserRequest
    {
        public required string Name { get; init; }
        public required string Description { get; init; }
        public string? SiteUrl { get; init; }
        public string? PictureUrl { get; init; }
    }
}
