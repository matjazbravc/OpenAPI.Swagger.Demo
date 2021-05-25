using CompanyWebApi.Contracts.Entities;
using CompanyWebApi.Persistence.DbContexts;
using System.Linq;
using System;

namespace CompanyWebApi.Services
{
	/// <summary>
	/// Class for seeding test data
	/// </summary>
	public class TestDataSeeder
	{
        private readonly ApplicationDbContext _dbContext;

        public TestDataSeeder(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbContext.Database.EnsureDeleted();
            _dbContext.Database.EnsureCreated();
		}
  
		public void SeedTestData()
		{
			if (!_dbContext.Companies.Any())
			{
				_dbContext.Companies.AddRange(
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

			if (!_dbContext.Departments.Any())
			{
				_dbContext.Departments.AddRange(
					new Department { CompanyId = 1, DepartmentId = 1, Name = "Logistics" },
					new Department { CompanyId = 1, DepartmentId = 2, Name = "Administration" },
					new Department { CompanyId = 1, DepartmentId = 3, Name = "Development" },

                    new Department { CompanyId = 2, DepartmentId = 4, Name = "Sales" },
                    new Department { CompanyId = 2, DepartmentId = 5, Name = "Marketing" },
                    new Department { CompanyId = 3, DepartmentId = 6, Name = "Customer support" },

                    new Department { CompanyId = 3, DepartmentId = 7, Name = "Research and Development" },
                    new Department { CompanyId = 3, DepartmentId = 8, Name = "Purchasing" },
                    new Department { CompanyId = 3, DepartmentId = 9, Name = "Human Resource Management" },
                    new Department { CompanyId = 3, DepartmentId = 10, Name = "Accounting and Finance" },
                    new Department { CompanyId = 3, DepartmentId = 11, Name = "Production" }
				);
			}

			if (!_dbContext.Employees.Any())
			{
				_dbContext.Employees.AddRange(
					new Employee
					{
						CompanyId = 1,
                        DepartmentId = 1,
						EmployeeId = 1,
						FirstName = "John",
						LastName = "Whyne",
						BirthDate = new DateTime(1991, 8, 7),
						Created = DateTime.UtcNow,
						Modified = DateTime.UtcNow
					},
					new Employee
					{
                        CompanyId = 2,
                        DepartmentId = 4,
						EmployeeId = 2,
						FirstName = "Mathias",
						LastName = "Gernold",
						BirthDate = new DateTime(1997, 10, 12),
						Created = DateTime.UtcNow,
						Modified = DateTime.UtcNow
					},
					new Employee
					{
                        CompanyId = 3,
                        DepartmentId = 7,
						EmployeeId = 3,
						FirstName = "Julia",
						LastName = "Reynolds",
						BirthDate = new DateTime(1955, 12, 16),
						Created = DateTime.UtcNow,
						Modified = DateTime.UtcNow
					},
					new Employee
					{
                        CompanyId = 1,
                        DepartmentId = 2,
						EmployeeId = 4,
						FirstName = "Alois",
						LastName = "Mock",
						BirthDate = new DateTime(1935, 2, 9),
						Created = DateTime.UtcNow,
						Modified = DateTime.UtcNow
					},
					new Employee
					{
						CompanyId = 2,
                        DepartmentId = 6,
						EmployeeId = 5,
						FirstName = "Gertraud",
						LastName = "Bochold",
						BirthDate = new DateTime(2001, 3, 4),
						Created = DateTime.UtcNow,
						Modified = DateTime.UtcNow
					}
                    ,
                    new Employee
                    {
						CompanyId = 2,
                        DepartmentId = 6,
                        EmployeeId = 6,
                        FirstName = "Alan",
                        LastName = "Ford",
                        BirthDate = new DateTime(1984, 6, 15),
                        Created = DateTime.UtcNow,
                        Modified = DateTime.UtcNow
                    }
				);

				if (!_dbContext.EmployeeAddresses.Any())
				{
					_dbContext.EmployeeAddresses.AddRange(
						new EmployeeAddress { EmployeeId = 1, Address = "Kentucky, USA" },
						new EmployeeAddress { EmployeeId = 2, Address = "Berlin, Germany" },
						new EmployeeAddress { EmployeeId = 3, Address = "Los Angeles, USA" },
						new EmployeeAddress { EmployeeId = 4, Address = "Vienna, Austria" },
						new EmployeeAddress { EmployeeId = 5, Address = "Cologne, Germany" },
                        new EmployeeAddress { EmployeeId = 6, Address = "Milano, Italy" }
					);
				}

				if (!_dbContext.Users.Any())
				{
					_dbContext.Users.AddRange(
						new User { EmployeeId = 1, Username = "johnw", Password = "test", Token = string.Empty },
						new User { EmployeeId = 2, Username = "mathiasg", Password = "test", Token = string.Empty },
						new User { EmployeeId = 3, Username = "juliar", Password = "test", Token = string.Empty },
						new User { EmployeeId = 4, Username = "aloism", Password = "test", Token = string.Empty },
						new User { EmployeeId = 5, Username = "gertraudb", Password = "test", Token = string.Empty },
						new User { EmployeeId = 6, Username = "alanf", Password = "test", Token = string.Empty }
					);
				}

				_dbContext.SaveChanges();
			}
		}
	}
}
