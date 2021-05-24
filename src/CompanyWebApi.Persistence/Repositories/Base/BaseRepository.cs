using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace CompanyWebApi.Persistence.Repositories.Base
{
    /// <summary>
    /// Generic asynchronous entity repository
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TDbContext">DbContext</typeparam>
    public class BaseRepository<TEntity, TDbContext> : IBaseRepository<TEntity>
        where TEntity : class
        where TDbContext : DbContext
    {
		protected readonly TDbContext DatabaseContext;
		protected readonly DbSet<TEntity> DatabaseSet;

		protected BaseRepository(TDbContext dbContext)
		{
            DatabaseContext = dbContext ?? throw new ArgumentException("DbContext is null", nameof(dbContext));
			DatabaseSet = DatabaseContext.Set<TEntity>();
		}

        public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
		{
			await DatabaseSet.AddAsync(entity, cancellationToken).ConfigureAwait(false);
		}

        public async Task AddAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
		{
			await DatabaseSet.AddRangeAsync(entities, cancellationToken).ConfigureAwait(false);
		}

        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate = null, bool tracking = false, CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> query = DatabaseSet;
            if (!tracking)
            {
                query = query.AsNoTracking();
            }
            if (predicate == null)
            {
                return await query.CountAsync(cancellationToken).ConfigureAwait(false);
            }
            return await query.CountAsync(predicate, cancellationToken).ConfigureAwait(false);
        }

        public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, bool tracking = false, CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> query = DatabaseSet;
            if (!tracking)
            {
                query = query.AsNoTracking();
            }
            return await query.AnyAsync(predicate, cancellationToken).ConfigureAwait(false);
        }

        public async Task<IList<TResult>> GetAsync<TResult>(Expression<Func<TEntity, bool>> predicate = null,
            Expression<Func<TEntity, TResult>> selector = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            bool tracking = false,
            CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> query = DatabaseSet;
            if (!tracking)
            {
                query = query.AsNoTracking();
            }
            if (include != null)
            {
                query = include(query);
            }
            if (predicate != null)
            {
                query = query.Where(predicate);
            }
            if (orderBy != null)
            {
                query = orderBy(query);
            }
            if (selector != null)
            {
                return await query.Select(selector).ToListAsync(cancellationToken).ConfigureAwait(false);
            }
            return (IList<TResult>)await query.ToListAsync(cancellationToken);
        }

        public async Task<TEntity> GetSingleOrDefaultAsync<TResult>(Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            bool tracking = false,
            CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> query = DatabaseSet;
            if (!tracking)
            {
                query = query.AsNoTracking();
            }
            if (include != null)
            {
                query = include(query);
            }
            if (predicate != null)
            {
                return await query.SingleOrDefaultAsync(predicate, cancellationToken).ConfigureAwait(false);
            }
            return await query.SingleOrDefaultAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task SaveAsync(CancellationToken cancellationToken = default)
        {
            await DatabaseContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        public void Remove(TEntity entity, bool tracking = true)
        {
            if (!tracking)
            {
                DatabaseContext.Entry(entity).State = EntityState.Deleted;
            }
            else
            {
                DatabaseSet.Remove(entity);
            }
        }

        public void Remove(IEnumerable<TEntity> entities, bool tracking = true)
        {
            if (!tracking)
            {
                foreach (var entity in entities)
                {
                    DatabaseContext.Entry(entity).State = EntityState.Deleted;
                }
            }
            else
            {
                DatabaseSet.RemoveRange(entities);
            }
        }

        public async Task UpdateAsync(TEntity entity)
		{
			var properties = entity.GetType().GetProperties();
            // Try to find a [Key] attribute
            var keyProperty = (from property in properties
							   let keyAttr = property.GetCustomAttributes(typeof(KeyAttribute), false).Cast<KeyAttribute>().FirstOrDefault()
							   where keyAttr != null
							   select property).FirstOrDefault();
            if (keyProperty == null)
			{
                throw new ArgumentException("TEntity has no [Key] attribute", nameof(entity));
            }
            var entityObject = await DatabaseContext.FindAsync(entity.GetType(), keyProperty.GetValue(entity)).ConfigureAwait(false);
            if (entityObject is not TEntity existing)
            {
                throw new ArgumentException("TEntity does not exists", nameof(entity));
            }
            DatabaseContext.Entry(existing).CurrentValues.SetValues(entity);
		}
    }
}