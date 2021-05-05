using CompanyWebApi.Contracts.Entities;
using CompanyWebApi.Persistence.DbContexts;
using CompanyWebApi.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace CompanyWebApi.Persistence.Repositories
{
    public class CompanyRepository : BaseRepository<Company>, ICompanyRepository
    {
        public CompanyRepository(ApplicationDbContext appDbContext) : base(appDbContext)
        {
        }

        // https://www.learnentityframeworkcore.com/dbset/querying-data
        public override async Task<Company> GetSingleAsync(Expression<Func<Company, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = await DatabaseSet
                .Include(cmp => cmp.Employees).ThenInclude(emp => emp.EmployeeAddress)
                .Include(cmp => cmp.Employees).ThenInclude(emp => emp.Department)
                .Include(cmp => cmp.Employees).ThenInclude(emp => emp.User)
                .AsNoTracking()
                .SingleOrDefaultAsync(predicate, cancellationToken).ConfigureAwait(false);
            return result;
        }

        public override async Task<IList<Company>> GetAllAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            IQueryable<Company> query = DatabaseSet;
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
