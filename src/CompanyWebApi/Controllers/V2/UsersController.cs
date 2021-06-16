using CompanyWebApi.Contracts.Converters;
using CompanyWebApi.Contracts.Dto;
using CompanyWebApi.Contracts.Entities;
using CompanyWebApi.Controllers.Base;
using CompanyWebApi.Persistence.Repositories.Factory;
using CompanyWebApi.Services.Authorization;
using CompanyWebApi.Services.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;

namespace CompanyWebApi.Controllers.V2
{
    [ApiAuthorization]
    [ApiController]
    [ApiVersion("2.0")]
    [Produces("application/json")]
    [EnableCors("EnableCORS")]
    [ServiceFilter(typeof(ValidModelStateAsyncActionFilter))]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class UsersController : BaseController<UsersController>
    {
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IConverter<User, UserDto> _userToDtoConverter;
        private readonly IConverter<IList<User>, IList<UserDto>> _usersToDtoConverter;
        private readonly IUserService _userService;

        public UsersController(IUserService userService,
            IRepositoryFactory repositoryFactory,
            IConverter<User, UserDto> userToDtoConverter,
            IConverter<IList<User>, IList<UserDto>> usersToDtoConverter)
        {
            _userService = userService;
            _repositoryFactory = repositoryFactory;
            _userToDtoConverter = userToDtoConverter;
            _usersToDtoConverter = usersToDtoConverter;
        }

        /// <summary>
        /// Authenticate user
        /// </summary>
        /// <remarks>
        /// Public route that accepts HTTP POST requests containing the username and password in the body.
        /// If the username and password are correct then a JWT authentication token and the user details are returned.
        ///
        /// Sample request body:
        ///
        ///     {
        ///       "username": "johnw",
        ///       "password": "test"
        ///     }
        /// 
        /// Sample response body:
        ///
        ///     {
        ///       "employeeId": 1,
        ///       "token": "eyJhbGciOiJIUzI1NiIsInRb2hudyIs...blah blah blah...",
        ///       "username": "johnw",
        ///       "password": null,
        ///       "employeeFirstName": "Carl",
        ///       "employeeLastName": "Weiss"
        ///     }
        /// </remarks>
        /// <param name="model">AuthenticateModel model</param>
        /// <param name="version">API version</param>
        [AllowAnonymous]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(UserAuthenticateDto), Description = "User with token")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Username or password is incorrect")]
        [HttpPost("authenticate", Name = "AuthenticateUserV2")]
        public async Task<IActionResult> AuthenticateAsync([FromBody] AuthenticateModel model, ApiVersion version)
        {
            var user = await _userService.AuthenticateAsync(model.Username, model.Password).ConfigureAwait(false);
            if (user == null)
            {
                return Unauthorized(new { message = "Username or password is incorrect" });
            }
            return Ok(user);
        }

