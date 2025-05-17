using Minio;
using System.Reactive.Linq;

namespace WebApi.Services
{
    public class FileService
    {
        private readonly MinioClient _minioClient;
        private readonly string _bucketName;

        public FileService(IConfiguration configuration)
        {
            var endpoint = configuration["Storage:Endpoint"]!;

            _minioClient = new MinioClient()
                .WithEndpoint(endpoint.Replace("https://", "").Replace("http://", ""))
                .WithCredentials(configuration["Storage:AccessKey"], configuration["Storage:SecretKey"])
                .WithSSL(endpoint.StartsWith("https://"))
                .Build();
            _bucketName = configuration["Storage:Bucket"]!;
        }

        public async Task<string> Upload(IFormFile file, string key)
        {
            using var stream = file.OpenReadStream();
            var putObjectArgs = new PutObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(key)
                .WithStreamData(stream)
                .WithObjectSize(file.Length)
                .WithContentType(file.ContentType);

            await _minioClient.PutObjectAsync(putObjectArgs);
            return key;
        }

        public async Task<Stream> Download(string key)
        {
            var memoryStream = new MemoryStream();

            var getObjectArgs = new GetObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(key)
                .WithCallbackStream(stream =>
                {
                    stream.CopyTo(memoryStream);
                    memoryStream.Position = 0;
                });

            await _minioClient.GetObjectAsync(getObjectArgs);

            return memoryStream;
        }

        public async Task DeleteAsync(string key)
        {
            var removeObjectArgs = new RemoveObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(key);

            await _minioClient.RemoveObjectAsync(removeObjectArgs);
        }

        public async Task<string> GeneratePresignedUrl(string key, TimeSpan expiresIn)
        {
            var presignedGetObjectArgs = new PresignedGetObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(key)
                .WithExpiry((int)expiresIn.TotalSeconds);

            return await _minioClient.PresignedGetObjectAsync(presignedGetObjectArgs);
        }

        public async Task<bool> ObjectExists(string key)
        {
            try
            {
                var statObjectArgs = new StatObjectArgs()
                    .WithBucket(_bucketName)
                    .WithObject(key);

                await _minioClient.StatObjectAsync(statObjectArgs);
                return true;
            } 
            catch (Minio.Exceptions.ObjectNotFoundException)
            {
                return false;
            }
        }

        public async Task<List<string>> ListObjects(string? prefix = null)
        {
            var objects = new List<string>();

            var listObjectsArgs = new ListObjectsArgs()
                .WithBucket(_bucketName);

            if (!string.IsNullOrEmpty(prefix))
            {
                listObjectsArgs.WithPrefix(prefix);
            }

            var objectList = await _minioClient.ListObjectsAsync(listObjectsArgs).ToList();

            foreach (var item in objectList)
            {
                objects.Add(item.Key);
            }

            return objects;
        }
    }
}
