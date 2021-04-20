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
    [Produces("application/json")]
    [EnableCors("EnableCORS")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class CompaniesController : BaseController<CompaniesController>
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IConverter<Company, CompanyDto> _companyToDtoConverter;
        private readonly IConverter<IList<Company>, IList<CompanyDto>> _companyToDtoListConverter;

        public CompaniesController(ICompanyRepository companyRepository,
            IConverter<Company, CompanyDto> companyToDtoConverter,
            IConverter<IList<Company>, IList<CompanyDto>> companyToDtoListConverter)
        {
            _companyRepository = companyRepository;
            _companyToDtoConverter = companyToDtoConverter;
            _companyToDtoListConverter = companyToDtoListConverter;
        }

        /// <summary>
        /// Create Company
        /// </summary>
        /// <remarks>This API will create new Company</remarks>
        /// POST /api/companies/create/{company}
        /// <param name="company">Company model</param>
        /// <param name="apiVersion">API version</param>
        [MapToApiVersion("1.1")]
        [HttpPost("create")]
        [ProducesResponseType(201, Type = typeof(Company))]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateAsync([FromBody]Company company, ApiVersion apiVersion)
        {
            Logger.LogDebug(nameof(CreateAsync));
            if (company == null)
            {
                return BadRequest(new BadRequestError("The company is null"));
            }
            await _companyRepository.AddAsync(company).ConfigureAwait(false);
            return CreatedAtRoute("GetCompanyById", new { id = company.CompanyId, version = apiVersion.ToString() }, company);
        }

        /// <summary>
        /// Delete Company
        /// </summary>
        /// <remarks>This API will delete Company with Id</remarks>
        /// DELETE /api/companies/{id}
        /// <param name="id"></param>
        /// <returns></returns>
        [MapToApiVersion("1.1")]
        [HttpDelete("{id}", Name = "DeleteCompanyById")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            Logger.LogDebug(nameof(DeleteAsync));
            var company = await _companyRepository.GetSingleAsync(company => company.CompanyId == id).ConfigureAwait(false);
            if (company == null)
            {
                return NotFound(new NotFoundError("The company was not found"));
            }
            await _companyRepository.DeleteAsync(company).ConfigureAwait(false);
            return NoContent();
        }

        /// <summary>
        /// Get all Companies
        /// </summary>
        /// <remarks>This API return list of all Companies</remarks>
        /// GET api/companies/getall
        /// <returns>List of Companies</returns>
        [MapToApiVersion("1.1")]
        [HttpGet("getAll")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<CompanyDto>))]
        [ProducesResponseType(404)]
        public async Task<ActionResult<IEnumerable<CompanyDto>>> GetAllAsync()
        {
            Logger.LogDebug(nameof(GetAllAsync));
            var companies = await _companyRepository.GetAllAsync().ConfigureAwait(false);
            if (!companies.Any())
            {
                return NotFound(new NotFoundError("The companies list is empty"));
            }
            var companiesDto = _companyToDtoListConverter.Convert(companies);
            return Ok(companiesDto);
        }
        /// <summary>
        /// Get Company
        /// </summary>
        /// <remarks>This API return Company with Id</remarks>
        /// GET /api/companies/{id}
        /// <param name="id">Company Id</param>
        /// <returns>Return Company</returns>
        [MapToApiVersion("1.1")]
        [HttpGet("{id}", Name = "GetCompanyById")]
        [ProducesResponseType(200, Type = typeof(CompanyDto))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            Logger.LogDebug(nameof(GetByIdAsync));
            var company = await _companyRepository.GetSingleAsync(comp => comp.CompanyId == id).ConfigureAwait(false);
            if (company == null)
            {
                return NotFound(new NotFoundError("The company was not found"));
            }
            var companyDto = _companyToDtoConverter.Convert(company);
            return Ok(companyDto);
        }

        /// <summary>
        /// Update Company
        /// </summary>
        /// <remarks>This API updates a company</remarks>
        /// POST /api/companies/update/{company}
        /// <param name="company">Company model</param>
        /// <param name="apiVersion">API version</param>
        /// <returns>Returns updated Company</returns>
        [MapToApiVersion("1.1")]
        [HttpPost("update")]
        [ProducesResponseType(201, Type = typeof(CompanyDto))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> UpdateAsync([FromBody]Company company, ApiVersion apiVersion)
        {
            Logger.LogDebug(nameof(UpdateAsync));
            if (company == null)
            {
                return BadRequest(new BadRequestError("The retrieved company is null"));
            }
            var updatedCompany = await _companyRepository.UpdateAsync(company).ConfigureAwait(false);
            if (updatedCompany == null)
            {
                return BadRequest(new BadRequestError("The updated company is null"));
            }
            return CreatedAtRoute("GetCompanyById", new { id = company.CompanyId, version = apiVersion.ToString() }, company);
        }
    }
}
