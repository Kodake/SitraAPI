using BackEnd.DTOs;
using BackEnd.Entities.Invoices;
using BackEnd.Repositories.Generics;

namespace BackEnd.Repositories.Invoices
{
    public interface ISpotPaymentRepository : IRepositoryBase<SpotPayment>
    {
        new Task<List<SpotPayment>> GetAllAsync(PaginationDTO paginationDTO);
    }
}
