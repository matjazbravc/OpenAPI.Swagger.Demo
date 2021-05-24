using CompanyWebApi.Contracts.Entities;
using CompanyWebApi.Persistence.Repositories.Base;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System;

namespace CompanyWebApi.Persistence.Repositories
{
	public interface ICompanyRepository : IBaseRepository<Company>
    {
        Task<Company> AddCompanyAsync(Company company, bool tracking = true);

        Task<IList<Company>> GetCompaniesAsync(bool tracking = false);

        Task<Company> GetCompanyAsync(int id, bool tracking = false);

        Task<Company> GetCompanyAsync(Expression<Func<Company, bool>> predicate, bool tracking = false);
    }
}
