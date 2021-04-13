using CompanyWebApi.Contracts.Entities;
using CompanyWebApi.Persistence.DbContexts;
using System.Linq;
using System;

namespace CompanyWebApi.Services
{
	public static class SeedData
	{
		//https://www.tektutorialshub.com/entity-framework-core-data-seeding/
		public static void Initialize(ApplicationDbContext context)
		{
			// Look for any records
			if (!context.Companies.Any())
			{
				context.Companies.AddRange(
					new Company
					{
						CompanyId = 1,
						Name = "Company One",
						Created = DateTime.UtcNow,
						Modified = DateTime.UtcNow
					},
					new Company
					{
						CompanyId = 2,
						Name = "Company Two",
						Created = DateTime.UtcNow,
						Modified = DateTime.UtcNow
					},
					new Company
					{
						CompanyId = 3,
						Name = "Company Three",
						Created = DateTime.UtcNow,
						Modified = DateTime.UtcNow
					}
				);
			}

			if (!context.Departments.Any())
			{
				context.Departments.AddRange(
					new Department { DepartmentId = 1, Name = "HR" },
					new Department { DepartmentId = 2, Name = "Admin" },
					new Department { DepartmentId = 3, Name = "Development" }
				);
			}

			if (!context.Employees.Any())
			{
				context.Employees.AddRange(
					new Employee
					{
						EmployeeId = 1,
						FirstName = "John",
						LastName = "Whyne",
						BirthDate = new DateTime(1991, 8, 7),
						CompanyId = 1,
						DepartmentId = 1,
						Created = DateTime.UtcNow,
						Modified = DateTime.UtcNow
					},
					new Employee
					{
						EmployeeId = 2,
						FirstName = "Mathias",
						LastName = "Gernold",
						BirthDate = new DateTime(1997, 10, 12),
						CompanyId = 1,
						DepartmentId = 2,
						Created = DateTime.UtcNow,
						Modified = DateTime.UtcNow
					},
					new Employee
					{
						EmployeeId = 3,
						FirstName = "Julia",
						LastName = "Reynolds",
						BirthDate = new DateTime(1955, 12, 16),
						CompanyId = 1,
						DepartmentId = 3,
						Created = DateTime.UtcNow,
						Modified = DateTime.UtcNow
					},
					new Employee
					{
						EmployeeId = 4,
						FirstName = "Alois",
						LastName = "Mock",
						BirthDate = new DateTime(1935, 2, 9),
						CompanyId = 2,
						DepartmentId = 1,
						Created = DateTime.UtcNow,
						Modified = DateTime.UtcNow
					},
					new Employee
					{
						EmployeeId = 5,
						FirstName = "Gertraud",
						LastName = "Bochold",
						BirthDate = new DateTime(2001, 3, 4),
						CompanyId = 3,
						DepartmentId = 2,
						Created = DateTime.UtcNow,
						Modified = DateTime.UtcNow
					}
				);

				if (!context.EmployeeAddresses.Any())
				{
					context.EmployeeAddresses.AddRange(
						new EmployeeAddress { EmployeeId = 1, Address = "Bangalore, India" },
						new EmployeeAddress { EmployeeId = 2, Address = "Newyork, USA" },
						new EmployeeAddress { EmployeeId = 3, Address = "California, USA" },
						new EmployeeAddress { EmployeeId = 4, Address = "NewDelhi, India" },
						new EmployeeAddress { EmployeeId = 5, Address = "Kentuki, USA" }
					);
				}

				if (!context.Users.Any())
				{
					context.Users.AddRange(
						new User { EmployeeId = 1, Username = "johnw", Password = "test", Token = string.Empty },
						new User { EmployeeId = 2, Username = "mathiasg", Password = "test", Token = string.Empty },
						new User { EmployeeId = 3, Username = "juliar", Password = "test", Token = string.Empty },
						new User { EmployeeId = 4, Username = "aloism", Password = "test", Token = string.Empty },
						new User { EmployeeId = 5, Username = "gertraudb", Password = "test", Token = string.Empty }
					);
				}

				context.SaveChanges();
			}
		}
	}
}
