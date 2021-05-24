using CompanyWebApi.Contracts.Dto;
using System.Threading.Tasks;

namespace CompanyWebApi.Services.Authorization
{
	public interface IUserService
	{
		Task<UserAuthenticateDto> AuthenticateAsync (string username, string password);
	}
}
