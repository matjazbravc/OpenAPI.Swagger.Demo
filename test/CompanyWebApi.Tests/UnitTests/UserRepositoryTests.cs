using CompanyWebApi.Contracts.Entities;
using CompanyWebApi.Services.Repositories;
using CompanyWebApi.Tests.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Xunit;
using System.Linq;

namespace CompanyWebApi.Tests.UnitTests
{
    [Collection("Sequential")]
    public class UserRepositoryTests : IClassFixture<WebApiTestFactory>
    {
        private readonly ILogger _logger;
        private readonly WebApiTestFactory _factory;
        private readonly IUserRepository _userRepository;

        public UserRepositoryTests(WebApiTestFactory factory)
        {
            _factory = factory;
            _logger = _factory.Services.GetRequiredService<ILogger<WebApiTestFactory>>();
            _userRepository = _factory.Services.GetRequiredService<IUserRepository>();
        }

        [Fact]
        public async Task CanAdd()
        {
            _logger.LogInformation("CanAdd");
            var user = new User { EmployeeId = 999, Username = "tester", Password = "test", Token = string.Empty };
            var newUser = await _userRepository.AddAsync(user).ConfigureAwait(false);
            Assert.Equal("tester", newUser.Username);
        }

        [Fact]
        public async Task CanCount()
        {
            _logger.LogInformation("CanCount");
            var nrCompanies = await _userRepository.CountAsync().ConfigureAwait(false);
            Assert.True(nrCompanies > 0);
        }

        [Fact]
        public async Task CanDelete()
        {
            _logger.LogInformation("CanDelete");
            var user = new User { EmployeeId = 9999, Username = "tester", Password = "test", Token = string.Empty };
            var newUser = await _userRepository.AddAsync(user).ConfigureAwait(false);
            var result = await _userRepository.DeleteAsync(newUser).ConfigureAwait(false);

            Assert.True(result > 0);
        }

        [Fact]
        public async Task CanGetAllByPredicate()
        {
            _logger.LogInformation("CanGetAllByPredicate");
            var companies = await _userRepository.GetAllAsync(cmp => cmp.Username.Equals("johnw")).ConfigureAwait(false);
            Assert.True(companies.Count() > 0);
        }

        [Fact]
        public async Task CanGetSingle()
        {
            _logger.LogInformation("LogInformation");
            var user = await _userRepository.GetSingleAsync(cmp => cmp.Username.Equals("johnw")).ConfigureAwait(false);
            Assert.True(user != null);
        }

        [Fact]
        public async Task CanGetAll()
        {
            _logger.LogInformation("CanGetAll");
            var companies = await _userRepository.GetAllAsync().ConfigureAwait(false);
            Assert.True(companies.Count() > 0);
        }

        [Fact]
        public async Task CanUpdate()
        {
            _logger.LogInformation("CanUpdate");
            var user = new User { EmployeeId = 1, Username = "johnw", Password = "sfd$%fsaDgw4564", Token = string.Empty };
            var updatedUser = await _userRepository.UpdateAsync(user).ConfigureAwait(false);
            Assert.Equal("sfd$%fsaDgw4564", updatedUser.Password);
        }
    }
}
