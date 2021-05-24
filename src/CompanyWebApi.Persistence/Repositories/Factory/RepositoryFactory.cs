using CompanyWebApi.Persistence.DbContexts;
using System.Threading;
using System.Threading.Tasks;

namespace CompanyWebApi.Persistence.Repositories.Factory
{
    public class RepositoryFactory : IRepositoryFactory 
    { 
        private readonly ApplicationDbContext _dbContext;
        private ICompanyRepository _companyRepository;
        private IDepartmentRepository _departmentRepository;
        private IEmployeeRepository _employeeRepository;
        private IUserRepository _userRepository;

        public RepositoryFactory(ApplicationDbContext repositoryContext,
            ICompanyRepository companyRepository,
            IDepartmentRepository departmentRepository,
            IEmployeeRepository employeeRepository,
            IUserRepository userRepository)
        {
            _dbContext = repositoryContext;
            _companyRepository = companyRepository;
            _departmentRepository = departmentRepository;
            _employeeRepository = employeeRepository;
            _userRepository = userRepository;
        }

        public ICompanyRepository CompanyRepository
        { 
            get { return _companyRepository ??= new CompanyRepository(_dbContext); } 
        }

        public IDepartmentRepository DepartmentRepository
        {
            get { return _departmentRepository ??= new DepartmentRepository(_dbContext); }
        }

        public IEmployeeRepository EmployeeRepository
        {
            get { return _employeeRepository ??= new EmployeeRepository(_dbContext); }
        }

        public IUserRepository UserRepository
        {
            get { return _userRepository ??= new UserRepository(_dbContext); }
        }

        public async Task SaveAsync(CancellationToken cancellationToken = default)
        {
            await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
