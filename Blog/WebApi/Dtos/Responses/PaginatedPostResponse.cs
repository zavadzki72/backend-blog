namespace WebApi.Dtos.Responses
{
    public record PaginatedPostResponse
    {
        public PaginatedPostResponse(List<PostResponse> data, int totalItems, int currentPage, int sizePage)
        {
            Data = data;
            TotalItems = totalItems;
            CurrentPage = currentPage;
            SizePage = sizePage;

            TotalPages = (int)Math.Ceiling((double)totalItems / sizePage);
            TotalItemsPage = data.Count;
        }

        public List<PostResponse> Data { get; private set; }
        public int TotalItems { get; private set; }
        public int CurrentPage { get; private set; }
        public int SizePage { get; private set; }
        public int TotalPages { get; private set; }
        public int TotalItemsPage { get; private set; }
    }

    public record PostResponse
    {
        public required Guid Id { get; init; }
        public required DateTime CreatedAt { get; init; }
        public required DateTime UpdatedAt { get; init; }
        public required string Title { get; init; }
        public required string TitleEnglish { get; init; }
        public required string SubTitle { get;   init; }
        public required string SubTitleEnglish { get;   init; }
        public required string Content { get; init; }
        public required string ContentEnglish { get; init; }
        public required UserResponse User { get; init; }
        public required string CoverImageUrl { get; init; }
        public required Dictionary<Guid, string> Categories { get; init; }
        public required List<string> Tags { get; init; }
        public required int Views { get; init; }
        public required int UpVotes { get; init; }
    }
}
