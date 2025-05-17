namespace WebApi.Dtos
{
    public record GetPostPaginatedDto
    {
        public int Page { get; init; } = 1;
        public int Size { get; init; } = 6;

        public DateTime? MinCreatedAt { get; init; }
        public DateTime? MaxCreatedAt { get; init; }
        public List<string> Titles { get; init; } = [];
        public List<Guid> UsersId { get; init; } = [];
        public List<Guid> Categories { get; init; } = [];
        public List<string> Tags { get; init; } = [];

        public GetPostPaginatedOrder OrderType { get; init; } = GetPostPaginatedOrder.Recents;

        public void Validate()
        {
            if (Size > 20 || Size < 1)
                throw new ArgumentException("O tamanho da pagina deve estar entre 1 e 20.");
        }
    }

    public enum GetPostPaginatedOrder
    {
        Recents = 1,
        MostViews = 2,
        MostVoteds = 3
    }
}
