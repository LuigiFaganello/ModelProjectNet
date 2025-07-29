using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IRepositoryBase<TEntity> where TEntity : class
    {
        // Métodos síncronos
        TEntity GetById(object id);
        TEntity GetByIdWithIncludes(object id, params Expression<Func<TEntity, object>>[] includes);
        IEnumerable<TEntity> GetAll();
        IEnumerable<TEntity> GetAllWithIncludes(params Expression<Func<TEntity, object>>[] includes);
        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);
        IEnumerable<TEntity> FindWithIncludes(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes);
        TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate);
        TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate);
        bool Any(Expression<Func<TEntity, bool>> predicate);
        int Count(Expression<Func<TEntity, bool>> predicate = null);

        // Métodos assíncronos
        Task<TEntity> GetByIdAsync(object id, CancellationToken cancellationToken = default);
        Task<TEntity> GetByIdWithIncludesAsync(object id, CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] includes);
        Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<TEntity>> GetAllWithIncludesAsync(CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] includes);
        Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
        Task<IEnumerable<TEntity>> FindWithIncludesAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] includes);
        Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate = null, CancellationToken cancellationToken = default);

        // Paginação
        Task<(IEnumerable<TEntity> Items, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            CancellationToken cancellationToken = default,
            params Expression<Func<TEntity, object>>[] includes);

        // Operações de modificação
        TEntity Add(TEntity entity);
        Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);
        void AddRange(IEnumerable<TEntity> entities);
        Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

        void Update(TEntity entity);
        void UpdateRange(IEnumerable<TEntity> entities);

        void Remove(TEntity entity);
        void Remove(object id);
        void RemoveRange(IEnumerable<TEntity> entities);

        // Transações e SaveChanges
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        int SaveChanges();

        // Funcionalidades avançadas
        IQueryable<TEntity> AsQueryable();
        Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
        Task BulkInsertAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
        Task BulkUpdateAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
        Task BulkDeleteAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    }
}
