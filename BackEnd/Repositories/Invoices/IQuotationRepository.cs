using BackEnd.DTOs;
using BackEnd.Entities.Invoices;
using BackEnd.Repositories.Generics;

namespace BackEnd.Repositories.Invoices
{
    public interface IQuotationRepository : IRepositoryBase<Quotation>
    {
        new Task<List<Quotation>> GetAllAsync(PaginationDTO paginationDTO);
        new Task<Quotation?> GetByIdAsync(long id);
    }
}
