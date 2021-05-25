using CompanyWebApi.Contracts.Entities;
using System;
using Xunit;

namespace CompanyWebApi.Tests.Factories
{
    /// <summary>
    /// Employee test factory
    /// </summary>
    public static class EmployeeTestFactory
    {
        /// <summary>
        /// Check if employee is adult
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        public static bool IsAdult(Employee employee)
        {
            return employee.Age >= 18;
        }

        /// <summary>
        /// Generate test employee data
        /// </summary>
        public static TheoryData<Employee> Employees =>
            new()
            {
                new Employee
                {
                    CompanyId = 1,
                    DepartmentId = 1,
                    FirstName = "Johanna",
                    LastName = "Clinger",
                    BirthDate = new DateTime(1991, 8, 7)
                },
                new Employee
                {
                    CompanyId = 2,
                    DepartmentId = 4,
                    FirstName = "Gert",
                    LastName = "Weinsdorf",
                    BirthDate = new DateTime(1997, 10, 12)
                },
                new Employee
                {
                    CompanyId = 3,
                    DepartmentId = 1,
                    FirstName = "Felix",
                    LastName = "Buntrock",
                    BirthDate = new DateTime(1999, 11, 20)
                }
            };
    }
}
