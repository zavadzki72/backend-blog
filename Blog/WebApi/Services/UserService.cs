using MongoDB.Driver;
using WebApi.Data;
using WebApi.Dtos;
using WebApi.Dtos.Responses;
using WebApi.Helpers;

namespace WebApi.Services
{
    public class UserService(MongoContext context, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, FileService fileService)
    {
        private readonly MongoContext _context = context;
        private readonly IConfiguration _configuration = configuration;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly FileService _fileService = fileService;

        public async Task<List<UserResponse>> GetAll()
        {
            return await _context.Users
                .Find(_ => true)
                .Project(x => new UserResponse
                {
                    Id = x.Id,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                    Name = x.Name,
                    Description = x.Description,
                    PictureUrl = x.PictureUrl,
                    SiteUrl = x.SiteUrl
                })
                .ToListAsync();
        }

        public async Task<string> GetToken(LoginDto request)
        {
            var user = await _context.Users.Find(x => x.Email == request.Email).FirstOrDefaultAsync();

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
                throw new UnauthorizedAccessException();

            var token = JwtHelper.GenerateToken(user.Email, _configuration);
            return token;
        }

        public async Task<UserResponse> GetLoggedUser()
        {
            var loggerUserEmail = _httpContextAccessor.HttpContext?.User?.Identity?.Name;
            var user = await _context.Users
                .Find(x => x.Email == loggerUserEmail)
                .Project(x => new UserResponse
                    {
                        Id = x.Id,
                        CreatedAt = x.CreatedAt,
                        UpdatedAt = x.UpdatedAt,
                        Name = x.Name,
                        Description = x.Description,
                        PictureUrl = x.PictureUrl,
                        SiteUrl = x.SiteUrl
                    })
                .FirstOrDefaultAsync()
                    ?? throw new ArgumentException("Usuario nao encontrado.");

            return user;
        }

        public async Task Update(Guid id, UpdateUserRequest request)
        {
            var user = await _context.Users.Find(x => x.Id == id).FirstOrDefaultAsync()
                ?? throw new ArgumentException("A Categoria informada nao foi encontrada.");

            if (string.IsNullOrWhiteSpace(request.PictureUrl) && !string.IsNullOrWhiteSpace(user.PictureUrl))
                await _fileService.DeleteAsync(user.PictureUrl);

            user.Update(request.Name, request.Description, request.SiteUrl, request.PictureUrl);

            await _context.Users.ReplaceOneAsync(x => x.Id == id, user);
        }
    }
}
