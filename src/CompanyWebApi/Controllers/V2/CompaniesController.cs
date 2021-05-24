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
    public class CompaniesController : BaseController<CompaniesController>
    {
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IConverter<Company, CompanyDto> _companyToDtoConverter;
        private readonly IConverter<IList<Company>, IList<CompanyDto>> _companyToDtoListConverter;

        public CompaniesController(IRepositoryFactory repositoryFactory,
            IConverter<Company, CompanyDto> companyToDtoConverter,
            IConverter<IList<Company>, IList<CompanyDto>> companyToDtoListConverter)
        {
            _repositoryFactory = repositoryFactory;
            _companyToDtoConverter = companyToDtoConverter;
            _companyToDtoListConverter = companyToDtoListConverter;
        }

        /// <summary>
        /// Add a new company
        /// </summary>
        /// <remarks>
        /// Sample request body:
        ///
        ///     {
        ///        "name": "Test Company"
        ///     }
        /// 
        /// Sample response body:
        /// 
        ///     {
        ///         "companyId": 12,
        ///         "name": "Test Company",
        ///         "employees": []
        ///     }
        /// </remarks>
        /// <param name="company">CompanyCreateDto model</param>
        /// <param name="version">API version</param>
        [SwaggerResponse(StatusCodes.Status201Created, Type = typeof(CompanyDto), Description = "Returns a new company")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized user")]
        [HttpPost("create", Name = "CreateCompanyV2")]
        public async Task<IActionResult> CreateAsync([FromBody] CompanyCreateDto company, ApiVersion version)
        {
            Logger.LogDebug(nameof(CreateAsync));
            var newCompany = new Company
            {
                Name = company.Name
            };
            var repoCompany = await _repositoryFactory.CompanyRepository.AddCompanyAsync(newCompany).ConfigureAwait(false);
            var result = _companyToDtoConverter.Convert(repoCompany);
            return new ObjectResult(result)
            {
                StatusCode = StatusCodes.Status201Created
            };
        }

        /// <summary>
        /// Deletes a company with Id
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE /api/v2/companies/1
        ///
        /// Sample response body:
        ///     
        ///    Code 204 Success
        /// 
        /// </remarks>
        /// <param name="id" example="1">Company Id</param>
        /// <param name="version">API version</param>
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Company was successfuly deleted")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "No company was found")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized user")]
        [HttpDelete("{id:int}", Name = "DeleteCompanyByIdV2")]
        public async Task<IActionResult> DeleteAsync(int id, ApiVersion version)
        {
            Logger.LogDebug(nameof(DeleteAsync));
            var company = await _repositoryFactory.CompanyRepository.GetCompanyAsync(id).ConfigureAwait(false);
            if (company == null)
            {
                return NotFound(new { message = "The company was not found" });
            }
            _repositoryFactory.CompanyRepository.Remove(company);
            await _repositoryFactory.SaveAsync().ConfigureAwait(false);
            return Ok();
        }

        /// <summary>
        /// Gets all companies
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/v2/companies/getall
        ///
        /// Sample response body:
        /// 
        ///     [
        ///       {
        ///         "companyId": 1,
        ///         "name": "Company One",
        ///         "employees": [
        ///           "John Whyne, Address: Bangalore, India, Department: HR, Username: johnw",
        ///           "Mathias Gernold, Address: Newyork, USA, Department: Admin, Username: mathiasg",
        ///           "Julia Reynolds, Address: California, USA, Department: Development, Username: juliar"
        ///         ]
        ///       },
        ///       {
        ///         "companyId": 2,
        ///         "name": "Company Two",
        ///         "employees": [
        ///           "Alois Mock, Address: NewDelhi, India, Department: HR, Username: aloism"
        ///         ]
        ///       },
        ///       {
        ///         "companyId": 3,
        ///         "name": "Company Three",
        ///         "employees": [
        ///           "Gertraud Bochold, Address: Kentuki, USA, Department: Admin, Username: gertraudb",
        ///           "Alan Ford, Address: Milano, Italy, Department: Admin, Username: alanf"
        ///         ]
        ///       }
        ///     ]
        /// </remarks>
        /// <param name="version">API version</param>
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(IEnumerable<CompanyDto>), Description = "Return list of companies")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "The companies list is empty")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized user")]
        [HttpGet("getall", Name = "GetAllCompaniesV2")]
        public async Task<ActionResult<IEnumerable<CompanyDto>>> GetAllAsync(ApiVersion version)
        {
            Logger.LogDebug(nameof(GetAllAsync));
            var companies = await _repositoryFactory.CompanyRepository.GetCompaniesAsync().ConfigureAwait(false);
            if (!companies.Any())
            {
                return NotFound(new { message = "The companies list is empty" });
            }
            var companiesDto = _companyToDtoListConverter.Convert(companies);
            return Ok(companiesDto);
        }

        /// <summary>
        /// Get a company with Id
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/v2/companies/1
        ///
        /// Sample response body:
        /// 
        ///     {
        ///       "companyId": 1,
        ///       "name": "Company One",
        ///       "employees": [
        ///         "John Whyne, Address: Bangalore, India, Department: HR, Username: johnw",
        ///         "Mathias Gernold, Address: Newyork, USA, Department: Admin, Username: mathiasg",
        ///         "Julia Reynolds, Address: California, USA, Department: Development, Username: juliar"
        ///       ]
        ///     }
        /// </remarks>
        /// <param name="id" example="1">Company Id</param>
        /// <param name="version">API version</param>
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(CompanyDto), Description = "Return company")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "The company was not found")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized user")]
        [HttpGet("{id:int}", Name = "GetCompanyByIdV2")]
        public async Task<IActionResult> GetCompanyById(int id, ApiVersion version)
        {
            Logger.LogDebug(nameof(GetCompanyById));
            var company = await _repositoryFactory.CompanyRepository.GetCompanyAsync(id).ConfigureAwait(false);
            if (company == null)
            {
                return NotFound(new { message = "The company was not found" });
            }
            var companyDto = _companyToDtoConverter.Convert(company);
            return Ok(companyDto);
        }

        /// <summary>
        /// Updates a company
        /// </summary>
        /// <remarks>
        /// Sample request body:
        ///
        ///     {
        ///       "companyId": 1,
        ///       "name": "New Company One"
        ///     }
        /// 
        /// Sample response body:
        /// 
        ///     {
        ///       "companyId": 1,
        ///       "name": "New Company One",
        ///       "employees": [
        ///         "John Whyne, Address: Bangalore, India, Department: HR, Username: johnw",
        ///         "Mathias Gernold, Address: Newyork, USA, Department: Admin, Username: mathiasg",
        ///         "Julia Reynolds, Address: California, USA, Department: Development, Username: juliar"
        ///       ]
        ///     }
        /// </remarks>
        /// <param name="company">CompanyUpdateDto model</param>
        /// <param name="version">API version</param>
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(CompanyDto), Description = "Return updated company")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "The company was not found")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized user")]
        [HttpPut("update", Name = "UpdateCompanyV2")]
        public async Task<IActionResult> UpdateAsync([FromBody] CompanyUpdateDto company, ApiVersion version)
        {
            Logger.LogDebug(nameof(UpdateAsync));
            var repoCompany = await _repositoryFactory.CompanyRepository.GetCompanyAsync(company.CompanyId).ConfigureAwait(false);
            if (repoCompany == null)
            {
                return NotFound(new { message = "The company was not found" });
            }
            repoCompany.Name = company.Name;
            await _repositoryFactory.CompanyRepository.UpdateAsync(repoCompany).ConfigureAwait(false);
            await _repositoryFactory.SaveAsync().ConfigureAwait(false);
            var result = _companyToDtoConverter.Convert(repoCompany);
            return Ok(result);
        }
    }
}