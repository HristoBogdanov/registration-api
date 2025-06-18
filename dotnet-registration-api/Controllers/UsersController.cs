using dotnet_registration_api.Data.Entities;
using dotnet_registration_api.Data.Models;
using dotnet_registration_api.Helpers;
using dotnet_registration_api.Services;
using Microsoft.AspNetCore.Mvc;
using static dotnet_registration_api.Constants.ErrorMessages;
using static dotnet_registration_api.Constants.SuccessMessages;

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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAll()
        {
            var users = await _userService.GetAll();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetById(int id)
        {
            try
            {
                var user = await _userService.GetById(id);
                return Ok(user);
            }
            catch(NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<User>> Login([FromBody]LoginRequest model)
        {
            try
            {
                var user = await _userService.Login(model);
                return Ok(user);
            }
            catch(AppException ex)
            {
                return BadRequest(ex.Message);
            }
            catch(NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register([FromBody]RegisterRequest model)
        {
            try
            {
                var user = await _userService.Register(model);
                return Ok(user);
            }
            catch(AppException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<User>> Update(int id, [FromBody]UpdateRequest model)
        {
            var user = await _userService.GetById(id);
            if (user == null)
            {
                return NotFound(NoUserErrorMessage);
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
                return Ok(DeletedUserSuccessMessage);
            }
            catch(NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
