using SITRA.Core.DTOs;
using SITRA.Core.Entities.Warehouses;

namespace SITRA.Core.Interfaces.Services.Warehouses
{
    public interface ITransferService
    {
        Task<List<Transfer>> GetAllAsync(PaginationDTO paginationDTO, CancellationToken cancellationToken = default);
    }
}
