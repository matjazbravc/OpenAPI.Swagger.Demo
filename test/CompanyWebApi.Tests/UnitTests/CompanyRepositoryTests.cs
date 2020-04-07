using System;
using System.Threading.Tasks;
using CompanyWebApi.Contracts.Entities;
using CompanyWebApi.Services.Repositories;
using CompanyWebApi.Tests.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace CompanyWebApi.Tests.UnitTests
{
    [Collection("Sequential")]
    public class CompanyRepositoryTests : IClassFixture<WebApiTestFactory>
    {
        private readonly ILogger _logger;
        private readonly WebApiTestFactory _factory;
        private readonly ICompanyRepository _companyRepository;

        public CompanyRepositoryTests(WebApiTestFactory factory)
        {
            _factory = factory;
            _logger = _factory.Services.GetRequiredService<ILogger<WebApiTestFactory>>();
            _companyRepository = factory.Services.GetRequiredService<ICompanyRepository>();
        }

        [Fact]
        public async Task CanAdd()
        {
            _logger.LogInformation("CanAdd");
            var company = new Company { CompanyId = 999, Name = "New Company", Created = DateTime.UtcNow, Modified = DateTime.UtcNow };
            var newCompany = await _companyRepository.AddAsync(company).ConfigureAwait(false);
            Assert.Equal("New Company", newCompany.Name);
        }

        [Fact]
        public async Task CanCount()
        {
            var nrCompanies = await _companyRepository.CountAsync().ConfigureAwait(false);
            Assert.True(nrCompanies > 0);
        }

        [Fact]
        public async Task CanDelete()
        {
            var newComp = new Company { CompanyId = 9999, Name = "Delete Company", Created = DateTime.UtcNow, Modified = DateTime.UtcNow };
            var newCompany = await _companyRepository.AddAsync(newComp).ConfigureAwait(false);
            var result = await _companyRepository.DeleteAsync(newCompany).ConfigureAwait(false);
            Assert.True(result > 0);
        }

        [Fact]
        public async Task CanGetAllByPredicate()
        {
            var companies = await _companyRepository.GetAllAsync(cmp => cmp.Name.Equals("Company Two")).ConfigureAwait(false);
            Assert.True(companies.Count > 0);
        }

        [Fact]
        public async Task CanGetSingle()
        {
            var company = await _companyRepository.GetSingleAsync(cmp => cmp.Name.Equals("Company One")).ConfigureAwait(false);
            Assert.Equal("Company One", company.Name);
        }

        [Fact]
        public async Task CanGetAll()
        {
            var companies = await _companyRepository.GetAllAsync().ConfigureAwait(false);
            Assert.True(companies.Count > 0);
        }

        [Fact]
        public async Task CanUpdate()
        {
            var company = new Company { CompanyId = 3, Name = "Updated Test Company", Created = DateTime.UtcNow, Modified = DateTime.UtcNow };
            var updatedCompany = await _companyRepository.UpdateAsync(company).ConfigureAwait(false);
            Assert.Equal("Updated Test Company", updatedCompany.Name);
        }
    }
}
