using CompanyWebApi.Contracts.Entities;
using Xunit;

namespace CompanyWebApi.Tests.Factories
{
    /// <summary>
    /// Company test factory
    /// </summary>
    public static class CompanyTestFactory
    {
        /// <summary>
        /// Generate test companies data
        /// </summary>
        public static TheoryData<Company> Companies =>
            new()
            {
                new Company { Name = "Test Company 1" },
                new Company { Name = "Test Company 2" },
                new Company { Name = "Test Company 3" }
            };
    }
}
