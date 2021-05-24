using System;
using CompanyWebApi.Contracts.Entities;
using CompanyWebApi.Persistence.Repositories;
using CompanyWebApi.Tests.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CompanyWebApi.Tests.UnitTests
{
    [Collection("Sequential")]
    public class UserRepositoryTests : IClassFixture<WebApiTestFactory>
    {
        private readonly ILogger _logger;
        private readonly IUserRepository _userRepository;
        private readonly IEmployeeRepository _employeeRepository;

        public UserRepositoryTests(WebApiTestFactory factory)
        {
            _logger = factory.Services.GetRequiredService<ILogger<WebApiTestFactory>>();
            _userRepository = factory.Services.GetRequiredService<IUserRepository>();
            _employeeRepository = factory.Services.GetRequiredService<IEmployeeRepository>();
        }

        [Fact]
        public async Task CanAdd()
        {
            _logger.LogInformation("CanAdd");
            var employee = new Employee
            {
                EmployeeId = 9999,
                FirstName = "TesterFirstName",
                LastName = "TesterLastName",
                BirthDate = new DateTime(2001, 12, 16),
                CompanyId = 1,
                DepartmentId = 1,
                Created = DateTime.UtcNow,
                Modified = DateTime.UtcNow
            };
            var repoEmployee = await _employeeRepository.AddEmployeeAsync(employee, true).ConfigureAwait(false);

            var user = new User
            {
                EmployeeId = 9999, 
                Username = "tester", 
                Password = "testPass", 
                Token = string.Empty
            };

            var addedUser = await _userRepository.AddUserAsync(user, true).ConfigureAwait(false);
            Assert.Equal("tester", addedUser.Username);

            var repoUser = await _userRepository.GetUserAsync(user.EmployeeId, true).ConfigureAwait(false);
            _userRepository.Remove(repoUser);
            await _userRepository.SaveAsync().ConfigureAwait(false);

            _employeeRepository.Remove(repoEmployee);
            await _employeeRepository.SaveAsync().ConfigureAwait(false);
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
            var user = new User
            {
                EmployeeId = 9999, Username = "tester", Password = "test", Token = string.Empty
            };
            await _userRepository.AddUserAsync(user, true).ConfigureAwait(false);
            _userRepository.Remove(user);
            await _userRepository.SaveAsync().ConfigureAwait(false);
            var repoUser = await _userRepository.GetUserAsync(user.EmployeeId).ConfigureAwait(false);
            Assert.Null(repoUser);
        }

        [Fact]
        public async Task CanGetSingle()
        {
            _logger.LogInformation("LogInformation");
            var user = await _userRepository.GetUserAsync(cmp => cmp.Username.Equals("johnw")).ConfigureAwait(false);
            Assert.True(user != null);
        }

        [Fact]
        public async Task CanGetAll()
        {
            _logger.LogInformation("CanGetAll");
            var companies = await _userRepository.GetUsersAsync().ConfigureAwait(false);
            Assert.True(companies.Any());
        }

        [Fact]
        public async Task CanUpdate()
        {
            _logger.LogInformation("CanUpdate");
            var user = new User
            {
                EmployeeId = 1, Username = "johnw", Password = "sfd$%fsaDgw4564", Token = string.Empty
            };
            await _userRepository.UpdateAsync(user).ConfigureAwait(false);
            await _userRepository.SaveAsync().ConfigureAwait(false);
            Assert.Equal("sfd$%fsaDgw4564", user.Password);
        }
    }
}
