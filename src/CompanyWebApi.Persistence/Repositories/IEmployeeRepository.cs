using CompanyWebApi.Contracts.Dto;
using CompanyWebApi.Contracts.Entities;
using CompanyWebApi.Persistence.Repositories.Base;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System;

namespace CompanyWebApi.Persistence.Repositories
{
    public interface IEmployeeRepository : IBaseRepository<Employee>
    {
        Task<Employee> AddEmployeeAsync(Employee employee, bool tracking = false);

        Task<Employee> GetEmployeeAsync(int id, bool tracking = false);

        Task<Employee> GetEmployeeAsync(Expression<Func<Employee, bool>> predicate, bool tracking = false);

        Task<IList<Employee>> GetEmployeesAsync(bool tracking = false);

        Task<IList<Employee>> GetEmployeesAsync(Expression<Func<Employee, bool>> predicate, bool tracking = false);

        Task<IList<Employee>> SearchEmployeesAsync(EmployeeSearchDto searchCriteria, bool tracking = false);
    }
}
