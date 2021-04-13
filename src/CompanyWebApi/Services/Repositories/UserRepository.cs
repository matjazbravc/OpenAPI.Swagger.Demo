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
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext appDbContext) : base(appDbContext)
        {
        }

        public override async Task<IList<User>> GetAllAsync(bool disableTracking = true, CancellationToken cancellationToken = default(CancellationToken))
        {
            IQueryable<User> query = _databaseSet;
            if (disableTracking)
            {
                query = query.AsNoTracking();
            }
            return await query
                .Include(usr => usr.Employee)
                .ThenInclude(emp => emp.Company)
                .ToListAsync(cancellationToken).ConfigureAwait(false);
        }

        // https://www.learnentityframeworkcore.com/dbset/querying-data
        public override async Task<User> GetSingleAsync(Expression<Func<User, bool>> predicate, bool disableTracking = true, CancellationToken cancellationToken = default(CancellationToken))
        {
            var user = await _databaseSet
                .Include(cmp => cmp.Employee).ThenInclude(emp => emp.EmployeeAddress)
                .Include(cmp => cmp.Employee).ThenInclude(emp => emp.Department)
                .AsNoTracking()
                .SingleOrDefaultAsync(predicate, cancellationToken).ConfigureAwait(false);
            return user;
        }
    }
}
