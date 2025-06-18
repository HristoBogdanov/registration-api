using dotnet_registration_api.Data.Entities;
using dotnet_registration_api.Data.Models;
using dotnet_registration_api.Helpers;
using dotnet_registration_api.Services;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_registration_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;
        public UsersController(UserService userService)
        {
            _userService = userService;
        }
        [HttpPost("login")]
        public async Task<ActionResult<User>> Login([FromBody]LoginRequest model)
        {
            if(model.Username == String.Empty || model.Password == String.Empty)
            {
                return BadRequest(ModelState);
            }

            var user = await _userService.Login(model);

            if (user == null)
            {
                return NotFound(ModelState);
            }

            return Ok(user);
        }
        [HttpPost("register")]
        public async Task<ActionResult<User>> Register([FromBody]RegisterRequest model)
        {
            var users = await _userService.GetAll();
            if(model.Password == String.Empty || users.Any(u => u.Username == model.Username))
            {
                return BadRequest(ModelState);
            }

            var user = await _userService.Register(model);
            return Ok(user);
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAll()
        {
            var users = await _userService.GetAll();
            return Ok(users);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetById(int id)
        {
            var user = await _userService.GetById(id);
            if(user == null)
            {
                return NotFound(ModelState);
            }

            return Ok(user);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<User>> Update(int id, [FromBody]UpdateRequest model)
        {
            var user = await _userService.GetById(id);
            if (user == null)
            {
                return NotFound(ModelState);
            }

            try
            {
                var resultUser = await _userService.Update(id, model);
                return Ok(resultUser);
            }
            catch (AppException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                await _userService.Delete(id);
                return Ok();
            }
            catch(NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
