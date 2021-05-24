using CompanyWebApi.Contracts.Dto;
using CompanyWebApi.Contracts.Entities;
using CompanyWebApi.Persistence.Repositories.Base;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System;

namespace CompanyWebApi.Persistence.Repositories
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<User> AddUserAsync(User user, bool tracking = false);

        Task<IList<UserDto>> GetUsersAsync(bool tracking = false);

        Task<User> GetUserAsync(int id, bool tracking = false);

        Task<User> GetUserAsync(Expression<Func<User, bool>> predicate, bool tracking = false);

        Task<IList<User>> GetUsersByUsernameAsync(string username, bool tracking = false);
    }
}
