using System.Linq.Expressions;

namespace Main.Domain.InterfacesRepository
{
    public interface IBaseRepository<TEntity>
    {
        Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? filter = null, CancellationToken cancellationToken = default, params string[] includeProperties);
        Task<TEntity?> GetFirstAsync(Expression<Func<TEntity, bool>>? filter = null, CancellationToken cancellationToken = default, params string[] includeProperties);
        Task<TEntity> CreateAsync(TEntity item, CancellationToken cancellationToken = default);
        Task<TEntity> UpdateAsync(TEntity item, CancellationToken cancellationToken = default);
        Task DeleteAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default);
        Task<bool> IsExistsAsync(Expression<Func<TEntity, bool>>? filter = null, CancellationToken cancellationToken = default);
    }
}
