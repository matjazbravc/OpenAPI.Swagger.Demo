using CompanyWebApi.Contracts.Entities;
using CompanyWebApi.Persistence.Repositories;
using CompanyWebApi.Tests.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CompanyWebApi.Tests.UnitTests
{
    [Collection("Sequential")]
    public class DepartmentRepositoryTests : IClassFixture<WebApiTestFactory>
    {
        private readonly ILogger _logger;
        private readonly IDepartmentRepository _departmentRepository;

        public DepartmentRepositoryTests(WebApiTestFactory factory)
        {
            _logger = factory.Services.GetRequiredService<ILogger<WebApiTestFactory>>();
            _departmentRepository = factory.Services.GetRequiredService<IDepartmentRepository>();
        }

        [Fact]
        public async Task CanAdd()
        {
            _logger.LogInformation("CanAdd");
            var department = new Department
            {
                CompanyId = 1,
                DepartmentId = 999,
                Name = "TEST DEPARTMENT"
            };
            var repoDepartment = await _departmentRepository.AddDepartmentAsync(department).ConfigureAwait(false);
            Assert.Equal("TEST DEPARTMENT", repoDepartment.Name);
        }

        [Fact]
        public async Task CanAddRange()
        {
            var departments = new List<Department>
            {
                new()
                {
                    CompanyId = 1, DepartmentId = 1111, Name = "TEST1"
                },
                new()
                {
                    CompanyId = 1, DepartmentId = 2222, Name = "TEST2"
                },
                new()
                {
                    CompanyId = 1, DepartmentId = 3333, Name = "TEST3"
                }
            };
            await _departmentRepository.AddAsync(departments).ConfigureAwait(false);
            await _departmentRepository.SaveAsync().ConfigureAwait(false);
            Assert.True(departments.Count > 0);
        }

        [Fact]
        public async Task CanCount()
        {
            var nrCompanies = await _departmentRepository.CountAsync().ConfigureAwait(false);
            Assert.True(nrCompanies > 0);
        }

        [Fact]
        public async Task CanDelete()
        {
            var department = new Department
            {
                CompanyId = 1,
                DepartmentId = 9999, 
                Name = "TEST DEPARTMENT"
            };
            await _departmentRepository.AddDepartmentAsync(department).ConfigureAwait(false);
            _departmentRepository.Remove(department);
            await _departmentRepository.SaveAsync().ConfigureAwait(false);
            var repoDepartment = await _departmentRepository.GetDepartmentAsync(department.DepartmentId).ConfigureAwait(false);
            Assert.Null(repoDepartment);
        }

        [Fact]
        public async Task CanGetByPredicate()
        {
            var department = await _departmentRepository.GetDepartmentAsync(dep => dep.Name.Equals("Development")).ConfigureAwait(false);
            Assert.True(department != null);
        }

        [Fact]
        public async Task CanGetAll()
        {
            var companies = await _departmentRepository.GetDepartmentsAsync().ConfigureAwait(false);
            Assert.True(companies.Any());
        }

        [Fact]
        public async Task CanUpdate()
        {
            var department = new Department
            {
                DepartmentId = 1, Name = "HR Updated"
            };
            await _departmentRepository.UpdateAsync(department).ConfigureAwait(false);
            await _departmentRepository.SaveAsync().ConfigureAwait(false);
            Assert.Equal("HR Updated", department.Name);
        }
    }
}
