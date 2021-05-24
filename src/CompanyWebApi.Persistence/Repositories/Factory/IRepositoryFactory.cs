using System.Threading;
using System.Threading.Tasks;

namespace CompanyWebApi.Persistence.Repositories.Factory
{
    public interface IRepositoryFactory 
    {
        ICompanyRepository CompanyRepository { get; }

        IDepartmentRepository DepartmentRepository { get; }

        IEmployeeRepository EmployeeRepository { get; }

        IUserRepository UserRepository { get; }

        /// <summary>
        /// Save method is used after all the modifications are finished on a certain entity.
        /// All changes will be applied or if something fails, all changes will be reverted.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task SaveAsync(CancellationToken cancellationToken = default);
    }
}
