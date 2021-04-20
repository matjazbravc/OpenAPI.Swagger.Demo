using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CompanyWebApi.Persistence.Repositories
{
	/// <summary>
	/// Generic asynchronous entity repository
	/// </summary>
	/// <typeparam name="TEntity">Entity type</typeparam>
	public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
	{
		protected readonly DbContext DatabaseContext;
		protected readonly DbSet<TEntity> DatabaseSet;

		protected BaseRepository(DbContext context)
		{
			DatabaseContext = context ?? throw new ArgumentException(nameof(context));
			DatabaseSet = DatabaseContext.Set<TEntity>();
		}

        /// <summary>
        /// Add entity
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
		public virtual async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
		{
			await DatabaseSet.AddAsync(entity, cancellationToken).ConfigureAwait(false);
			await DatabaseContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
			return entity;
		}

        /// <summary>
        /// Add list of entities
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
		public virtual async Task<int> AddAsync(IList<TEntity> entities, CancellationToken cancellationToken = default)
		{
			await DatabaseSet.AddRangeAsync(entities, cancellationToken).ConfigureAwait(false);
			return await DatabaseContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
		}

        /// <summary>
        /// Count entities
        /// </summary>
        /// <param name="disableTracking"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
		public virtual async Task<int> CountAsync(bool disableTracking = true, CancellationToken cancellationToken = default)
		{
			IQueryable<TEntity> query = DatabaseSet;
			if (disableTracking)
			{
				query = query.AsNoTracking();
			}
			return await query.CountAsync(cancellationToken).ConfigureAwait(false);
		}

        /// <summary>
        /// Delete entity
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
		public virtual async Task<int> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
		{
			DatabaseSet.Remove(entity);
			return await DatabaseContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
		}

		/// <summary>
		/// Get all entities by predicate
		/// </summary>
		/// <param name="predicate"></param>
		/// <param name="disableTracking"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		/// usage:
		/// var ownerId = 1;
		/// var owner = await FindByConditionAsync(o => o.Id.Equals(ownerId));
		public virtual async Task<IList<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate, bool disableTracking = true, CancellationToken cancellationToken = default)
		{
			IQueryable<TEntity> query = DatabaseSet;
			if (disableTracking)
			{
				query = query.AsNoTracking();
			}
			return await query.Where(predicate).ToListAsync(cancellationToken).ConfigureAwait(false);
		}

        /// <summary>
        /// Get all entities
        /// </summary>
        /// <param name="disableTracking"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
		public virtual async Task<IList<TEntity>> GetAllAsync(bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> query = DatabaseSet;
            if (disableTracking)
            {
                query = query.AsNoTracking();
            }
            return await query.ToListAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Get single entity
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="disableTracking"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
		public virtual async Task<TEntity> GetSingleAsync(Expression<Func<TEntity, bool>> predicate, bool disableTracking = true, CancellationToken cancellationToken = default)
		{
			IQueryable<TEntity> query = DatabaseSet;
			if (disableTracking)
			{
				query = query.AsNoTracking();
			}
			return await query.SingleOrDefaultAsync(predicate, cancellationToken).ConfigureAwait(false);
		}
        
        /// <summary>
        /// Update entity
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="disableTracking"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<TEntity> UpdateAsync(TEntity entity, bool disableTracking = true, CancellationToken cancellationToken = default)
		{
			var properties = entity.GetType().GetProperties();
			var keyProperty = (from property in properties
							   let keyAttr = property.GetCustomAttributes(typeof(KeyAttribute), false).Cast<KeyAttribute>().FirstOrDefault()
							   where keyAttr != null
							   select property).FirstOrDefault();
			if (keyProperty == null || !(await DatabaseContext.FindAsync(entity.GetType(), keyProperty.GetValue(entity)) is TEntity existing))
			{
				return null;
			}
			DatabaseContext.Entry(existing).CurrentValues.SetValues(entity);
			await DatabaseContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
			return existing;
		}
	}
}