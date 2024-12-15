using BackEnd.DTOs;

namespace BackEnd.Repositories.Generics
{
    public interface IRepositoryBase<T> where T : class
    {
        Task<List<T>> GetAllAsync(PaginationDTO paginationDTO);
        Task<T?> GetByIdAsync(long id);
        Task<T> CreateAsync(T entity);
        T Update(T entity);
        Task ActivateAsync(long id);
        Task DeactivateAsync(long id);
        Task<bool> ExistsAsync(long id);
    }
}
