using CompanyWebApi.Contracts.Entities;
using System;
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
                new Company
                {
                    Name = "Test Company One",
                    Created = DateTime.UtcNow,
                    Modified = DateTime.UtcNow
                },
                new Company
                {
                    Name = "Test Company Two",
                    Created = DateTime.UtcNow,
                    Modified = DateTime.UtcNow
                },
                new Company
                {
                    Name = "Test Company Three",
                    Created = DateTime.UtcNow,
                    Modified = DateTime.UtcNow
                }
            };
    }
}
