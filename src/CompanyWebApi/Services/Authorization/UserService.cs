using CompanyWebApi.Configurations;
using CompanyWebApi.Contracts.Entities;
using CompanyWebApi.Core.Auth;
using CompanyWebApi.Services.Repositories;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace CompanyWebApi.Services.Authorization
{
	public class UserService : IUserService
	{
		private readonly AuthSettings _authSettings;
		private readonly IUserRepository _userRepository;
        private readonly IJwtFactory _jwtFactory;

        public UserService(IOptions<AuthSettings> authSettings, IUserRepository userRepository, IJwtFactory jwtFactory)
		{
			_userRepository = userRepository;
            _jwtFactory = jwtFactory;
            _authSettings = authSettings.Value;
		}

		public async Task<User> AuthenticateAsync(string username, string password)
		{
			var user = await _userRepository.GetSingleAsync(x => x.Username == username && x.Password == password).ConfigureAwait(false);
			if (user == null)
			{
				return null;
			}
            user.Token = string.IsNullOrEmpty(_authSettings.SecretKey) ? null : _jwtFactory.EncodeToken(user.Username);
			// Remove password before returning!
			user.Password = null;
			return user;
		}
	}
}