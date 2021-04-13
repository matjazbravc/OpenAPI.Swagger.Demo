using CompanyWebApi.Contracts.Converters;
using CompanyWebApi.Contracts.Dto;
using CompanyWebApi.Contracts.Entities;
using CompanyWebApi.Controllers.Base;
using CompanyWebApi.Core.Errors;
using CompanyWebApi.Services.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CompanyWebApi.Controllers
{
	[Authorize]
	[ApiController]
	[ApiVersion("1.0", Deprecated = true)]
	[ApiVersion("1.1")]
	[EnableCors("EnableCORS")]
	[Route("api/v{version:apiVersion}/[controller]")]
	public class EmployeesController : BaseController<EmployeesController>
	{
		public IEmployeeRepository EmployeeRepository;
        private readonly IConverter<Employee, EmployeeDto> _employeeToDtoConverter;
        private readonly IConverter<IList<Employee>, IList<EmployeeDto>> _employeeToDtoListConverter;

        public EmployeesController(IEmployeeRepository employeeRepository,
            IConverter<Employee,EmployeeDto> employeeToDtoConverter,
            IConverter<IList<Employee>, IList<EmployeeDto>> employeeToDtoListConverter)
        {
            EmployeeRepository = employeeRepository;
            _employeeToDtoConverter = employeeToDtoConverter;
            _employeeToDtoListConverter = employeeToDtoListConverter;
        }

		/// <summary>
		/// Create Employee
		/// </summary>
		/// <remarks>This API will create new Employee</remarks>
		/// POST /api/employees/create/{employee}
		/// <param name="employee">Employee model</param>
		/// <param name="apiVersion">API version</param>
		[MapToApiVersion("1.1")]
		[HttpPost("create", Name = "CreateEmployee")]
		[ProducesResponseType(201, Type = typeof(Employee))]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		public async Task<IActionResult> CreateAsync([FromBody] Employee employee, ApiVersion apiVersion)
		{
			Logger.LogDebug("CreateAsync");
			if (employee == null)
			{
				return BadRequest(new BadRequestError("The employee is null"));
			}
			await EmployeeRepository.AddAsync(employee);
			return CreatedAtRoute("GetEmployeeById", new { id = employee.EmployeeId, version = apiVersion.ToString() }, employee);
		}

		/// <summary>
		/// Delete Employee
		/// </summary>
		/// <remarks>This API will delete Employee with Id</remarks>
		/// DELETE /api/employees/{id}
		/// <param name="id"></param>
		/// <returns></returns>
		[MapToApiVersion("1.1")]
		[HttpDelete("{id}", Name = "DeleteEmployeeById")]
		[ProducesResponseType(204)]
		[ProducesResponseType(404)]
		public async Task<IActionResult> DeleteAsync(int id)
		{
			Logger.LogDebug("DeleteAsync");
			var employee = await EmployeeRepository.GetSingleAsync(emp => emp.EmployeeId == id);
			if (employee == null)
			{
				return NotFound(new NotFoundError("The employee was not found"));
			}
			await EmployeeRepository.DeleteAsync(employee);
			return NoContent();
		}

		/// <summary>
		/// Get all Employees
		/// </summary>
		/// <remarks>This API return list of all Employees</remarks>
		/// GET api/employees/getall
		/// <returns>List of Employees</returns>
		[MapToApiVersion("1.1")]
		[HttpGet("getall", Name = "GetAllEmployees")]
		[ProducesResponseType(200, Type = typeof(IEnumerable<EmployeeDto>))]
		[ProducesResponseType(404)]
		public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetAllAsync()
		{
			Logger.LogDebug("GetAllAsync");
			var employees = await EmployeeRepository.GetAllAsync().ConfigureAwait(false);
			if (!employees.Any())
			{
				return NotFound(new NotFoundError("The employees list is empty"));
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
		/// <returns>Return Employee</returns>
		[AllowAnonymous]
		[MapToApiVersion("1.1")]
		[HttpGet("{id}", Name = "GetEmployeeById")]
		[ProducesResponseType(200, Type = typeof(EmployeeDto))]
		[ProducesResponseType(404)]
		public async Task<ActionResult<EmployeeDto>> GetAsync(int id)
		{
			Logger.LogDebug("GetAsync");
			var employee = await EmployeeRepository.GetSingleAsync(emp => emp.EmployeeId == id);
			if (employee == null)
			{
				return NotFound(new NotFoundError("The employee was not found"));
			}
            var employeeDto = _employeeToDtoConverter.Convert(employee);
            return Ok(employeeDto);
		}

		/// <summary>
		/// Update Employee
		/// </summary>
		/// POST /api/employees/update/{employee}
		/// <param name="employee"></param>
		/// <param name="apiVersion">API version</param>
		/// <returns>Returns updated Employee</returns>
		[MapToApiVersion("1.1")]
		[HttpPost("update", Name = "UpdateEmployee")]
		[ProducesResponseType(201, Type = typeof(EmployeeDto))]
		[ProducesResponseType(400)]
		public async Task<IActionResult> UpdateAsync([FromBody] Employee employee, ApiVersion apiVersion)
		{
			Logger.LogDebug("UpdateAsync");
			if (employee == null)
			{
				return BadRequest(new BadRequestError("The retrieved employee is null"));
			}
			var updatedEmployee = await EmployeeRepository.UpdateAsync(employee);
			if (updatedEmployee == null)
			{
				return BadRequest(new BadRequestError("The updated employee is null"));
			}
			return CreatedAtRoute("GetEmployeeById", new { id = employee.EmployeeId, version = apiVersion.ToString() }, employee);
		}
	}
}
