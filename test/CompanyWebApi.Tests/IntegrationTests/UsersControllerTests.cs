using CompanyWebApi.Contracts.Entities;
using CompanyWebApi.Tests.Services;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System;
using Xunit;

namespace CompanyWebApi.Tests.IntegrationTests
{
    public class UsersControllerTests : ControllerTestsBase
    {
        private const string BASE_URL = "/api/v2/users/";
        private readonly HttpClientHelper _httpClientHelper;

        public UsersControllerTests(WebApiTestFactory factory)
            : base(factory)
        {
            _httpClientHelper = new HttpClientHelper(Client);
        }

        [Fact]
        public async Task CanCreateAndDeleteUsers()
        {
            var newCompany = new Company
            {
                CompanyId = 9999,
                Name = "Company TEST",
                Created = DateTime.UtcNow,
                Modified = DateTime.UtcNow
            };
            var newDepartment = new Department { DepartmentId = 9999, Name = "Department TEST" };
            var newEmployee = new Employee
            {
                EmployeeId = 9999,
                FirstName = "Sylvester",
                LastName = "Holt",
                BirthDate = new DateTime(1995, 8, 7),
                Company = newCompany,
                Department = newDepartment,
                Created = DateTime.UtcNow,
                Modified = DateTime.UtcNow
            };
            var newUser = new User
            {
                EmployeeId = newEmployee.EmployeeId,
                Employee = newEmployee,
                Username = "testuser",
                Password = "test",
                Token = string.Empty
            };

            _httpClientHelper.Client.SetFakeBearerToken((object)Token);
            var user = await _httpClientHelper.PostAsync(BASE_URL + "create", newUser).ConfigureAwait(false);
            Assert.Equal("testuser", user.Username);

            /* Delete new User */
            _httpClientHelper.Client.SetFakeBearerToken((object)Token);
            await _httpClientHelper.DeleteAsync(BASE_URL + "DeleteUserByName/testuser").ConfigureAwait(false);
        }

        [Fact]
        public async Task CanGetAllUsers()
        {
            _httpClientHelper.Client.SetFakeBearerToken((object)Token);
            var users = await _httpClientHelper.GetAsync<List<User>>(BASE_URL + "getall").ConfigureAwait(false);
            Assert.Contains(users, p => p.Username == "johnw");
        }

        [Fact]
        public async Task CanGetUser()
        {
            _httpClientHelper.Client.SetFakeBearerToken((object)Token);
            var user = await _httpClientHelper.GetAsync<User>(BASE_URL + "johnw").ConfigureAwait(false);
            Assert.Equal("johnw", user.Username);
        }

        [Fact]
        public async Task CanUpdateUser()
        {
            // Get user and change Password
            var newUser = new User
            {
                EmployeeId = 2,
                Username = "mathiasg",
                Password = "abcde12345",
                Token = string.Empty
            };

            _httpClientHelper.Client.SetFakeBearerToken((object)Token);
            var updatedUser = await _httpClientHelper.PostAsync(BASE_URL + "update", newUser).ConfigureAwait(false);
            Assert.Equal("abcde12345", updatedUser.Password);
        }

        [Fact]
        public async Task CanUserAuthenticate()
        {
            _httpClientHelper.Client.SetFakeBearerToken((object)Token);
            var user = new User { EmployeeId = 1, Username = "johnw", Password = "test", Token = string.Empty };
            var authenticatedUser = await _httpClientHelper.PostAsync(BASE_URL + "authenticate", user).ConfigureAwait(false);
            Assert.NotNull(authenticatedUser);
        }
    }
}
