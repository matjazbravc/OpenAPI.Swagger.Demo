using CompanyWebApi.Contracts.Converters;
using CompanyWebApi.Contracts.Dto;
using CompanyWebApi.Contracts.Entities;
using CompanyWebApi.Controllers.Base;
using CompanyWebApi.Persistence.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CompanyWebApi.Controllers.V2
{
    [Authorize]
    [ApiController]
    [ApiVersion("2.0")]
    [Produces("application/json")]
    [EnableCors("EnableCORS")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class EmployeesController : BaseController<EmployeesController>
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IConverter<Employee, EmployeeDto> _employeeToDtoConverter;
        private readonly IConverter<IList<Employee>, IList<EmployeeDto>> _employeeToDtoListConverter;

        public EmployeesController(IEmployeeRepository employeeRepository,
            IConverter<Employee, EmployeeDto> employeeToDtoConverter,
            IConverter<IList<Employee>, IList<EmployeeDto>> employeeToDtoListConverter)
        {
            _employeeRepository = employeeRepository;
            _employeeToDtoConverter = employeeToDtoConverter;
            _employeeToDtoListConverter = employeeToDtoListConverter;
        }

        /// <summary>
        /// Create Employee
        /// </summary>
        /// <remarks>This API will create new Employee</remarks>
        /// POST /api/employees/create/{employee}
        /// <param name="employee">Employee model</param>
        /// <param name="version">API version</param>
        [ProducesResponseType(201, Type = typeof(Employee))]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [HttpPost("create", Name = "CreateEmployeeV2")]
        public async Task<IActionResult> CreateAsync([FromBody] Employee employee, ApiVersion version)
        {
            Logger.LogDebug("CreateAsync");
            if (employee == null)
            {
                return BadRequest(new { message = "The employee is null"});
            }
            await _employeeRepository.AddAsync(employee);
            return CreatedAtRoute("GetEmployeeByIdV2", new { id = employee.EmployeeId, version = version.ToString() }, employee);
        }

        /// <summary>
        /// Delete Employee
        /// </summary>
        /// <remarks>This API will delete Employee with Id</remarks>
        /// DELETE /api/employees/{id}
        /// <param name="id"></param>
        /// <param name="version">API version</param>
        /// <returns></returns>
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [HttpDelete("{id:int}", Name = "DeleteEmployeeByIdV2")]
        public async Task<IActionResult> DeleteAsync(int id, ApiVersion version)
        {
            Logger.LogDebug("DeleteAsync");
            var employee = await _employeeRepository.GetSingleAsync(emp => emp.EmployeeId == id);
            if (employee == null)
            {
                return NotFound(new { message = "The employee was not found"});
            }
            await _employeeRepository.DeleteAsync(employee);
            return NoContent();
        }

        /// <summary>
        /// Get all Employees
        /// </summary>
        /// <remarks>This API return list of all Employees</remarks>
        /// GET api/employees/getall
        /// <param name="version">API version</param>
        /// <returns>List of Employees</returns>
        [ProducesResponseType(200, Type = typeof(IEnumerable<EmployeeDto>))]
        [ProducesResponseType(404)]
        [HttpGet("getAll", Name = "GetAllEmployeesV2")]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetAllAsync(ApiVersion version)
        {
            Logger.LogDebug("GetAllAsync");
            var employees = await _employeeRepository.GetAllAsync().ConfigureAwait(false);
            if (!employees.Any())
            {
                return NotFound(new { message = "The employees list is empty"});
            }
            var employeesDto = _employeeToDtoListConverter.Convert(employees);
            return Ok(employeesDto);
        }

        /// <summary>
        /// Get Employee
        /// </summary>
        /// <remarks>This API return Employee with Id</remarks>
        /// GET /api/employees/{id}
        /// <param name="id"></param>
        /// <param name="version">API version</param>
        /// <returns>Return Employee</returns>
        [AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(EmployeeDto))]
        [ProducesResponseType(404)]
        [HttpGet("{id:int}", Name = "GetEmployeeByIdV2")]
        public async Task<ActionResult<EmployeeDto>> GetAsync(int id, ApiVersion version)
        {
            Logger.LogDebug("GetAsync");
            var employee = await _employeeRepository.GetSingleAsync(emp => emp.EmployeeId == id);
            if (employee == null)
            {
                return NotFound(new { message = "The employee was not found"});
            }
            var employeeDto = _employeeToDtoConverter.Convert(employee);
            return Ok(employeeDto);
        }

        /// <summary>
        /// Update Employee
        /// </summary>
        /// POST /api/employees/update/{employee}
        /// <param name="employee"></param>
        /// <param name="version">API version</param>
        /// <returns>Returns updated Employee</returns>
        [ProducesResponseType(201, Type = typeof(EmployeeDto))]
        [ProducesResponseType(400)]
        [HttpPost("update", Name = "UpdateEmployeeV2")]
        public async Task<IActionResult> UpdateAsync([FromBody] Employee employee, ApiVersion version)
        {
            Logger.LogDebug("UpdateAsync");
            if (employee == null)
            {
                return BadRequest(new { message = "The retrieved employee is null"});
            }
            var updatedEmployee = await _employeeRepository.UpdateAsync(employee);
            if (updatedEmployee == null)
            {
                return BadRequest(new { message = "The updated employee is null"});
            }
            return CreatedAtRoute("GetEmployeeByIdV2", new { id = employee.EmployeeId, version = version.ToString() }, employee);
        }
    }
}
