using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Services;

namespace WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class FileController(FileService fileService, UserService userService, PostService postService) : ControllerBase
    {
        private readonly FileService _fileService = fileService;
        private readonly UserService _userService = userService;
        private readonly PostService _postService = postService;

        [HttpPost("upload/post-files")]
        public async Task<IActionResult> Upload([FromForm] IFormFile file, [FromForm] Guid postId)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Arquivo inválido.");

            var key = $"posts/{postId}/featured_image_{Path.GetFileName(file.FileName)}";
            await _fileService.Upload(file, key);

            await _postService.UpdateImage(postId, key);

            return Ok(new { Data = key });
        }

        [HttpPost("upload/user-files")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Arquivo inválido.");

            var loggedUser = await _userService.GetLoggedUser();

            var key = $"users/{loggedUser.Id}/picture_image_{Path.GetFileName(file.FileName)}";
            await _fileService.Upload(file, key);

            return Ok(new { Data = key });
        }

        [HttpGet("access-url")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAccessUrl([FromQuery] string key)
        {
            var url = await _fileService.GeneratePresignedUrl(key, TimeSpan.FromMinutes(10));
            return Ok(new { Data = url });
        }
    }
}
