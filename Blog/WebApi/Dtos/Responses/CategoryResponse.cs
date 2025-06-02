namespace WebApi.Dtos.Responses
{
    public record CategoryResponse
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; init; }
        public DateTime UpdatedAt { get; init; }
        public required string Name { get; init; }
        public int PostQuantity { get; set; }
    }
}