        /// <summary>
        /// Add a new user
        /// </summary>
        /// <remarks>
        /// Sample request body:
        ///
        ///     {
        ///       "employeeId": 1,
        ///       "username": "alanf",
        ///       "password": "tntgroup!129"
        ///     }
        /// 
        /// Sample response body:
        /// 
        ///     {
        ///       "username": "alanf",
        ///       "password": "tntgroup!129",
        ///       "employeeFirstName": "Alan",
        ///       "employeeLastName": "Ford"
        ///     }
        /// </remarks>
        /// <param name="user">UserCreateDto model</param>
        /// <param name="version">API AssemblyVersion</param>
        [SwaggerResponse(StatusCodes.Status201Created, Type = typeof(UserDto), Description = "Returns a new user")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "The user with EmployeeId {user.EmployeeId} already exists")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "The employee with {EmployeeId} was not found")]
        [HttpPost("create", Name = "CreateUserV2")]
        public async Task<IActionResult> CreateAsync([FromBody] UserCreateDto user, ApiVersion version)
        {
            Logger.LogDebug(nameof(CreateAsync));
            if (await _repositoryFactory.UserRepository.ExistsAsync(u => u.EmployeeId == user.EmployeeId).ConfigureAwait(false))
            {
                return BadRequest(new { message = $"The User with EmployeeId {user.EmployeeId} already exists" });
            }
            if (!await _repositoryFactory.EmployeeRepository.ExistsAsync(u => u.EmployeeId == user.EmployeeId).ConfigureAwait(false))
            {
                return NotFound(new { message = $"The Employee with EmployeeId {user.EmployeeId} was not found" });
            }

            var newUser = new User
            {
                EmployeeId = user.EmployeeId,
                Username = user.Username,
                Password = user.Password
            };

            var repoUser = await _repositoryFactory.UserRepository.AddUserAsync(newUser).ConfigureAwait(false);
            var result = _userToDtoConverter.Convert(repoUser);
            var createdResult = new ObjectResult(result)
            {
                StatusCode = StatusCodes.Status201Created
            };
            return createdResult;
        }

        /// <summary>
        /// Deletes a user
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE /api/v2/users/username
        ///
        /// Sample response body:
        ///     
        ///    Code 200 Success
        /// 
        /// </remarks>
        /// <param name="userName">User name</param>
        /// <param name="version">API version</param>
        [SwaggerResponse(StatusCodes.Status200OK, Description = "User was successfuly deleted")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "No user was found")]
        [HttpDelete("{userName}", Name = "DeleteUserByNameV2")]
        public async Task<ActionResult> DeleteAsync([Required]string userName, ApiVersion version)
        {
            Logger.LogDebug(nameof(DeleteAsync));
            var users = await _repositoryFactory.UserRepository.GetUsersByUsernameAsync(userName).ConfigureAwait(false);
            if (!users.Any())
            {
                return NotFound(new { message = $"The users with Username {userName} were not found" });
            }
            _repositoryFactory.UserRepository.Remove(users);
            await _repositoryFactory.SaveAsync().ConfigureAwait(false);
            return Ok();
        }

        /// <summary>
        /// Deletes a user
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE /api/v2/users/{employeeId}
        ///
        /// Sample response body:
        ///     
        ///    Code 200 Success
        /// 
        /// </remarks>
        /// <param name="employeeId">Employee Id</param>
        /// <param name="version">API version</param>
        [SwaggerResponse(StatusCodes.Status200OK, Description = "User was successfuly deleted")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "No user was found")]
        [HttpDelete("{employeeId:int}", Name = "DeleteUserByEmployeeIdAsyncV2")]
        public async Task<ActionResult> DeleteByEmployeeIdAsync([Required] int employeeId, ApiVersion version)
        {
            Logger.LogDebug(nameof(DeleteByEmployeeIdAsync));
            var user = await _repositoryFactory.UserRepository.GetUserAsync(employeeId).ConfigureAwait(false);
            if (user == null)
            {
                return NotFound(new { message = $"The user with EmployeeId {employeeId} was not found" });
            }
            _repositoryFactory.UserRepository.Remove(user);
            await _repositoryFactory.SaveAsync().ConfigureAwait(false);
            return Ok();
        }

        /// <summary>
        /// Gets all users
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/v2/users/getall
        ///
        /// Sample response body:
        /// 
        ///     [
        ///       {
        ///         "employeeId": 1,
        ///         "firstName": "Carl",
        ///         "lastName": "Weiss",
        ///         "username": "johnw",
        ///         "password": "test"
        ///       },
        ///       {
        ///         "employeeId": 2,
        ///         "firstName": "Mathias",
        ///         "lastName": "Gernold",
        ///         "username": "mathiasg",
        ///         "password": "test"
        ///       },
        ///       {
        ///         "employeeId": 3,
        ///         "firstName": "Julia",
        ///         "lastName": "Reynolds",
        ///         "username": "juliar",
        ///         "password": "test"
        ///       }
        ///     ]
        /// </remarks>
        /// <param name="version">API version</param>
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(IList<UserDto>), Description = "Return list of users")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "The users list is empty")]
        [HttpGet("getAll", Name = "GetAllUsersV2")]
        public async Task<ActionResult<IList<UserDto>>> GetAllAsync(ApiVersion version)
        {
            Logger.LogDebug(nameof(GetAllAsync));
            var usersDto = await _repositoryFactory.UserRepository.GetUsersAsync().ConfigureAwait(false);
            if (!usersDto.Any())
            {
                return NotFound(new { message = "The users list is empty"});
            }
            return Ok(usersDto);
        }

        /// <summary>
        /// Get user(s)
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/v2/users/alanf
        ///
        /// Sample response body:
        ///  
        ///     [
        ///       {
        ///         "employeeId": 6,
        ///         "firstName": "Alan",
        ///         "lastName": "Ford",
        ///         "username": "alanf",
        ///         "password": "tntgroup!129"
        ///       },
        ///       {
        ///         "employeeId": 7,
        ///         "firstName": "Alan",
        ///         "lastName": "Ford",
        ///         "username": "alanf",
        ///         "password": "tntgroup!129"
        ///       },
        ///       {
        ///         "employeeId": 8,
        ///         "firstName": "Alan",
        ///         "lastName": "Ford",
        ///         "username": "alanf",
        ///         "password": "tntgroup!129"
        ///       }
        ///     ]
        /// </remarks>
        /// <param name="userName"></param>
        /// <param name="version">API version</param>
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(IList<UserDto>), Description = "Return list of users")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "The users were not found")]
        [HttpGet("{userName}", Name = "GetUserByUsernameV2")]
        public async Task<ActionResult<IList<UserDto>>> GetAsync(string userName, ApiVersion version)
        {
            Logger.LogDebug(nameof(GetAsync));
            var users = await _repositoryFactory.UserRepository.GetUsersByUsernameAsync(userName).ConfigureAwait(false);
            if (!users.Any())
            {
                return NotFound(new { message = $"The users with {userName} were not found" });
            }
            var usersDto = _usersToDtoConverter.Convert(users);
            return Ok(usersDto);
        }

        /// <summary>
        /// Update user
        /// </summary>
        /// <remarks>
        /// Sample request body:
        ///
        ///     {
        ///       "employeeId": 1,
        ///       "username": "alanf",
        ///       "password": "new!pass"
        ///     }
        /// 
        /// Sample response body:
        /// 
        ///     {
        ///       "employeeId": 1,
        ///       "firstName": "Carl",
        ///       "lastName": "Weiss",
        ///       "username": "alanf",
        ///       "password": "new!pass"
        ///     }
        /// </remarks>
        /// <param name="userToUpdate"><see cref="UserUpdateDto"/></param>
        /// <param name="version">API AssemblyVersion</param>
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(UserDto), Description = "Return updated user")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "The employee was not found")]
        [HttpPut("update", Name = "UpdateUserV2")]
        public async Task<IActionResult> UpdateAsync([FromBody] UserUpdateDto userToUpdate, ApiVersion version)
        {
            Logger.LogDebug(nameof(UpdateAsync));
            var repoUser = await _repositoryFactory.UserRepository.GetUserAsync(userToUpdate.EmployeeId).ConfigureAwait(false);
            if (repoUser == null)
            {
                return NotFound(new { message = "The user was not found" });
            }

            repoUser.Username = userToUpdate.Username;
            repoUser.Password = userToUpdate.Password;

            await _repositoryFactory.UserRepository.UpdateAsync(repoUser);
            await _repositoryFactory.SaveAsync().ConfigureAwait(false);

            var userDto = _userToDtoConverter.Convert(repoUser);
            return Ok(userDto);
        }
    }
}