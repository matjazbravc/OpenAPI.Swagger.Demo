using CompanyWebApi.Services.Authorization;
using CompanyWebApi.Tests.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Xunit;

namespace CompanyWebApi.Tests.UnitTests
{
    public class UserServiceTests : IClassFixture<WebApiTestFactory>
    {
        private readonly WebApiTestFactory _factory;
        private readonly IUserService _userService;

        public UserServiceTests(WebApiTestFactory factory)
        {
            _factory = factory;
            _userService = _factory.Services.GetRequiredService<IUserService>();
        }

        [Fact]
        public async Task CanAuthorizeUser()
        {
            var authenticatedUser = await _userService.AuthenticateAsync("johnw", "test").ConfigureAwait(false);
            Assert.True(!string.IsNullOrEmpty(authenticatedUser.Token));
        }
    }
}
