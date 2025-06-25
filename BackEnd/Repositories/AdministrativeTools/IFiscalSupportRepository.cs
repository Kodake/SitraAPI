using BackEnd.DTOs;
using BackEnd.Entities.AdministrativeTools;
using BackEnd.Enums;
using BackEnd.Repositories.Generics;

namespace BackEnd.Repositories.AdministrativeTools
{
    public interface IFiscalSupportRepository : IRepositoryBase<FiscalSupport>
    {
        new Task<List<FiscalSupport>> GetAllAsync(PaginationDTO paginationDTO);
        Task<List<FiscalSupport>> GetAll(StatusType status);
        Task<FiscalSupport?> GetByIdAsync(long? id);
        Task<List<FiscalSupport>> GetByDocumentTypeAndWarehouseId(FiscalSupportType supportType, long warehouseId);
        Task<bool> Exists(long id, string code);
    }
}
