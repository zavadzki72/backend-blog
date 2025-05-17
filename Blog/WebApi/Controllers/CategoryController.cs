using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models;
using WebApi.Services;

namespace WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController(CategoryService service) : ControllerBase
    {
        private readonly CategoryService _service = service;

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var categories = await _service.GetAll();
            return Ok(new { Data = categories });
        }

        [HttpPost]
        public async Task<IActionResult> Post(Category request)
        {
            await _service.Add(request);
            return Created();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, Category request)
        {
            await _service.Update(id, request);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _service.Delete(id);
            return NoContent();
        }
    }
}
