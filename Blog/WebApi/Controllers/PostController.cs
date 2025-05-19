using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Dtos;
using WebApi.Services;

namespace WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PostController(PostService service) : ControllerBase
    {
        private readonly PostService _service = service;

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetPaginated(GetPostPaginatedDto request)
        {
            var result = await _service.GetPaginatedPostsAsync(request);
            return Ok(new { Data = result });
        }

        [HttpPost]
        public async Task<IActionResult> Post(AddPostDto request)
        {
            await _service.CreatePost(request);
            return Created();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, UpdatePostDto request)
        {
            await _service.UpdatePost(id, request);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _service.RemovePost(id);
            return NoContent();
        }

        [HttpPatch("/archive")]
        public async Task<IActionResult> Archive(Guid id)
        {
            await _service.ArchivePost(id);
            return NoContent();
        }

        [HttpPatch("/reactivate")]
        public async Task<IActionResult> Reactivate(Guid id)
        {
            await _service.ReactivatePost(id);
            return NoContent();
        }

        [HttpPatch("/up-vote")]
        [AllowAnonymous]
        public async Task<IActionResult> UpVote(Guid id)
        {
            await _service.UpVote(id);
            return NoContent();
        }

        [HttpPatch("/view")]
        [AllowAnonymous]
        public async Task<IActionResult> View(Guid id)
        {
            await _service.View(id);
            return NoContent();
        }
    }
}
