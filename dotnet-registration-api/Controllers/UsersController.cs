using dotnet_registration_api.Data.Entities;
using dotnet_registration_api.Data.Models;
using dotnet_registration_api.Helpers;
using dotnet_registration_api.Services;
using Microsoft.AspNetCore.Mvc;
using static dotnet_registration_api.Constants.ErrorMessages;
using static dotnet_registration_api.Constants.SuccessMessages;

namespace dotnet_registration_api.Controllers
{
    /// <summary>
    /// Controller for managing user operations such as registration, login, and CRUD.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UsersController"/> class.
        /// </summary>
        /// <param name="userService">The user service for handling business logic.</param>
        public UsersController(UserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Retrieves all users.
        /// </summary>
        /// <returns>A list of all registered users.</returns>
        /// <response code="200">Returns the list of users.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<User>))]
        public async Task<ActionResult<IEnumerable<User>>> GetAll()
        {
            var users = await _userService.GetAll();
            return Ok(users);
        }

        /// <summary>
        /// Retrieves a specific user by their ID.
        /// </summary>
        /// <param name="id">The ID of the user to retrieve.</param>
        /// <returns>The requested user.</returns>
        /// <response code="200">Returns the user with the specified ID.</response>
        /// <response code="404">If the user does not exist.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(User))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        public async Task<ActionResult<User>> GetById(int id)
        {
            try
            {
                var user = await _userService.GetById(id);
                return Ok(user);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Authenticates a user and generates a login response.
        /// </summary>
        /// <param name="model">The login request containing credentials.</param>
        /// <returns>The authenticated user details.</returns>
        /// <response code="200">Returns the authenticated user.</response>
        /// <response code="400">If the request is invalid (e.g., incorrect password).</response>
        /// <response code="404">If the user does not exist.</response>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(User))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        public async Task<ActionResult<User>> Login([FromBody] LoginRequest model)
        {
            try
            {
                var user = await _userService.Login(model);
                return Ok(user);
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

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="model">The registration request containing user details.</param>
        /// <returns>The newly created user.</returns>
        /// <response code="200">Returns the registered user.</response>
        /// <response code="400">If the request is invalid (e.g., duplicate email).</response>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(User))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        public async Task<ActionResult<User>> Register([FromBody] RegisterRequest model)
        {
            try
            {
                var user = await _userService.Register(model);
                return Ok(user);
            }
            catch (AppException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Updates an existing user.
        /// </summary>
        /// <param name="id">The ID of the user to update.</param>
        /// <param name="model">The update request containing new user details.</param>
        /// <returns>The updated user.</returns>
        /// <response code="200">Returns the updated user.</response>
        /// <response code="400">If the request is invalid (e.g., validation errors).</response>
        /// <response code="404">If the user does not exist.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(User))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        public async Task<ActionResult<User>> Update(int id, [FromBody] UpdateRequest model)
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

        /// <summary>
        /// Deletes a user by their ID.
        /// </summary>
        /// <param name="id">The ID of the user to delete.</param>
        /// <returns>A success message upon deletion.</returns>
        /// <response code="200">Returns a success message.</response>
        /// <response code="404">If the user does not exist.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                await _userService.Delete(id);
                return Ok(DeletedUserSuccessMessage);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}