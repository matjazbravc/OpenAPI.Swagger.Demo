using CompanyWebApi.Contracts.Dto;
using CompanyWebApi.Contracts.Entities;
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
		private const string BASE_URL = "/api/v2/employees/";
		private readonly HttpClientHelper _httpClientHelper;

		public EmployeesControllerTests(WebApiTestFactory factory)
			: base(factory)
		{
			_httpClientHelper = new HttpClientHelper(Client);
			_httpClientHelper.Client.SetFakeBearerToken((object)Token);
		}

		[Fact]
		public async Task CanCreateAndDeleteEmployee()
		{
			var newCompany = new Company
			{
				CompanyId = 999,
				Name = "TEST COMPANY",
				Created = DateTime.UtcNow,
				Modified = DateTime.UtcNow
			};

			var newDepartment = new Department { DepartmentId = 999, Name = "TEST DEPARTMENT"};

			/* Create Employee */
			var newEmployee = new Employee
			{
				EmployeeId = 999,
				FirstName = "Sylvester",
				LastName = "Holt",
				BirthDate = new DateTime(1995, 8, 7),
				Company = newCompany,
				Department = newDepartment,
				Created = DateTime.UtcNow,
				Modified = DateTime.UtcNow
			};

			var employee = await _httpClientHelper.PostAsync(BASE_URL + "create", newEmployee).ConfigureAwait(false);
			Assert.Equal("Sylvester", employee.FirstName);
			Assert.Equal("Holt", employee.LastName);

			/* Delete new Employee */
			await _httpClientHelper.DeleteAsync(BASE_URL + "999").ConfigureAwait(false);
		}

		[Fact]
		public async Task CanGetAllEmployees()
		{
			var employees = await _httpClientHelper.GetAsync<List<EmployeeDto>>(BASE_URL + "getall").ConfigureAwait(false);
			Assert.Contains(employees, p => p.FirstName == "Julia");
		}

		[Fact]
		public async Task CanGetEmployee()
		{
			// The endpoint or route of the controller action.
			var employee = await _httpClientHelper.GetAsync<EmployeeDto>(BASE_URL + "3").ConfigureAwait(false);
			Assert.Equal(3, employee.EmployeeId);
			Assert.Equal("Julia", employee.FirstName);
		}

		[Fact]
		public async Task CanUpdateEmployee()
		{
			// Get first employee
            /* Create Employee */
            var newEmployee = new Employee
            {
                EmployeeId = 1,
                FirstName = "Johnny",
                LastName = "Holt",
                BirthDate = new DateTime(1995, 8, 7),
                Created = DateTime.UtcNow,
                Modified = DateTime.UtcNow
            };

			// Update employee
			var updatedEmployee = await _httpClientHelper.PostAsync(BASE_URL + "update", newEmployee).ConfigureAwait(false);

			// First name should be a new one
			Assert.Equal("Johnny", updatedEmployee.FirstName);
		}
	}
}
