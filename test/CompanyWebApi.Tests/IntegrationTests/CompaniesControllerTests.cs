using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using CompanyWebApi.Contracts.Dto;
using CompanyWebApi.Contracts.Entities;
using CompanyWebApi.Tests.Services;
using Xunit;

namespace CompanyWebApi.Tests.IntegrationTests
{
	public class CompaniesControllerTests : ControllerTestsBase
	{
		private const string BASE_URL = "/api/v1.1/companies/";
		private readonly HttpClientHelper _httpClientHelper;

		public CompaniesControllerTests(WebApiTestFactory factory)
			: base(factory)
		{
			_httpClientHelper = new HttpClientHelper(Client);
			_httpClientHelper.Client.SetFakeBearerToken((object)Token);
		}

		[Fact]
		public async Task CanCreateAndDeleteCompanies()
		{
			var newCompany = new Company { CompanyId = 999, Name = "Test Company",  Created = DateTime.UtcNow, Modified = DateTime.UtcNow };
			var company = await _httpClientHelper.PostAsync(BASE_URL + "create", newCompany).ConfigureAwait(false);
			Assert.Equal("Test Company", company.Name);
			await _httpClientHelper.DeleteAsync(BASE_URL + "DeleteCompanyById/999").ConfigureAwait(false);
		}

		[Fact]
		public async Task CanGetAllCompanies()
		{
			var companies = await _httpClientHelper.GetAsync<List<CompanyDto>>(BASE_URL + "getAll").ConfigureAwait(false);
			Assert.Contains(companies, p => p.Name == "Company Two");
		}

		[Fact]
		public async Task CanGetCompany()
		{
			var company = await _httpClientHelper.GetAsync<CompanyDto>(BASE_URL + "3").ConfigureAwait(false);
			Assert.Equal(3, company.CompanyId);
			Assert.Equal("Company Three", company.Name);
		}

		[Fact]
		public async Task CanUpdateCompany()
		{
            var companyToUpdate = new Company { CompanyId = 1, Name = "Test Company",  Created = DateTime.UtcNow, Modified = DateTime.UtcNow };
			var updatedCompany = await _httpClientHelper.PostAsync(BASE_URL + "update", companyToUpdate).ConfigureAwait(false);
			Assert.Equal("Test Company", updatedCompany.Name);
		}
	}
}
