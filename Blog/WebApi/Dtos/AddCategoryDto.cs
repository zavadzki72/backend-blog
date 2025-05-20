namespace WebApi.Dtos
{
    public record AddCategoryDto
    {
        public required string Name { get; init; }

        public void Validate()
        {
            if (Name.Length < 2)
                throw new ArgumentException("O nome precisa ter no minimo 3 caracters.");
        }
    }
}
