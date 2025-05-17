using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebApi.Data;
using WebApi.Middlewares;
using WebApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();
var mongoConnectionString = Environment.GetEnvironmentVariable("MongoDbConnectionString");

if (string.IsNullOrWhiteSpace(mongoConnectionString))
{
    mongoConnectionString = builder.Configuration["MongoDbSettings:ConnectionString"];
}

builder.Services.Configure<MongoDbSettings>(options =>
{
    options.ConnectionString = mongoConnectionString!;
    options.DatabaseName = builder.Configuration.GetSection("MongoDbSettings:DatabaseName").Value!;
});

builder.Services.AddSingleton<MongoContext>();

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<PostService>();
builder.Services.AddScoped<FileService>();

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        var config = builder.Configuration;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = config["Jwt:Issuer"],
            ValidAudience = config["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!))
        };
    });

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddHttpContextAccessor();

builder.Services.AddCors(x => {
    x.AddPolicy("AllowAll", options => { options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); });
});

var app = builder.Build();
app.UseCors("AllowAll");

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapOpenApi();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/openapi/v1.json", "WebApi");
});

var mongoContext = app.Services.GetService<MongoContext>();
await MongoIndexInitializer.CreateIndexesAsync(mongoContext!);

app.Run();