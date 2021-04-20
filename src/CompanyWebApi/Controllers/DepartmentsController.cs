﻿using CompanyWebApi.Contracts.Converters;
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
    [Produces("application/json")]
	[EnableCors("EnableCORS")]
	[Route("api/v{version:apiVersion}/[controller]")]
	public class DepartmentsController : BaseController<DepartmentsController>
	{
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IConverter<Department, DepartmentDto> _departmentToDtoConverter;
        private readonly IConverter<IList<Department>, IList<DepartmentDto>> _departmentToDtoListConverter;

        public DepartmentsController(IDepartmentRepository departmentRepository,
            IConverter<Department, DepartmentDto> departmentToDtoConverter,
            IConverter<IList<Department>, IList<DepartmentDto>> departmentToDtoListConverter)
        {
            _departmentRepository = departmentRepository;
            _departmentToDtoConverter = departmentToDtoConverter;
            _departmentToDtoListConverter = departmentToDtoListConverter;
        }

		/// <summary>
		/// Create Department
		/// </summary>
		/// <remarks>This API will create new Department</remarks>
		/// POST /api/departments/create/{department}
		/// <param name="department">Department model</param>
		/// <param name="apiVersion">API version</param>
		[MapToApiVersion("1.1")]
		[HttpPost("create", Name = "CreateDepartment")]
		[ProducesResponseType(201, Type = typeof(Department))]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		public async Task<IActionResult> CreateAsync([FromBody] Department department, ApiVersion apiVersion)
		{
			Logger.LogDebug("CreateAsync");
			if (department == null)
			{
				return BadRequest(new BadRequestError("The department is null"));
			}
			await _departmentRepository.AddAsync(department).ConfigureAwait(false);
			return CreatedAtRoute("GetDepartmentById", new { id = department.DepartmentId, version = apiVersion.ToString() }, department);
		}

		/// <summary>
		/// Delete Department
		/// </summary>
		/// <remarks>This API will delete Department with Id</remarks>
		/// DELETE /api/departments/{id}
		/// <param name="id"></param>
		/// <returns></returns>
		[MapToApiVersion("1.1")]
		[HttpDelete("{id}", Name = "DeleteDepartmentById")]
		[ProducesResponseType(204)]
		[ProducesResponseType(404)]
		public async Task<IActionResult> DeleteAsync(int id)
		{
			Logger.LogDebug("DeleteAsync");
			var department = await _departmentRepository.GetSingleAsync(cmp => cmp.DepartmentId == id).ConfigureAwait(false);
			if (department == null)
			{
				return NotFound(new NotFoundError("The department was not found"));
			}
			await _departmentRepository.DeleteAsync(department).ConfigureAwait(false);
			return NoContent();
		}

		/// <summary>
		/// Get all Departments
		/// </summary>
		/// <remarks>This API return list of all Departments</remarks>
		/// GET api/departments/getAll
		/// <returns>List of Departments</returns>
		[MapToApiVersion("1.1")]
		[HttpGet("getAll", Name = "GetAllDepartments")]
		[ProducesResponseType(200, Type = typeof(IEnumerable<DepartmentDto>))]
		[ProducesResponseType(404)]
        public async Task<ActionResult<IEnumerable<DepartmentDto>>> GetAllAsync()
        {
	        Logger.LogDebug("GetAllAsync");
			var departments = await _departmentRepository.GetAllAsync().ConfigureAwait(false);
	        if (!departments.Any())
	        {
		        return NotFound(new NotFoundError("The departments list is empty"));
	        }
            var departmentsDto = _departmentToDtoListConverter.Convert(departments);
            return Ok(departmentsDto);
        }

		/// <summary>
		/// Get Department
		/// </summary>
		/// <remarks>This API return Department with Id</remarks>
		/// GET /api/departments/{id}
		/// <param name="id"></param>
		/// <returns>Return Department</returns>
		[AllowAnonymous]
		[MapToApiVersion("1.1")]
		[HttpGet("{id}", Name = "GetDepartmentById")]
		[ProducesResponseType(200, Type = typeof(DepartmentDto))]
		[ProducesResponseType(404)]
		public async Task<ActionResult<DepartmentDto>> GetAsync(int id)
		{
			Logger.LogDebug("GetAsync");
			var department = await _departmentRepository.GetSingleAsync(cmp => cmp.DepartmentId == id).ConfigureAwait(false);
			if (department == null)
			{
				return NotFound(new NotFoundError("The department was not found"));
			}
            var departmentDto = _departmentToDtoConverter.Convert(department);
			return Ok(departmentDto);
		}

		/// <summary>
		/// Update Department
		/// </summary>
		/// POST /api/departments/update/{department}
		/// <param name="department"></param>
		/// <param name="apiVersion">API version</param>
		/// <returns>Returns updated Department</returns>
		[MapToApiVersion("1.1")]
		[HttpPost("update", Name = "UpdateDepartment")]
		[ProducesResponseType(201, Type = typeof(DepartmentDto))]
		[ProducesResponseType(400)]
		public async Task<IActionResult> UpdateAsync([FromBody] Department department, ApiVersion apiVersion)
		{
			Logger.LogDebug("UpdateAsync");
			if (department == null)
			{
				return BadRequest(new BadRequestError("The retrieved department is null"));
			}
			var updatedDepartment = await _departmentRepository.UpdateAsync(department).ConfigureAwait(false);
			if (updatedDepartment == null)
			{
				return BadRequest(new BadRequestError("The updated department is null"));
			}
			return CreatedAtRoute("GetDepartmentById", new { id = department.DepartmentId, version = apiVersion.ToString() }, department);
		}
	}
}
