using CompanyWebApi.Contracts.Dto;
using CompanyWebApi.Contracts.Entities;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace CompanyWebApi.Contracts.Converters
{
	/// <summary>
	/// Department to DepartmentDto converter
	/// </summary>
	public class DepartmentToDtoConverter : IConverter<Department, DepartmentDto>, IConverter<IList<Department>, IList<DepartmentDto>>
	{
		private readonly ILogger<DepartmentToDtoConverter> _logger;

		public DepartmentToDtoConverter(ILogger<DepartmentToDtoConverter> logger)
		{
			_logger = logger;
		}

		public DepartmentDto Convert(Department department)
		{
			_logger.LogDebug("Convert");
			var departmentDto = new DepartmentDto
			{
				DepartmentId = department.DepartmentId,
				Name = department.Name
			};
			foreach (var employee in department.Employees)
            {
                var addressStr = employee.EmployeeAddress == null ? "N/A" : employee.EmployeeAddress.Address;
                var departmentStr = employee.Department == null ? "N/A" : employee.Department.Name;
                var username = employee.User == null ? "N/A" : employee.User.Username;
                var employeeDto = $"{employee.FirstName} {employee.LastName}, Address: {addressStr}, Department: {departmentStr}, Username: {username}";
                departmentDto.Employees.Add(employeeDto);
			}
			return departmentDto;
		}

		public IList<DepartmentDto> Convert(IList<Department> companies)
		{
			_logger.LogDebug("ConvertList");
			return companies.Select(cmp =>
			{
				var departmentDto = Convert(cmp);
				return departmentDto;
			}).ToList();
		}
	}
}
