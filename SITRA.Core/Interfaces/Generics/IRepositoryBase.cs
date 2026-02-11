using SITRA.Core.DTOs;
using System.Linq.Expressions;

namespace SITRA.Core.Interfaces.Generics
{
    public interface IRepositoryBase<T> where T : class
    {
        Task<List<T>> GetAllAsync(PaginationDTO paginationDTO, CancellationToken cancellationToken = default);
        Task<List<T>> GetAllAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default);
        Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<T> CreateAsync(T entity, CancellationToken cancellationToken = default);
        T Update(T entity);
        Task ActivateAsync(Guid id, CancellationToken cancellationToken = default);
        Task DeactivateAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default);
    }
}
