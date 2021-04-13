using CompanyWebApi.Contracts.Entities;
using CompanyWebApi.Persistence.DbContexts;
using CompanyWebApi.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace CompanyWebApi.Services.Repositories
{
    public class CompanyRepository : BaseRepository<Company>, ICompanyRepository
    {
        public CompanyRepository(ApplicationDbContext appDbContext) : base(appDbContext)
        {
        }

        // https://www.learnentityframeworkcore.com/dbset/querying-data
        public override async Task<Company> GetSingleAsync(Expression<Func<Company, bool>> predicate, bool disableTracking = true, CancellationToken cancellationToken = default(CancellationToken))
        {
            Company company = null;
            try
            {
                company = await _databaseSet
                    .Include(cmp => cmp.Employees).ThenInclude(emp => emp.EmployeeAddress)
                    .Include(cmp => cmp.Employees).ThenInclude(emp => emp.Department)
                    .Include(cmp => cmp.Employees).ThenInclude(emp => emp.User)
                    .AsNoTracking()
                    .SingleOrDefaultAsync(predicate, cancellationToken).ConfigureAwait(false);
                return company;
            }
            catch (Exception)
            {
                return company;
            }
        }

        public override async Task<IList<Company>> GetAllAsync(bool disableTracking = true, CancellationToken cancellationToken = default(CancellationToken))
        {
            IQueryable<Company> query = _databaseSet;
            if (disableTracking)
            {
                query = query.AsNoTracking();
            }
            var result = await query
                .Include(cmp => cmp.Employees).ThenInclude(emp => emp.EmployeeAddress)
                .Include(cmp => cmp.Employees).ThenInclude(emp => emp.Department)
                .Include(cmp => cmp.Employees).ThenInclude(emp => emp.User)
                .AsNoTracking()
                .ToListAsync(cancellationToken).ConfigureAwait(false);
            return result;
        }
    }
}
