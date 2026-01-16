using SITRA.Core.DTOs;
using SITRA.Core.Entities.AdministrativeTools;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces.Generics;

namespace SITRA.Core.Interfaces.AdministrativeTools
{
    public interface IFiscalSupportRepository : IRepositoryBase<FiscalSupport>
    {
        new Task<List<FiscalSupport>> GetAllAsync(PaginationDTO paginationDTO);
        Task<List<FiscalSupport>> GetAll(StatusType status);
        Task<FiscalSupport?> GetByIdAsync(long? id);
        Task<FiscalSupport?> GetByDocumentTypeAndWarehouseIdAsync(
            FiscalSupportType supportType,
            long warehouseId);
        Task<bool> Exists(long id, string code);
        Task<FiscalSupport?> GetByTypeAndWarehouseAsync(FiscalSupportType fiscalSupportType, long warehouseId);
    }
}
