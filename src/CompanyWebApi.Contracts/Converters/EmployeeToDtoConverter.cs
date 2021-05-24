using CompanyWebApi.Contracts.Dto;
using CompanyWebApi.Contracts.Entities;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace CompanyWebApi.Contracts.Converters
{
	/// <summary>
	/// Employee to EmployeeDto converter
	/// </summary>
	public class EmployeeToDtoConverter : IConverter<Employee, EmployeeDto>, IConverter<IList<Employee>, IList<EmployeeDto>>
	{
		private readonly ILogger<EmployeeToDtoConverter> _logger;

		public EmployeeToDtoConverter(ILogger<EmployeeToDtoConverter> logger)
		{
			_logger = logger;
		}

		public EmployeeDto Convert(Employee employee)
		{
			_logger.LogDebug("Convert");
			var employeeDto = new EmployeeDto
			{
				EmployeeId = employee.EmployeeId,
				FirstName = employee.FirstName,
				LastName = employee.LastName,
                Address = employee.EmployeeAddress == null ? string.Empty : employee.EmployeeAddress.Address,
                Age = employee.Age,
                BirthDate = employee.BirthDate,
                Username = employee.User == null ? string.Empty : employee.User.Username,
                CompanyId = employee.Company?.CompanyId ?? 0,
                Company = employee.Company == null ? string.Empty : employee.Company.Name,
                DepartmentId = employee.Department?.DepartmentId ?? 0,
                Department = employee.Department == null ? string.Empty : employee.Department.Name
			};
			return employeeDto;
		}

		public IList<EmployeeDto> Convert(IList<Employee> employees)
		{
			_logger.LogDebug("ConvertList");
			return employees.Select(Convert).ToList();
		}
	}
}
