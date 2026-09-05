using SITRA.Core.DTOs;
using SITRA.Core.Entities.Invoices;
using SITRA.Core.Interfaces.Generics;

namespace SITRA.Core.Interfaces.Invoices
{
    public interface IQuotationRepository : IRepositoryBase<Quotation>
    {

        new Task<Quotation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(Guid id, string quotationNumber, CancellationToken cancellationToken = default);
    }
}
