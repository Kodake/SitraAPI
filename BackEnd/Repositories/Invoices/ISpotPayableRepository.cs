using BackEnd.DTOs;
using BackEnd.Entities.Invoices;
using BackEnd.Repositories.Generics;

namespace BackEnd.Repositories.Invoices
{
    public interface ISpotPayableRepository : IRepositoryBase<SpotPayable>
    {
        new Task<List<SpotPayable>> GetAllAsync(PaginationDTO paginationDTO);
        new Task<SpotPayable?> GetByIdAsync(long id);
    }
}