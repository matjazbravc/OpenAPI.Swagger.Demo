using CompanyWebApi.Contracts.Entities;
using CompanyWebApi.Persistence.Repositories;
using CompanyWebApi.Tests.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;
using System.Linq;
using Xunit;

namespace CompanyWebApi.Tests.UnitTests
{
    [Collection("Sequential")]
    public class CompanyRepositoryTests : IClassFixture<WebApiTestFactory>
    {
        private readonly ILogger _logger;
        private readonly ICompanyRepository _companyRepository;

        public CompanyRepositoryTests(WebApiTestFactory factory)
        {
            _logger = factory.Services.GetRequiredService<ILogger<WebApiTestFactory>>();
            _companyRepository = factory.Services.GetRequiredService<ICompanyRepository>();
        }

        [Fact]
        public async Task CanAdd()
        {
            _logger.LogInformation("CanAdd");
            var company = new Company
            {
                CompanyId = 999, Name = "New Company", Created = DateTime.UtcNow, Modified = DateTime.UtcNow
            };
            var repoCompany = await _companyRepository.AddCompanyAsync(company).ConfigureAwait(false);
            Assert.Equal("New Company", repoCompany.Name);
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
            var company = new Company
            {
                CompanyId = 9999, Name = "Delete Company", Created = DateTime.UtcNow, Modified = DateTime.UtcNow
            };
            await _companyRepository.AddCompanyAsync(company, true).ConfigureAwait(false);
            _companyRepository.Remove(company);
            await _companyRepository.SaveAsync().ConfigureAwait(false);
            var repoCompany = await _companyRepository.GetCompanyAsync(company.CompanyId).ConfigureAwait(false);
            Assert.Null(repoCompany);
        }

        [Fact]
        public async Task CanGetAll()
        {
            var companies = await _companyRepository.GetCompaniesAsync().ConfigureAwait(false);
            Assert.True(companies.Any());
        }

        [Fact]
        public async Task CanGetSingle()
        {
            var company = await _companyRepository.GetCompanyAsync(predicate: cmp => cmp.Name.Equals("Company One")).ConfigureAwait(false);
            Assert.Equal("Company One", company.Name);
        }

        [Fact]
        public async Task CanUpdate()
        {
            var company = new Company
            {
                CompanyId = 3, Name = "Updated Test Company", Created = DateTime.UtcNow, Modified = DateTime.UtcNow
            };
            await _companyRepository.UpdateAsync(company).ConfigureAwait(false);
            await _companyRepository.SaveAsync().ConfigureAwait(false);
            Assert.Equal("Updated Test Company", company.Name);
        }
    }
}
