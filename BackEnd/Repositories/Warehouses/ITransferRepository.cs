using BackEnd.DTOs;
using BackEnd.Entities.Warehouses;
using BackEnd.Repositories.Generics;

namespace BackEnd.Repositories.Warehouses
{
    public interface ITransferRepository : IRepositoryBase<Transfer>
    {
        new Task<Transfer?> GetByIdAsync(long id);
        new Task<List<Transfer>> GetAllAsync(PaginationDTO paginationDTO);
    }
}
