using CompanyWebApi.Contracts.Dto;
using CompanyWebApi.Contracts.Entities;
using CompanyWebApi.Persistence.DbContexts;
using CompanyWebApi.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace CompanyWebApi.Persistence.Repositories
{
    public class UserRepository : BaseRepository<User, ApplicationDbContext>, IUserRepository
    {
        public UserRepository(ApplicationDbContext dbContext)
            : base(dbContext)
        {
        }

        public async Task<User> AddUserAsync(User user, bool tracking = false)
        {
            await AddAsync(user).ConfigureAwait(false);
            await SaveAsync().ConfigureAwait(false);
            return await GetUserAsync(user.EmployeeId, tracking).ConfigureAwait(false);
        }

        public async Task<IList<UserDto>> GetUsersAsync(bool tracking = false)
        {
            var result = await GetAsync(
                include: source => source
                    .Include(user => user.Employee),
                selector: user => new UserDto
                {
                    EmployeeId = user.EmployeeId,
                    FirstName = user.Employee.FirstName,
                    LastName = user.Employee.LastName,
                    Username = user.Username,
                    Password = user.Password
                },
                orderBy: o => o
                    .OrderBy(ob => ob.EmployeeId),
                tracking: tracking).ConfigureAwait(false);
            return result;
        }

        public async Task<User> GetUserAsync(int id, bool tracking = false)
        {
            var result = await GetSingleOrDefaultAsync<User>(
                user => user.EmployeeId == id,
                source => source
                    .Include(user => user.Employee).ThenInclude(user => user.EmployeeAddress)
                    .Include(user => user.Employee).ThenInclude(user => user.Department),
                tracking).ConfigureAwait(false);
            return result;
        }

        public async Task<User> GetUserAsync(Expression<Func<User, bool>> predicate, bool tracking = false)
        {
            var result = await GetSingleOrDefaultAsync<User>(
                predicate,
                source => source
                    .Include(user => user.Employee).ThenInclude(user => user.EmployeeAddress)
                    .Include(user => user.Employee).ThenInclude(user => user.Department),
                tracking).ConfigureAwait(false);
            return result;
        }

        public async Task<IList<User>> GetUsersByUsernameAsync(string username, bool tracking = false)
        {
            var result = await GetAsync<User>(
                usr => usr.Username.Equals(username),
                include: source => source
                    .Include(user => user.Employee),
                orderBy: o => o
                    .OrderBy(ob => ob.Username),
                tracking: tracking).ConfigureAwait(false);
            return result;
        }
    }
}
