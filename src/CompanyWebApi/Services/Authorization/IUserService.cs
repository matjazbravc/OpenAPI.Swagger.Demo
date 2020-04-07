using CompanyWebApi.Contracts.Entities;
using System.Threading.Tasks;

namespace CompanyWebApi.Services.Authorization
{
	public interface IUserService
	{
		Task<User> AuthenticateAsync (string username, string password);
	}
}
