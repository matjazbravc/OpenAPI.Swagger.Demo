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

namespace CompanyWebApi.Controllers.V1
{
    [ApiAuthorization]
    [ApiController]
    [ApiVersion("1.0", Deprecated = true)]
    [Produces("application/json")]
    [EnableCors("EnableCORS")]
    [ServiceFilter(typeof(ValidModelStateAsyncActionFilter))]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class DepartmentsController : BaseController<DepartmentsController>
    {
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IConverter<Department, DepartmentDto> _departmentToDtoConverter;
        private readonly IConverter<IList<Department>, IList<DepartmentDto>> _departmentToDtoListConverter;

        public DepartmentsController(IRepositoryFactory repositoryFactory,
            IConverter<Department, DepartmentDto> departmentToDtoConverter,
            IConverter<IList<Department>, IList<DepartmentDto>> departmentToDtoListConverter)
        {
            _repositoryFactory = repositoryFactory;
            _departmentToDtoConverter = departmentToDtoConverter;
            _departmentToDtoListConverter = departmentToDtoListConverter;
        }

        /// <summary>
        /// Add a new department
        /// </summary>
        /// <remarks>
        /// Sample request body:
        ///
        ///     {
        ///        "companyId" : 1, 
        ///        "name": "Test Department"
        ///     }
        /// 
        /// Sample response body:
        /// 
        ///     {
        ///         "departmentId": 10,
        ///         "name": "Test Department",
        ///         "employees": []
        ///     }
        /// 
        /// </remarks>
        /// <param name="department">DepartmentDto model</param>
        /// <param name="version">API version</param>
        [SwaggerResponse(StatusCodes.Status201Created, Type = typeof(DepartmentDto), Description = "Returns a new department")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Company was not found")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized user")]
        [HttpPost("create", Name = "CreateDepartmentV1")]
        public async Task<IActionResult> CreateAsync([FromBody] DepartmentCreateDto department, ApiVersion version)
        {
            Logger.LogDebug("CreateAsync");
            if (!await _repositoryFactory.CompanyRepository.ExistsAsync(c => c.CompanyId == department.CompanyId).ConfigureAwait(false))
            {
                return NotFound(new { message = $"The Company with id {department.CompanyId} was not found" });
            }
            
            var newDepartment = new Department()
            {
                CompanyId = department.CompanyId,
                Name = department.Name
            };

            var repoDepartment = await _repositoryFactory.DepartmentRepository.AddDepartmentAsync(newDepartment).ConfigureAwait(false);
            var result = _departmentToDtoConverter.Convert(repoDepartment);
            return new ObjectResult(result)
            {
                StatusCode = StatusCodes.Status201Created
            };
        }

