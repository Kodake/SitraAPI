using BackEnd.DTOs;
using BackEnd.Entities.Warehouses;

namespace BackEnd.Repositories.Warehouses
{
    public interface ITransferRepository
    {
        Task<Transfer?> GetById(long id);
        Task<List<Transfer>> GetAll(PaginationDTO paginationDTO);
        Task<long> Create(Transfer transfer);
        Task Update(Transfer transfer);
    }
}
