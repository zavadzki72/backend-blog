using System;

namespace WebApi.Dtos
{
    public record UpdatePostDto
    {
        public required string Title { get; init; }
        public required string TitleEnglish { get; init; }
        public required string SubTitle { get; init; }
        public required string SubTitleEnglish { get; init; }
        public required string Content { get; init; }
        public required string ContentEnglish { get; init; }
        public required string CoverImageUrl { get; init; }
        public List<Guid> Categories { get; init; } = [];
        public List<string> Tags { get; init; } = [];

        public void Validate()
        {
            if (Title.Length < 3)
                throw new ArgumentException("O titulo precisa ter no minimo 3 caracters.");

            if (SubTitle.Length < 3)
                throw new ArgumentException("O sub titulo precisa ter no minimo 3 caracters.");

            if (Content.Length < 100)
                throw new ArgumentException("O conteudo precisa ter no minimo 100 caracters.");

            if (!Uri.TryCreate(CoverImageUrl, UriKind.RelativeOrAbsolute, out _))
                throw new ArgumentException("Url de imagem invalida.");

            if(Categories.Count == 0)
                throw new ArgumentException("O POST precisa ter no minimo 1 categoria.");

            if (Tags.Count == 0)
                throw new ArgumentException("O POST precisa ter no minimo 1 tag.");
        }
    }
}