        /// <summary>
        /// Deletes a department with id
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE /api/v2/departments/1
        ///
        /// Sample response body:
        ///     
        ///    Code 200 Success
        /// 
        /// </remarks>
        /// <param name="id" example="1">Department Id</param>
        /// <param name="version">API version</param>
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Department was successfuly deleted")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "No department was found")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized user")]
        [HttpDelete("{id:int}", Name = "DeleteDepartmentByIdV1")]
        public async Task<IActionResult> DeleteAsync(int id, ApiVersion version)
        {
            Logger.LogDebug("RemoveAsync");
            var department = await _repositoryFactory.DepartmentRepository.GetDepartmentAsync(id).ConfigureAwait(false);
            if (department == null)
            {
                return NotFound(new { message = "The department was not found" });
            }
            _repositoryFactory.DepartmentRepository.Remove(department);
            await _repositoryFactory.SaveAsync().ConfigureAwait(false);
            return Ok();
        }

        /// <summary>
        /// Gets all departments
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/v2/departments/getall
        ///
        /// Sample response body:
        ///
        ///     [
        ///       {
        ///         "departmentId": 1,
        ///         "name": "Logistics",
        ///         "companyId": 1,
        ///         "companyName": "Company One",
        ///         "employees": [
        ///           "John Whyne, Address: Kentucky, USA, Department: Logistics, Username: johnw"
        ///         ]
        ///       },
        ///       {
        ///         "departmentId": 2,
        ///         "name": "Administration",
        ///         "companyId": 1,
        ///         "companyName": "Company One",
        ///         "employees": [
        ///           "Alois Mock, Address: Vienna, Austria, Department: Administration, Username: aloism"
        ///         ]
        ///       },
        ///       {
        ///         "departmentId": 3,
        ///         "name": "Development",
        ///         "companyId": 1,
        ///         "companyName": "Company One",
        ///         "employees": []
        ///       }
        ///     ]
        /// </remarks>
        /// <param name="version">API version</param>
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(IEnumerable<DepartmentDto>), Description = "Return list of departments")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "The departments list is empty")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized user")]
        [HttpGet("getAll", Name = "GetAllDepartmentsV1")]
        public async Task<ActionResult<IEnumerable<DepartmentDto>>> GetAllAsync(ApiVersion version)
        {
            Logger.LogDebug("GetAsync");
            var departments = await _repositoryFactory.DepartmentRepository.GetDepartmentsAsync().ConfigureAwait(false);
            if (!departments.Any())
            {
                return NotFound(new { message = "The departments list is empty" });
            }
            var departmentsDto = _departmentToDtoListConverter.Convert(departments);
            return Ok(departmentsDto);
        }

        /// <summary>
        /// Get a department with id
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/v2/departments/1
        ///
        /// Sample response body:
        /// 
        ///     {
        ///       "departmentId": 1,
        ///       "name": "Logistics",
        ///       "companyId": 1,
        ///       "companyName": "Company One",
        ///       "employees": [
        ///         "John Whyne, Address: Kentucky, USA, Department: Logistics, Username: johnw"
        ///       ]
        ///     }
        /// 
        /// </remarks>
        /// <param name="id" example="1">Department Id</param>
        /// <param name="version">API version</param>
        [HttpGet("{id:int}", Name = "GetDepartmentByIdV1")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(DepartmentDto), Description = "Return department")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "The department was not found")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized user")]
        public async Task<ActionResult<DepartmentDto>> GetAsync(int id, ApiVersion version)
        {
            Logger.LogDebug("GetAsync");
            var department = await _repositoryFactory.DepartmentRepository.GetDepartmentAsync(id).ConfigureAwait(false);
            if (department == null)
            {
                return NotFound(new { message = "The department was not found" });
            }
            var departmentDto = _departmentToDtoConverter.Convert(department);
            return Ok(departmentDto);
        }

        /// <summary>
        /// Updates a department
        /// </summary>
        /// <remarks>
        /// Sample request body:
        ///
        ///     {
        ///       "departmentId": 1,
        ///       "name": "NEW DEPARTMENT"
        ///     }
        /// 
        /// Sample response body:
        /// 
        ///     {
        ///       "departmentId": 1,
        ///       "name": "NEW DEPARTMENT",
        ///       "employees": [
        ///         "John Whyne, Address: Bangalore, India, Department: NEW DEPARTMENT, Username: johnw",
        ///         "Alois Mock, Address: NewDelhi, India, Department: NEW DEPARTMENT, Username: aloism"
        ///       ]
        ///     }
        /// 
        /// </remarks>
        /// <param name="department">DepartmentUpdateDto model</param>
        /// <param name="version">API version</param>
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(DepartmentDto), Description = "Return updated department")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "The department was not found")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized user")]
        [HttpPut("update", Name = "UpdateDepartmentV1")]
        public async Task<IActionResult> UpdateAsync([FromBody] DepartmentUpdateDto department, ApiVersion version)
        {
            Logger.LogDebug("UpdateAsync");
            var repoDepartment = await _repositoryFactory.DepartmentRepository.GetDepartmentAsync(department.DepartmentId).ConfigureAwait(false);
            if (repoDepartment == null)
            {
                return NotFound(new { message = "The department was not found" });
            }

            // Update Department's name
            repoDepartment.Name = department.Name;

            await _repositoryFactory.DepartmentRepository.UpdateAsync(repoDepartment).ConfigureAwait(false);
            await _repositoryFactory.SaveAsync().ConfigureAwait(false);

            var result = _departmentToDtoConverter.Convert(repoDepartment);
            return Ok(result);
        }
    }
}
