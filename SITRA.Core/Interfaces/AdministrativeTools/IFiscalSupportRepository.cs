using SITRA.Core.DTOs;
using SITRA.Core.Entities.AdministrativeTools;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces.Generics;

namespace SITRA.Core.Interfaces.AdministrativeTools
{
    public interface IFiscalSupportRepository : IRepositoryBase<FiscalSupport>
    {
        new Task<List<FiscalSupport>> GetAllAsync(PaginationDTO paginationDTO, CancellationToken cancellationToken = default);
        new Task<List<FiscalSupport>> GetReadOnlyAsync(PaginationDTO paginationDTO, CancellationToken cancellationToken = default);
        Task<FiscalSupport?> GetByIdAsync(Guid? id, CancellationToken cancellationToken = default);
        Task<FiscalSupport?> GetByDocumentTypeAsync(
            EcfType supportType,
            CancellationToken cancellationToken = default);
        Task<bool> Exists(Guid id, string code, CancellationToken cancellationToken = default);
        Task<FiscalSupport?> GetByTypeAsync(EcfType fiscalSupportType, CancellationToken cancellationToken = default);
        Task<bool> SetDefault(Guid id, CancellationToken cancellationToken = default);
    }
}

