using CompanyWebApi.Contracts.Entities;
using CompanyWebApi.Persistence.Repositories.Base;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System;

namespace CompanyWebApi.Persistence.Repositories
{
	public interface IDepartmentRepository : IBaseRepository<Department>
    {
        Task<Department> AddDepartmentAsync(Department department, bool tracking = true);

        Task<IList<Department>> GetDepartmentsAsync(bool tracking = false);

        Task<Department> GetDepartmentAsync(int id, bool tracking = false);

        Task<Department> GetDepartmentAsync(Expression<Func<Department, bool>> predicate, bool tracking = false);
    }
}
