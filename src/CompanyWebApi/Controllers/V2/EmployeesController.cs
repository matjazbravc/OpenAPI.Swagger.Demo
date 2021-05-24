using CompanyWebApi.Contracts.Converters;
using CompanyWebApi.Contracts.Dto;
using CompanyWebApi.Contracts.Entities;
using CompanyWebApi.Controllers.Base;
using CompanyWebApi.Persistence.Repositories.Factory;
using CompanyWebApi.Services.Filters;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CompanyWebApi.Controllers.V2
{
    [ApiAuthorization]
    [ApiController]
    [ApiVersion("2.0")]
    [Produces("application/json")]
    [EnableCors("EnableCORS")]
    [ServiceFilter(typeof(ValidModelStateAsyncActionFilter))]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class EmployeesController : BaseController<EmployeesController>
    {
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IConverter<Employee, EmployeeDto> _employeeToDtoConverter;
        private readonly IConverter<IList<Employee>, IList<EmployeeDto>> _employeeToDtoListConverter;
        private readonly IConverter<EmployeeCreateDto, Employee> _employeeFromDtoConverter;

        public EmployeesController(IRepositoryFactory repositoryFactory,
            IConverter<Employee, EmployeeDto> employeeToDtoConverter,
            IConverter<IList<Employee>, IList<EmployeeDto>> employeeToDtoListConverter,
            IConverter<EmployeeCreateDto, Employee> employeeFromDtoConverter)
        {
            _repositoryFactory = repositoryFactory;
            _employeeToDtoConverter = employeeToDtoConverter;
            _employeeToDtoListConverter = employeeToDtoListConverter;
            _employeeFromDtoConverter = employeeFromDtoConverter;
        }

        /// <summary>
        /// Add a new employee
        /// </summary>
        /// <remarks>
        /// Sample request body:
        ///
        ///     {
        ///       "firstName": "Alan",
        ///       "lastName": "Ford",
        ///       "birthDate": "1971-05-18",
        ///       "companyId": 1,
        ///       "departmentId": 2,
        ///       "address": "Hamilton Street 145/a, 10007 NY",
        ///       "username": "alanf",
        ///       "password": "tntgroup!129"
        ///     }
        /// 
        /// Sample response body:
        /// 
        ///     {
        ///       "employeeId": 8,
        ///       "firstName": "Alan",
        ///       "lastName": "Ford",
        ///       "birthDate": "1971-05-18T00:00:00",
        ///       "age": 50,
        ///       "company": "N/A",
        ///       "department": "N/A",
        ///       "address": "Hamilton Street 145/a, 10007 NY",
        ///       "username": "alanf"
        ///     }
        /// </remarks>
        /// <param name="employee">EmployeeCreateDto model</param>
        /// <param name="version">API version</param>
        [SwaggerResponse(StatusCodes.Status201Created, Type = typeof(EmployeeDto), Description = "Returns a new employee")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Company or Department was not found")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized user")]
        [HttpPost("create", Name = "CreateEmployeeV2")]
        public async Task<IActionResult> CreateAsync([FromBody] EmployeeCreateDto employee, ApiVersion version)
        {
            Logger.LogDebug(nameof(CreateAsync));
            var newEmployee = _employeeFromDtoConverter.Convert(employee);
            if (!await _repositoryFactory.CompanyRepository.ExistsAsync(c => c.CompanyId == employee.CompanyId).ConfigureAwait(false))
            {
                return NotFound(new { message = $"The Company with id {employee.CompanyId} was not found" });
            }
            if (!await _repositoryFactory.DepartmentRepository.ExistsAsync(c => c.DepartmentId == employee.DepartmentId).ConfigureAwait(false))
            {
                return NotFound(new { message = $"The Department with id {employee.DepartmentId} was not found" });
            }

            var repoEmployee = await _repositoryFactory.EmployeeRepository.AddEmployeeAsync(newEmployee).ConfigureAwait(false);
            var result = _employeeToDtoConverter.Convert(repoEmployee);
            var createdResult = new ObjectResult(result)
            {
                StatusCode = StatusCodes.Status201Created
            };
            return createdResult;
        }

        /// <summary>
        /// Deletes a employee with Id
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE /api/v2/employees/1
        ///
        /// Sample response body:
        ///     
        ///    Code 200 Success
        /// 
        /// </remarks>
        /// <param name="id" example="1">Employee Id</param>
        /// <param name="version">API version</param>
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Employee was successfuly deleted")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "No employee was found")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized user")]
        [HttpDelete("{id:int}", Name = "DeleteEmployeeByIdV2")]
        public async Task<IActionResult> DeleteAsync(int id, ApiVersion version)
        {
            Logger.LogDebug(nameof(DeleteAsync));
            var employee = await _repositoryFactory.EmployeeRepository.GetEmployeeAsync(id).ConfigureAwait(false);
            if (employee == null)
            {
                return NotFound(new { message = "No employee was found" });
            }
            _repositoryFactory.EmployeeRepository.Remove(employee);
            await _repositoryFactory.SaveAsync().ConfigureAwait(false);
            return Ok();
        }

        /// <summary>
        /// Gets all employees
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/v2/employees/getall
        ///
        /// Sample response body:
        ///
        ///     [
        ///       {
        ///         "employeeId": 1,
        ///         "firstName": "John",
        ///         "lastName": "Whyne",
        ///         "birthDate": "1991-08-07T00:00:00",
        ///         "age": 29,
        ///         "address": "Kentucky, USA",
        ///         "username": "johnw",
        ///         "companyId": 1,
        ///         "company": "Company One",
        ///         "departmentId": 1,
        ///         "department": "Logistics"
        ///       },
        ///       {
        ///         "employeeId": 2,
        ///         "firstName": "Mathias",
        ///         "lastName": "Gernold",
        ///         "birthDate": "1997-10-12T00:00:00",
        ///         "age": 23,
        ///         "address": "Berlin, Germany",
        ///         "username": "mathiasg",
        ///         "companyId": 2,
        ///         "company": "Company Two",
        ///         "departmentId": 4,
        ///         "department": "Sales"
        ///       },
        ///       {
        ///         "employeeId": 3,
        ///         "firstName": "Julia",
        ///         "lastName": "Reynolds",
        ///         "birthDate": "1955-12-16T00:00:00",
        ///         "age": 65,
        ///         "address": "Los Angeles, USA",
        ///         "username": "juliar",
        ///         "companyId": 3,
        ///         "company": "Company Three",
        ///         "departmentId": 7,
        ///         "department": "Research and Development"
        ///       }
        ///     ]
        /// </remarks>
        /// <param name="version">API version</param>
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(IEnumerable<EmployeeDto>), Description = "Return list of employees")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "The employees list is empty")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized user")]
        [HttpGet("getAll", Name = "GetAllEmployeesV2")]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetAllAsync(ApiVersion version)
        {
            Logger.LogDebug(nameof(GetAllAsync));
            var employees = await _repositoryFactory.EmployeeRepository.GetEmployeesAsync().ConfigureAwait(false);
            if (!employees.Any())
            {
                return NotFound(new { message = "The employees list is empty" });
            }
            var employeesDto = _employeeToDtoListConverter.Convert(employees);
            return Ok(employeesDto);
        }

        /// <summary>
        /// Get a employee with Id
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/v2/employees/6
        ///
        /// Sample response body:
        /// 
        ///     {
        ///       "employeeId": 6,
        ///       "firstName": "Alan",
        ///       "lastName": "Ford",
        ///       "birthDate": "1984-06-15T00:00:00",
        ///       "age": 36,
        ///       "company": "Company Three",
        ///       "department": "Admin",
        ///       "address": "Milano, Italy",
        ///       "username": "alanf"
        ///     }
        /// </remarks>
        /// <param name="id" example="1">Employee Id</param>
        /// <param name="version">API version</param>
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(EmployeeDto), Description = "Return employee")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "The employee was not found")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized user")]
        [HttpGet("{id:int}", Name = "GetEmployeeByIdV2")]
        public async Task<ActionResult<EmployeeDto>> GetAsync(int id, ApiVersion version)
        {
            Logger.LogDebug(nameof(GetAsync));
            var employee = await _repositoryFactory.EmployeeRepository.GetEmployeeAsync(id).ConfigureAwait(false);
            if (employee == null)
            {
                return NotFound(new { message = "The employee was not found" });
            }
            var employeeDto = _employeeToDtoConverter.Convert(employee);
            return Ok(employeeDto);
        }

        /// <summary>
        /// Update a employee
        /// </summary>
        /// <remarks>
        /// Sample request body:
        ///
        ///     {
        ///       "employeeId": 1,
        ///       "firstName": "John",
        ///       "lastName": "Whyne",
        ///       "birthDate": "1965-05-31"
        ///     }
        /// 
        /// Sample response body:
        /// 
        ///     {
        ///       "employeeId": 1,
        ///       "firstName": "John",
        ///       "lastName": "Whyne",
        ///       "birthDate": "1965-05-31T00:00:00",
        ///       "age": 55,
        ///       "company": "Company One",
        ///       "department": "HR",
        ///       "address": "Bangalore, India",
        ///       "username": "johnw"
        ///     }
        /// </remarks>
        /// <param name="employee"><see cref="EmployeeUpdateDto"/></param>
        /// <param name="version">API version</param>
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(EmployeeDto), Description = "Return updated employee")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "The employee was not found")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized user")]
        [HttpPut("update", Name = "UpdateEmployeeV2")]
        public async Task<IActionResult> UpdateAsync([FromBody] EmployeeUpdateDto employee, ApiVersion version)
        {
            Logger.LogDebug(nameof(UpdateAsync));
            var repoEmployee = await _repositoryFactory.EmployeeRepository.GetEmployeeAsync(employee.EmployeeId).ConfigureAwait(false);
            if (repoEmployee == null)
            {
                return NotFound(new { message = "The employee was not found" });
            }

            repoEmployee.FirstName = employee.FirstName;
            repoEmployee.LastName = employee.LastName;
            repoEmployee.BirthDate = employee.BirthDate;

            await _repositoryFactory.EmployeeRepository.UpdateAsync(repoEmployee);
            await _repositoryFactory.SaveAsync().ConfigureAwait(false);
            var employeeDto = _employeeToDtoConverter.Convert(repoEmployee);
            return Ok(employeeDto);
        }

        /// <summary>
        /// Search for employees
        /// </summary>
        /// <param name="searchCriteria">EmployeeSearchDto model</param>
        /// <param name="version">API version</param>
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(IEnumerable<EmployeeDto>), Description = "Returns a list of employees according to search criteria")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "No employees were found")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized user")]
        [HttpGet("search", Name = "SearchEmployeeV2")]
        public async Task<IActionResult> SearcAsync([FromQuery] EmployeeSearchDto searchCriteria, ApiVersion version)
        {
            Logger.LogDebug(nameof(SearcAsync));
            var employees = await _repositoryFactory.EmployeeRepository.SearchEmployeesAsync(searchCriteria).ConfigureAwait(false);
            if (!employees.Any())
            {
                return NotFound(new { message = "No employees were found" });
            }
            var employeesDto = _employeeToDtoListConverter.Convert(employees);
            return Ok(employeesDto);
        }
    }
}
