using CompanyWebApi.Contracts.Entities;
using CompanyWebApi.Tests.Services;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System;
using System.Linq;
using CompanyWebApi.Contracts.Dto;
using Xunit;

namespace CompanyWebApi.Tests.IntegrationTests
{
    public class UsersControllerTests : ControllerTestsBase
    {
        private const string API_VERSION = "V2";
        private readonly string _usersBaseUrl;
        private readonly string _employeesBaseUrl;
        private readonly HttpClientHelper _httpClientHelper;

        public UsersControllerTests(WebApiTestFactory factory)
            : base(factory)
        {
            _usersBaseUrl = $"/api/{API_VERSION.ToLower()}/users/";
            _employeesBaseUrl = $"/api/{API_VERSION.ToLower()}/employees/";
            _httpClientHelper = new HttpClientHelper(Client);
            _httpClientHelper.Client.SetFakeBearerToken((object)Token);
        }

        [Fact]
        public async Task CanCreateAndDeleteUsers()
        {
            var newEmployee = new EmployeeCreateDto()
            {
                CompanyId = 1,
                DepartmentId = 1,
                FirstName = "TestFirstName",
                LastName = "TestLastName",
                BirthDate = new DateTime(1991, 8, 7),
                Address = "TestAddress"
            };
            // Save test employee
            var employee = await _httpClientHelper.PostAsync<EmployeeCreateDto, EmployeeDto>(_employeesBaseUrl + "create", newEmployee).ConfigureAwait(false);

            // Create test user
            var newUser = new UserCreateDto()
            {
                EmployeeId = employee.EmployeeId,
                Username = "testuser",
                Password = "testpassword"
            };

            var user = await _httpClientHelper.PostAsync<UserCreateDto, UserDto>(_usersBaseUrl + "create", newUser).ConfigureAwait(false);
            Assert.Equal("testuser", user.Username);

            // Delete test user
            await _httpClientHelper.DeleteAsync(_usersBaseUrl + $"DeleteUserByEmployeeIdAsync{API_VERSION}/{user.EmployeeId}").ConfigureAwait(false);

            // Delete test employee
            await _httpClientHelper.DeleteAsync(_employeesBaseUrl + $"DeleteEmployeeById{API_VERSION}/{employee.EmployeeId}").ConfigureAwait(false);
        }

        [Fact]
        public async Task CanGetAllUsers()
        {
            _httpClientHelper.Client.SetFakeBearerToken((object)Token);
            var users = await _httpClientHelper.GetAsync<List<UserDto>>(_usersBaseUrl + "getall").ConfigureAwait(false);
            Assert.Contains(users, p => p.Username == "johnw");
        }

        [Fact]
        public async Task CanGetUser()
        {
            _httpClientHelper.Client.SetFakeBearerToken((object)Token);
            var users = await _httpClientHelper.GetAsync<List<UserDto>>(_usersBaseUrl + "johnw").ConfigureAwait(false);
            Assert.True(users.Any());
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

            var updatedUser = await _httpClientHelper.PutAsync(_usersBaseUrl + "update", newUser).ConfigureAwait(false);
            Assert.Equal("abcde12345", updatedUser.Password);
        }

        [Fact]
        public async Task CanUserAuthenticate()
        {
            var user = new AuthenticateModel
            {
                Username = "johnw", Password = "test"
            };
            var authenticatedUser = await _httpClientHelper.PostAsync<AuthenticateModel, AuthenticateModel>(_usersBaseUrl + "authenticate", user).ConfigureAwait(false);
            Assert.NotNull(authenticatedUser);
        }
    }
}
