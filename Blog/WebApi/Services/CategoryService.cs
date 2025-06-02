using MongoDB.Driver;
using WebApi.Data;
using WebApi.Dtos;
using WebApi.Dtos.Responses;
using WebApi.Models;

namespace WebApi.Services
{
    public class CategoryService(MongoContext context)
    {
        private readonly MongoContext _context = context;

        public async Task<List<CategoryResponse>> GetAll()
        {
            var categories = await _context.Categories
                .Find(_ => true)
                .Project(x => new CategoryResponse
                {
                    Id = x.Id,
                    Name = x.Name,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                    PostQuantity = 0
                })
                .ToListAsync();

            foreach (var category in categories)
            {
                category.PostQuantity = (int)await _context.Posts.CountDocumentsAsync(p => p.Categories.Contains(category.Id));
            }

            return categories;
        }

        public async Task Add(AddCategoryDto request)
        {
            var category = new Category(request.Name);
            await _context.Categories.InsertOneAsync(category);
        }

        public async Task Update(Guid id, AddCategoryDto request)
        {
            var category = await _context.Categories.Find(x => x.Id == id).FirstOrDefaultAsync()
                ?? throw new ArgumentException("A Categoria informada nao foi encontrada.");

            category.Update(request.Name);

            await _context.Categories.ReplaceOneAsync(x => x.Id == id, category);
        }

        public async Task Delete(Guid id)
        {
            var result = await _context.Categories.DeleteOneAsync(x => x.Id == id);

            if (result.DeletedCount == 0)
                throw new ArgumentException("A Categoria informada nao foi encontrada.");
        }
    }
}
