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
        /// <summary>
        /// Add a new employee
        /// </summary>
        /// <param name="employee">Employee model</param>
        /// <param name="tracking">Tracking changes</param>
        /// <returns></returns>
        Task<Employee> AddEmployeeAsync(Employee employee, bool tracking = true);

        /// <summary>
        /// Get employee by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tracking">Tracking changes</param>
        /// <returns></returns>
        Task<Employee> GetEmployeeAsync(int id, bool tracking = false);

        /// <summary>
        /// Get employee by predicate
        /// </summary>
        /// <param name="predicate">Where conditions</param>
        /// <param name="tracking">Tracking changes</param>
        /// <returns></returns>
        Task<Employee> GetEmployeeAsync(Expression<Func<Employee, bool>> predicate, bool tracking = false);

        /// <summary>
        /// Get all employees by predicate
        /// </summary>
        /// <param name="predicate">Where conditions</param>
        /// <param name="tracking">Tracking changes</param>
        /// <returns></returns>
        Task<IList<Employee>> GetEmployeesAsync(Expression<Func<Employee, bool>> predicate = null, bool tracking = false);

        /// <summary>
        /// Search employees
        /// </summary>
        /// <param name="searchCriteria">EmployeeSearchDto model</param>
        /// <param name="tracking">Tracking changes</param>
        /// <returns></returns>
        Task<IList<Employee>> SearchEmployeesAsync(EmployeeSearchDto searchCriteria, bool tracking = false);
    }
}
