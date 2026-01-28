using SITRA.Core.DTOs;
using System.Linq.Expressions;

namespace SITRA.Core.Interfaces.Generics
{
    public interface IRepositoryBase<T> where T : class
    {
        Task<List<T>> GetAllAsync(PaginationDTO paginationDTO);
        Task<List<T>> GetAllAsync(Expression<Func<T, bool>> filter);
        Task<T?> GetByIdAsync(Guid id);
        Task<T> CreateAsync(T entity);
        T Update(T entity);
        Task ActivateAsync(Guid id);
        Task DeactivateAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
    }
}
