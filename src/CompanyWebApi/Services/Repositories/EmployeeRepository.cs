using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using CompanyWebApi.Contracts.Entities;
using CompanyWebApi.Persistence.DbContexts;
using CompanyWebApi.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CompanyWebApi.Services.Repositories
{
	public class EmployeeRepository : BaseRepository<Employee>, IEmployeeRepository
	{
		public EmployeeRepository(ApplicationDbContext appDbContext) : base(appDbContext)
		{
		}

		public override async Task<Employee> GetSingleAsync(Expression<Func<Employee, bool>> predicate, bool disableTracking = true, CancellationToken cancellationToken = default(CancellationToken))
		{
			Employee employee = null;
			try
			{
				// https://www.learnentityframeworkcore.com/dbset/querying-data
				employee = await _databaseSet
                    .Include(emp => emp.Company)
                    .Include(emp => emp.Department)
                    .Include(emp => emp.EmployeeAddress)
                    .Include(emp => emp.User)
					.AsNoTracking()
					.SingleOrDefaultAsync(predicate, cancellationToken).ConfigureAwait(false);
				return employee;
			}
			catch (Exception)
			{
				return employee;
			}
		}

		public override async Task<IList<Employee>> GetAllAsync(bool disableTracking = true, CancellationToken cancellationToken = default(CancellationToken))
		{
			IQueryable<Employee> query = _databaseSet;
			if (disableTracking)
			{
				query = query.AsNoTracking();
			}
			var result = await query
                .Include(emp => emp.Company)
                .Include(emp => emp.Department)
				.Include(emp => emp.EmployeeAddress)
                .Include(emp => emp.User)
				.AsNoTracking()
				.ToListAsync(cancellationToken).ConfigureAwait(false);
			return result;
		}
	}
}
