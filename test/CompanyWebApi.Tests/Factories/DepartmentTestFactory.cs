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
                new Department { CompanyId = 1, Name = "Test Logistics" },
                new Department { CompanyId = 1, Name = "Test Administration" },
                new Department { CompanyId = 1, Name = "Test Development" },

                new Department { CompanyId = 2, Name = "Test Sales" },
                new Department { CompanyId = 2, Name = "Test Marketing" },
                new Department { CompanyId = 3, Name = "Test Customer support" },

                new Department { CompanyId = 3, Name = "Test Research and Development" },
                new Department { CompanyId = 3, Name = "Test Purchasing" },
                new Department { CompanyId = 3, Name = "Test Human Resource Management" },
                new Department { CompanyId = 3, Name = "Test Accounting and Finance" },
                new Department { CompanyId = 3, Name = "Test Production" }
            };
    }
}
