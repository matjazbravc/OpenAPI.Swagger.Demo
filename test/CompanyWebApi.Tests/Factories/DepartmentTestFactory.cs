using CompanyWebApi.Contracts.Entities;
using Xunit;

namespace CompanyWebApi.Tests.Factories
{
    /// <summary>
    /// Department test factory
    /// </summary>
    public static class DepartmentTestFactory
    {
        /// <summary>
        /// Generate test companies data
        /// </summary>
        public static TheoryData<Department> Departments =>
            new()
            {
                new Department { CompanyId = 1, Name = "Test Department 1" },
                new Department { CompanyId = 1, Name = "Test Department 2" },
                new Department { CompanyId = 1, Name = "Test Department 3" }
            };
    }
}
