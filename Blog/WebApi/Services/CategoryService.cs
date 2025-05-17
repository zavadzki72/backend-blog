using WebApi.Data;
using WebApi.Models;
using MongoDB.Driver;

namespace WebApi.Services
{
    public class CategoryService(MongoContext context)
    {
        private readonly MongoContext _context = context;

        public async Task<List<Category>> GetAll()
        {
            return await _context.Categories.Find(_ => true).ToListAsync();
        }

        public async Task Add(Category request)
        {
            await _context.Categories.InsertOneAsync(request);
        }

        public async Task Update(Guid id, Category request)
        {
            var category = await _context.Categories.Find(x => x.Id == id).FirstOrDefaultAsync()
                ?? throw new ArgumentException("A Categoria informada nao foi encontrada.");

            category.Update(request.Name);

            await _context.Categories.ReplaceOneAsync(x => x.Id == id, request);
        }

        public async Task Delete(Guid id)
        {
            var result = await _context.Categories.DeleteOneAsync(x => x.Id == id);

            if (result.DeletedCount == 0)
                throw new ArgumentException("A Categoria informada nao foi encontrada.");
        }
    }
}
