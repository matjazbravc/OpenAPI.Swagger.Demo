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
	public class CompanyToDtoCoverter : IConverter<Company, CompanyDto>, IConverter<IList<Company>, IList<CompanyDto>>
	{
		private readonly ILogger<CompanyToDtoCoverter> _logger;

		public CompanyToDtoCoverter(ILogger<CompanyToDtoCoverter> logger)
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
			foreach (var employee in company.Employees)
            {
                var address = employee.EmployeeAddress == null ? "N/A" : employee.EmployeeAddress.Address;
                var department = employee.Department == null ? "N/A" : employee.Department.Name;
                var username = employee.User == null ? "N/A" : employee.User.Username;
                var employeeDto = $"{employee.FirstName} {employee.LastName}, Address: {address}, Department: {department}, Username: {username}";
                companyDto.Employees.Add(employeeDto);
			}
			return companyDto;
		}

		public IList<CompanyDto> Convert(IList<Company> companies)
		{
			_logger.LogDebug("ConvertList");
			return companies.Select(cmp =>
			{
				var companyDto = Convert(cmp);
				return companyDto;
			}).ToList();
		}
	}
}
