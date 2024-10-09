using BackEnd.DTOs;
using BackEnd.Entities.Warehouses;

namespace BackEnd.Repositories.Warehouses
{
    public interface ITransferRepository
    {
        Task<Transfer?> GetById(int id);
        Task<List<Transfer>> GetAll(PaginationDTO paginationDTO);
        Task<int> Create(Transfer transfer);
        Task Update(Transfer transfer);
    }
}
