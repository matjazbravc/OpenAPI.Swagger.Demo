using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace CompanyWebApi.Persistence.Repositories.Base
{
    public interface IBaseRepository<TEntity> where TEntity : class
    {
		Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);
		
	    Task<int> AddAsync(IList<TEntity> entities, CancellationToken cancellationToken = default);

		Task<int> CountAsync(CancellationToken cancellationToken = default);

		Task<int> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);

	    Task<IList<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

        Task<IList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<TEntity> GetSingleAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

        Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    }
}
