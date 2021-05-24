using CompanyWebApi.Contracts.Dto;
using CompanyWebApi.Contracts.Entities;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace CompanyWebApi.Contracts.Converters
{
    /// <summary>
    /// Employee from EmployeeCreateDto converter
    /// </summary>
    public class EmployeeFromDtoConverter : IConverter<EmployeeCreateDto, Employee>, IConverter<IList<EmployeeCreateDto>, IList<Employee>>
    {
        private readonly ILogger<EmployeeFromDtoConverter> _logger;

        public EmployeeFromDtoConverter(ILogger<EmployeeFromDtoConverter> logger)
        {
            _logger = logger;
        }

        public Employee Convert(EmployeeCreateDto employee)
        {
            _logger.LogDebug("Convert");
            var employeeDto = new Employee
            {
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                CompanyId = employee.CompanyId,
                DepartmentId = employee.DepartmentId,
                BirthDate = employee.BirthDate,
                EmployeeAddress = new EmployeeAddress
                {
                    Address = string.IsNullOrEmpty(employee.Address) ? string.Empty : employee.Address
                }
            };

            if (!string.IsNullOrEmpty(employee.Username) &&
                !string.IsNullOrEmpty(employee.Password))
            {
                employeeDto.User = new User
                {
                    Username = string.IsNullOrEmpty(employee.Username) ? string.Empty : employee.Username,
                    Password = string.IsNullOrEmpty(employee.Password) ? string.Empty : employee.Password
                };
            }

            return employeeDto;
        }

        public IList<Employee> Convert(IList<EmployeeCreateDto> employees)
        {
            _logger.LogDebug("ConvertList");
            return employees.Select(Convert).ToList();
        }
    }
}
