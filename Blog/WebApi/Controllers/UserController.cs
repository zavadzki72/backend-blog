using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Dtos;
using WebApi.Services;

namespace WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController(UserService service) : ControllerBase
    {
        private readonly UserService _service = service;

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var users = await _service.GetAll();
            return Ok(new { Data = users });
        }

        [HttpPost("get-token")]
        [AllowAnonymous]
        public async Task<IActionResult> Post(LoginDto request)
        {
            var token = await _service.GetToken(request);
            return Ok(new { Data = token });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, UpdateUserRequest request)
        {
            await _service.Update(id, request);
            return NoContent();
        }
    }
}
