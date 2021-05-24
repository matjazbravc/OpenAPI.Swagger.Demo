using CompanyWebApi.Contracts.Dto;
using CompanyWebApi.Contracts.Entities;
using Microsoft.Extensions.Logging;

namespace CompanyWebApi.Contracts.Converters
{
	/// <summary>
	/// User to UserDto converter
	/// </summary>
	public class UserToAuthenticateDtoConverter : IConverter<User, UserAuthenticateDto>
	{
		private readonly ILogger<UserToAuthenticateDtoConverter> _logger;

		public UserToAuthenticateDtoConverter(ILogger<UserToAuthenticateDtoConverter> logger)
		{
			_logger = logger;
		}

		public UserAuthenticateDto Convert(User user)
		{
			_logger.LogDebug("Convert");
			var userAuthenticateDto = new UserAuthenticateDto
			{
				EmployeeId = user.EmployeeId,
                Username = user.Username,
                Password = user.Password,
                FirstName = user.Employee.FirstName,
                LastName = user.Employee.LastName,
				Token = user.Token
			};
			return userAuthenticateDto;
		}
	}
}
