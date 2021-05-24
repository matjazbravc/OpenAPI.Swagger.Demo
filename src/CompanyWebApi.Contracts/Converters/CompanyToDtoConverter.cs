using CompanyWebApi.Contracts.Dto;
using CompanyWebApi.Contracts.Entities;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace CompanyWebApi.Contracts.Converters
{
	/// <summary>
	/// Company to CompanyDto converter
	/// </summary>
	public class CompanyToDtoConverter : IConverter<Company, CompanyDto>, IConverter<IList<Company>, IList<CompanyDto>>
	{
		private readonly ILogger<CompanyToDtoConverter> _logger;

		public CompanyToDtoConverter(ILogger<CompanyToDtoConverter> logger)
		{
			_logger = logger;
		}

		public CompanyDto Convert(Company company)
		{
			_logger.LogDebug("Convert");
			var companyDto = new CompanyDto
			{
				CompanyId = company.CompanyId,
				Name = company.Name
			};
			foreach (var department in company.Departments)
            {
                foreach (var employee in department.Employees)
                {
                    var address = employee.EmployeeAddress == null ? string.Empty : employee.EmployeeAddress.Address;
                    var username = employee.User == null ? string.Empty : employee.User.Username;
                    var employeeDto = $"{employee.FirstName} {employee.LastName}, Address: {address}, Department: {department.Name}, Username: {username}";
                    companyDto.Employees.Add(employeeDto);
                }
			}
			return companyDto;
		}

		public IList<CompanyDto> Convert(IList<Company> companies)
		{
			_logger.LogDebug("ConvertList");
			return companies.Select(Convert).ToList();
		}
	}
}
