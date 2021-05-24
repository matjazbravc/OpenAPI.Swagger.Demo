using CompanyWebApi.Contracts.Dto;
using CompanyWebApi.Tests.Services;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System;
using Xunit;

namespace CompanyWebApi.Tests.IntegrationTests
{
	public class EmployeesControllerTests : ControllerTestsBase
	{
        private const string API_VERSION = "V2";
        private readonly string _baseUrl;
		private readonly HttpClientHelper _httpClientHelper;

		public EmployeesControllerTests(WebApiTestFactory factory)
			: base(factory)
		{
            _baseUrl = $"/api/{API_VERSION.ToLower()}/employees/";
			_httpClientHelper = new HttpClientHelper(Client);
			_httpClientHelper.Client.SetFakeBearerToken((object)Token);
		}

		[Fact]
		public async Task CanCreateAndDeleteEmployee()
		{
            var newEmployee = new EmployeeCreateDto
			{
                FirstName = "Sylvester",
                LastName = "Holt",
                BirthDate = new DateTime(1995, 8, 7),
                CompanyId = 1,
                DepartmentId = 1,
				Address = "New Address",
				Password = "sdfaeswgraewg23423&",
				Username = "sylholt"
			};
            var employee = await _httpClientHelper.PostAsync<EmployeeCreateDto, EmployeeDto>(_baseUrl + "create", newEmployee).ConfigureAwait(false);
            Assert.Equal(newEmployee.FirstName, employee.FirstName);
            await _httpClientHelper.DeleteAsync(_baseUrl + $"DeleteEmployeeById{API_VERSION}/{employee.EmployeeId}").ConfigureAwait(false);
		}

		[Fact]
		public async Task CanGetAllEmployees()
		{
			var employees = await _httpClientHelper.GetAsync<List<EmployeeDto>>(_baseUrl + "getall").ConfigureAwait(false);
			Assert.Contains(employees, p => p.FirstName == "Julia");
		}

		[Fact]
		public async Task CanGetEmployee()
		{
			// The endpoint or route of the controller action.
			var employee = await _httpClientHelper.GetAsync<EmployeeDto>(_baseUrl + "3").ConfigureAwait(false);
			Assert.Equal(3, employee.EmployeeId);
			Assert.Equal("Julia", employee.FirstName);
		}

		[Fact]
		public async Task CanUpdateEmployee()
		{
			// Get first employee
            /* Create Employee */
            var newEmployee = new EmployeeUpdateDto()
            {
                EmployeeId = 1,
                FirstName = "Johnny",
                LastName = "Holt",
                BirthDate = new DateTime(1995, 8, 7)
            };

			// Update employee
			var updatedEmployee = await _httpClientHelper.PutAsync<EmployeeUpdateDto, EmployeeDto>(_baseUrl + "update", newEmployee).ConfigureAwait(false);

			// First name should be a new one
			Assert.Equal("Johnny", updatedEmployee.FirstName);
		}
	}
}
