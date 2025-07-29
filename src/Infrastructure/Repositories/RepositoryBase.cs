using System.Linq.Expressions;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class RepositoryBase<TEntity> : IRepositoryBase<TEntity> where TEntity : class
    {
        protected readonly DbContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        public RepositoryBase(DbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = _context.Set<TEntity>();
        }

        #region Métodos síncronos

        public virtual TEntity GetById(object id)
        {
            return _dbSet.Find(id);
        }

        public virtual TEntity GetByIdWithIncludes(object id, params Expression<Func<TEntity, object>>[] includes)
        {
            var query = _dbSet.AsQueryable();

            if (includes != null)
            {
                query = includes.Aggregate(query, (current, include) => current.Include(include));
            }

            // Assume que a entidade tem uma propriedade Id
            var parameter = Expression.Parameter(typeof(TEntity), "x");
            var property = Expression.Property(parameter, "Id");
            var constant = Expression.Constant(id);
            var equal = Expression.Equal(property, constant);
            var lambda = Expression.Lambda<Func<TEntity, bool>>(equal, parameter);

            return query.FirstOrDefault(lambda);
        }

        public virtual IEnumerable<TEntity> GetAll()
        {
            return _dbSet.ToList();
        }

        public virtual IEnumerable<TEntity> GetAllWithIncludes(params Expression<Func<TEntity, object>>[] includes)
        {
            var query = _dbSet.AsQueryable();

            if (includes != null)
            {
                query = includes.Aggregate(query, (current, include) => current.Include(include));
            }

            return query.ToList();
        }

        public virtual IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return _dbSet.Where(predicate).ToList();
        }

        public virtual IEnumerable<TEntity> FindWithIncludes(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes)
        {
            var query = _dbSet.AsQueryable();

            if (includes != null)
            {
                query = includes.Aggregate(query, (current, include) => current.Include(include));
            }

            return query.Where(predicate).ToList();
        }

        public virtual TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return _dbSet.SingleOrDefault(predicate);
        }

        public virtual TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return _dbSet.FirstOrDefault(predicate);
        }

        public virtual bool Any(Expression<Func<TEntity, bool>> predicate)
        {
            return _dbSet.Any(predicate);
        }

        public virtual int Count(Expression<Func<TEntity, bool>> predicate = null)
        {
            return predicate == null ? _dbSet.Count() : _dbSet.Count(predicate);
        }

        #endregion

        #region Métodos assíncronos

        public virtual async Task<TEntity> GetByIdAsync(object id, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
        }

        public virtual async Task<TEntity> GetByIdWithIncludesAsync(object id, CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] includes)
        {
            var query = _dbSet.AsQueryable();

            if (includes != null)
            {
                query = includes.Aggregate(query, (current, include) => current.Include(include));
            }

            var parameter = Expression.Parameter(typeof(TEntity), "x");
            var property = Expression.Property(parameter, "Id");
            var constant = Expression.Constant(id);
            var equal = Expression.Equal(property, constant);
            var lambda = Expression.Lambda<Func<TEntity, bool>>(equal, parameter);

            return await query.FirstOrDefaultAsync(lambda, cancellationToken);
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet.ToListAsync(cancellationToken);
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllWithIncludesAsync(CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] includes)
        {
            var query = _dbSet.AsQueryable();

            if (includes != null)
            {
                query = includes.Aggregate(query, (current, include) => current.Include(include));
            }

            return await query.ToListAsync(cancellationToken);
        }

        public virtual async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _dbSet.Where(predicate).ToListAsync(cancellationToken);
        }

        public virtual async Task<IEnumerable<TEntity>> FindWithIncludesAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] includes)
        {
            var query = _dbSet.AsQueryable();

            if (includes != null)
            {
                query = includes.Aggregate(query, (current, include) => current.Include(include));
            }

            return await query.Where(predicate).ToListAsync(cancellationToken);
        }

        public virtual async Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _dbSet.SingleOrDefaultAsync(predicate, cancellationToken);
        }

        public virtual async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate, cancellationToken);
        }

        public virtual async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AnyAsync(predicate, cancellationToken);
        }

        public virtual async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate = null, CancellationToken cancellationToken = default)
        {
            return predicate == null
                ? await _dbSet.CountAsync(cancellationToken)
                : await _dbSet.CountAsync(predicate, cancellationToken);
        }

        #endregion

        #region Paginação

        public virtual async Task<(IEnumerable<TEntity> Items, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            CancellationToken cancellationToken = default,
            params Expression<Func<TEntity, object>>[] includes)
        {
            var query = _dbSet.AsQueryable();

            if (includes != null)
            {
                query = includes.Aggregate(query, (current, include) => current.Include(include));
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }

        #endregion

        #region Operações de modificação

        public virtual TEntity Add(TEntity entity)
        {
            return _dbSet.Add(entity).Entity;
        }

        public virtual async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            var result = await _dbSet.AddAsync(entity, cancellationToken);
            return result.Entity;
        }

        public virtual void AddRange(IEnumerable<TEntity> entities)
        {
            _dbSet.AddRange(entities);
        }

        public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddRangeAsync(entities, cancellationToken);
        }

        public virtual void Update(TEntity entity)
        {
            _dbSet.Update(entity);
        }

        public virtual void UpdateRange(IEnumerable<TEntity> entities)
        {
            _dbSet.UpdateRange(entities);
        }

        public virtual void Remove(TEntity entity)
        {
            _dbSet.Remove(entity);
        }

        public virtual void Remove(object id)
        {
            var entity = _dbSet.Find(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
            }
        }

        public virtual void RemoveRange(IEnumerable<TEntity> entities)
        {
            _dbSet.RemoveRange(entities);
        }

        #endregion

        #region SaveChanges

        public virtual async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public virtual int SaveChanges()
        {
            return _context.SaveChanges();
        }

        #endregion

        #region Funcionalidades avançadas

        public virtual IQueryable<TEntity> AsQueryable()
        {
            return _dbSet.AsQueryable();
        }

        public virtual async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AnyAsync(predicate, cancellationToken);
        }

        public virtual async Task BulkInsertAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            // Para operações bulk mais eficientes, considere usar EFCore.BulkExtensions
            await _dbSet.AddRangeAsync(entities, cancellationToken);
            await SaveChangesAsync(cancellationToken);
        }

        public virtual async Task BulkUpdateAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            _dbSet.UpdateRange(entities);
            await SaveChangesAsync(cancellationToken);
        }

        public virtual async Task BulkDeleteAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            var entities = await _dbSet.Where(predicate).ToListAsync(cancellationToken);
            _dbSet.RemoveRange(entities);
            await SaveChangesAsync(cancellationToken);
        }

        #endregion
    }
}