using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using CompanyWebApi.Contracts.Entities;
using CompanyWebApi.Contracts.Dto;
using CompanyWebApi.Tests.Services;
using Xunit;

namespace CompanyWebApi.Tests.IntegrationTests
{
	public class DepartmentsControllerTests : ControllerTestsBase
	{
		private const string BASE_URL = "/api/v1.1/departments/";
		private readonly HttpClientHelper _httpClientHelper;

		public DepartmentsControllerTests(WebApiTestFactory factory)
			: base(factory)
		{
			_httpClientHelper = new HttpClientHelper(Client);
			_httpClientHelper.Client.SetFakeBearerToken((object)Token);
		}

		[Fact]
		public async Task CanCreateAndDelete()
		{
			var newDepartment = new Department { DepartmentId = 999, Name = "TEST DEPARTMENT" };
			var department = await _httpClientHelper.PostAsync(BASE_URL + "create", newDepartment).ConfigureAwait(false);
			Assert.Equal("TEST DEPARTMENT", department.Name);
			await _httpClientHelper.DeleteAsync(BASE_URL + "DeleteDepartmentById/999").ConfigureAwait(false);
		}

		[Fact]
		public async Task CanGetAll()
		{
			var departments = await _httpClientHelper.GetAsync<List<DepartmentDto>>(BASE_URL + "getall").ConfigureAwait(false);
			Assert.Contains(departments, p => p.Name == "Development");
		}

		[Fact]
		public async Task CanGetSingle()
		{
			var department = await _httpClientHelper.GetAsync<DepartmentDto>(BASE_URL + "3").ConfigureAwait(false);
			Assert.Equal(3, department.DepartmentId);
			Assert.Equal("Development", department.Name);
		}

		[Fact]
		public async Task CanUpdate()
		{
            var departmentToUpdate = new Department { DepartmentId = 1, Name = "TEST DEPARTMENT",  Created = DateTime.UtcNow, Modified = DateTime.UtcNow };
			var updatedDepartment = await _httpClientHelper.PostAsync(BASE_URL + "update", departmentToUpdate).ConfigureAwait(false);
			Assert.Equal("TEST DEPARTMENT", updatedDepartment.Name);
		}
	}
}
