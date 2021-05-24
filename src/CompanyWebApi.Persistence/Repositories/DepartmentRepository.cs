using CompanyWebApi.Contracts.Entities;
using CompanyWebApi.Persistence.DbContexts;
using CompanyWebApi.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace CompanyWebApi.Persistence.Repositories
{
    public class DepartmentRepository : BaseRepository<Department, ApplicationDbContext>, IDepartmentRepository
    {
        public DepartmentRepository(ApplicationDbContext dbContext)
            : base(dbContext)
        {
        }

        public async Task<Department> AddDepartmentAsync(Department department, bool tracking = false)
        {
            await AddAsync(department).ConfigureAwait(false);
            await SaveAsync().ConfigureAwait(false);
            return await GetDepartmentAsync(department.DepartmentId, tracking).ConfigureAwait(false);
        }

        public async Task<IList<Department>> GetDepartmentsAsync(bool tracking = false)
        {
            var result = await GetAsync<Department>(
                include: source => source
                    .Include(dep => dep.Company)
                    .Include(emp => emp.Employees).ThenInclude(emp => emp.EmployeeAddress)
                    .Include(emp => emp.Employees).ThenInclude(emp => emp.User),
                orderBy: o => o
                    .OrderBy(ob => ob.DepartmentId),
                tracking:tracking).ConfigureAwait(false);
            return result;
        }

        public async Task<Department> GetDepartmentAsync(int id, bool tracking = false)
        {
            var result = await GetSingleOrDefaultAsync<Department>(
                dpt => dpt.DepartmentId == id,
                source => source
                    .Include(dep => dep.Company)
                    .Include(emp => emp.Employees).ThenInclude(emp => emp.EmployeeAddress)
                    .Include(emp => emp.Employees).ThenInclude(emp => emp.User),
                tracking).ConfigureAwait(false);
            return result;
        }

        public async Task<Department> GetDepartmentAsync(Expression<Func<Department, bool>> predicate, bool tracking = false)
        {
            var result = await GetSingleOrDefaultAsync<Department>(predicate,
                source => source
                    .Include(dep => dep.Company)
                    .Include(emp => emp.Employees).ThenInclude(emp => emp.EmployeeAddress)
                    .Include(emp => emp.Employees).ThenInclude(emp => emp.User),
                tracking).ConfigureAwait(false);
            return result;
        }
    }
}
