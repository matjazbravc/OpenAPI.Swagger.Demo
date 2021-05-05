using CompanyWebApi.Contracts.Entities;
using CompanyWebApi.Persistence.Repositories;
using CompanyWebApi.Tests.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using System;
using Xunit;

namespace CompanyWebApi.Tests.UnitTests
{

    public class EmployeeRepositoryTests : IClassFixture<WebApiTestFactory>
    {
        private readonly ILogger _logger;
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeRepositoryTests(WebApiTestFactory factory)
        {
            _logger = factory.Services.GetRequiredService<ILogger<WebApiTestFactory>>();
            _employeeRepository = factory.Services.GetRequiredService<IEmployeeRepository>();
        }

        [Fact]
        public async Task CanAdd()
        {
            _logger.LogInformation("CanAdd");
            var employee = new Employee
            {
                EmployeeId = 999,
                FirstName = "Test",
                LastName = "Tester",
                BirthDate = new DateTime(2001, 12, 16),
                CompanyId = 1,
                DepartmentId = 1,
                Created = DateTime.UtcNow,
                Modified = DateTime.UtcNow
            };
            var newEmployee = await _employeeRepository.AddAsync(employee).ConfigureAwait(false);
            Assert.Equal("Tester", newEmployee.LastName);
        }

        [Fact]
        public async Task CanCount()
        {
            var numEmployees = await _employeeRepository.CountAsync().ConfigureAwait(false);
            Assert.True(numEmployees > 0);
        }

        [Fact]
        public async Task CanDelete()
        {
            var employee = new Employee
            {
                EmployeeId = 9999,
                FirstName = "Test",
                LastName = "Tester",
                BirthDate = new DateTime(2001, 12, 16),
                CompanyId = 1,
                DepartmentId = 1,
                Created = DateTime.UtcNow,
                Modified = DateTime.UtcNow
            };            
            var newEmployee = await _employeeRepository.AddAsync(employee).ConfigureAwait(false);
            var result = await _employeeRepository.DeleteAsync(newEmployee).ConfigureAwait(false);

            Assert.True(result > 0);
        }

        [Fact]
        public async Task CanGetAllByPredicate()
        {
            var employees = await _employeeRepository.GetAllAsync(cmp => cmp.FirstName.Equals("Julia")).ConfigureAwait(false);
            Assert.True(employees.Count() > 0);
        }

        [Fact]
        public async Task CanGetSingle()
        {
            var employee = await _employeeRepository.GetSingleAsync(cmp => cmp.FirstName.Equals("Alois") && cmp.LastName.Equals("Mock")).ConfigureAwait(false);
            Assert.True(employee != null);
        }

        [Fact]
        public async Task CanGetAll()
        {
            var employees = await _employeeRepository.GetAllAsync().ConfigureAwait(false);
            Assert.True(employees.Count() > 0);
        }

        [Fact]
        public async Task CanUpdate()
        {
            var employee = new Employee
            {
                EmployeeId = 3,
                FirstName = "Julia",
                LastName = "Reynolds Updated",
                BirthDate = new DateTime(1955, 12, 16),
                CompanyId = 1,
                DepartmentId = 3,
                Created = DateTime.UtcNow,
                Modified = DateTime.UtcNow
            };
            var updatedEmployee = await _employeeRepository.UpdateAsync(employee).ConfigureAwait(false);
            Assert.Equal("Reynolds Updated", updatedEmployee.LastName);
        }
    }
}
