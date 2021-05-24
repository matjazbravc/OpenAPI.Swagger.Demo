using CompanyWebApi.Configurations;
using CompanyWebApi.Contracts.Entities;
using CompanyWebApi.Core.Auth;
using CompanyWebApi.Persistence.Repositories;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using CompanyWebApi.Contracts.Converters;
using CompanyWebApi.Contracts.Dto;

namespace CompanyWebApi.Services.Authorization
{
	public class UserService : IUserService
	{
		private readonly AuthSettings _authSettings;
		private readonly IUserRepository _userRepository;
        private readonly IJwtFactory _jwtFactory;
        private readonly IConverter<User, UserAuthenticateDto> _userToAuthenticateDtoConverter;

        public UserService(IOptions<AuthSettings> authSettings, IUserRepository userRepository, 
            IJwtFactory jwtFactory, IConverter<User, UserAuthenticateDto> userToAuthenticateDtoConverter)
		{
            _authSettings = authSettings.Value;
            _jwtFactory = jwtFactory;
            _userToAuthenticateDtoConverter = userToAuthenticateDtoConverter;
			_userRepository = userRepository;
		}

		public async Task<UserAuthenticateDto> AuthenticateAsync(string username, string password)
		{
			var user = await _userRepository.GetUserAsync(x => x.Username == username && x.Password == password).ConfigureAwait(false);
			if (user == null)
			{
				return null;
			}
            user.Token = string.IsNullOrEmpty(_authSettings.SecretKey) ? null : _jwtFactory.EncodeToken(user.Username);
			var result = _userToAuthenticateDtoConverter.Convert(user);
			return result;
		}
	}
}