using CompanyWebApi.Contracts.Entities;
using CompanyWebApi.Controllers.Base;
using CompanyWebApi.Core.Errors;
using CompanyWebApi.Services.Authorization;
using CompanyWebApi.Services.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CompanyWebApi.Controllers
{
    [Authorize]
    [ApiController]
    [ApiVersion("1.1")]
    [EnableCors("EnableCORS")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class UsersController : BaseController<UsersController>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserService _userService;
        public UsersController(IUserService userService, IUserRepository userRepository)
        {
            _userService = userService;
            _userRepository = userRepository;
        }

        /// <summary>
        /// Authenticate User
        /// </summary>
        /// <remarks>Public route that accepts HTTP POST requests containing the username and password in the body.
        /// If the username and password are correct then a JWT authentication token and the user details are returned.
        /// </remarks>
        /// POST /api/users/v1.1/authenticate/{user}
        /// <param name="model"></param>
        /// <returns>User with token</returns>
		[AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<IActionResult> AuthenticateAsync([FromBody] AuthenticateModel model)
        {
            var user = await _userService.AuthenticateAsync(model.Username, model.Password).ConfigureAwait(false);
            if (user == null)
            {
                return BadRequest(new { message = "Username or password is incorrect" });
            }
            return Ok(user);
        }

        /// <summary>
        /// Create User
        /// </summary>
        /// <remarks>This API will create new User</remarks>
        /// POST /api/users/v1.1/create/{user}
        /// <param name="user">User model</param>
        /// <param name="apiVersion">API Version</param>
        [MapToApiVersion("1.1")]
        [HttpPost("create", Name = "CreateUser")]
        [ProducesResponseType(201, Type = typeof(User))]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateAsync([FromBody] User user, ApiVersion apiVersion)
        {
            Logger.LogDebug("CreateAsync");
            if (user == null)
            {
                return BadRequest(new BadRequestError("The user is null"));
            }
            await _userRepository.AddAsync(user).ConfigureAwait(false);
            return CreatedAtRoute("GetUserByUserName", new { userName = user.Username, version = apiVersion.ToString() }, user);
        }

        /// <summary>
        /// Delete User
        /// </summary>
        /// <remarks>This API will delete User with userName</remarks>
        /// GET /api/users/v1.1/{userName}
        /// <param name="userName"></param>
        /// <returns>Return User</returns>
        [MapToApiVersion("1.1")]
        [HttpDelete("{userName}", Name = "DeleteUserByName")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> DeleteAsync(string userName)
        {
            Logger.LogDebug("DeleteAsync");
            var user = await _userRepository.GetSingleAsync(user => user.Username == userName).ConfigureAwait(false);
            if (user == null)
            {
                return NotFound(new NotFoundError("The User was not found"));
            }
            await _userRepository.DeleteAsync(user).ConfigureAwait(false);
            return NoContent();
        }

        /// <summary>
        /// Get all Users
        /// </summary>
        /// <remarks>Secure route that accepts HTTP GET requests and returns a list of all the users in the application 
        /// if the HTTP Authorization header contains a valid JWT token.
        /// If there is no auth token or the token is invalid then a 401 Unauthorized response is returned.
        /// </remarks>
        /// GET /api/users/v1.1/getall
        /// <returns>List of Users</returns>
        [MapToApiVersion("1.1")]
        [HttpGet("getall")]
        public async Task<ActionResult<IList<User>>> GetAllAsync()
        {
            Logger.LogDebug("GetAllAsync");
            var users = await _userRepository.GetAllAsync().ConfigureAwait(false);
            if (!users.Any())
            {
                return NotFound(new NotFoundError("The Users list is empty"));
            }
            var result = users.Select(user => new
            {
                user.Username,
                user.Password,
                EmployeeFirstName = user.Employee?.FirstName,
                EmployeeLastName = user.Employee?.LastName
            }).ToList();
            return Ok(result);
        }

        /// <summary>
        /// Get User
        /// </summary>
        /// <remarks>This API return User with Username</remarks>
        /// GET /api/users/v1.1/{userName}
        /// <param name="userName"></param>
        /// <returns>Return User</returns>
        [MapToApiVersion("1.1")]
        [HttpGet("{userName}", Name = "GetUserByUserName")]
        [ProducesResponseType(200, Type = typeof(User))]
        [ProducesResponseType(404)]
        public async Task<ActionResult<User>> GetAsync(string userName)
        {
            Logger.LogDebug("GetAsync");
            var user = await _userRepository.GetSingleAsync(user => user.Username.Equals(userName)).ConfigureAwait(false);
            if (user == null)
            {
                return NotFound(new NotFoundError("The User was not found"));
            }
            return Ok(user);
        }

        /// <summary>
        /// Update User
        /// </summary>
        /// POST /api/users/v1.1/update/{user}
        /// <param name="user"></param>
        /// <param name="apiVersion">API Version</param>
        /// <returns>Returns updated User</returns>
        [MapToApiVersion("1.1")]
        [HttpPost("update", Name = "UpdateUser")]
        [ProducesResponseType(201, Type = typeof(User))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> UpdateAsync([FromBody] User user, ApiVersion apiVersion)
        {
            Logger.LogDebug("UpdateAsync");
            if (user == null)
            {
                return BadRequest(new BadRequestError("The retrieved user is null"));
            }
            var updatedUser = await _userRepository.UpdateAsync(user);
            if (updatedUser == null)
            {
                return BadRequest(new BadRequestError("The updated user is null"));
            }
            return CreatedAtRoute("GetUserByUserName", new { userName = user.Username, version = apiVersion.ToString() }, user);
        }
    }
}