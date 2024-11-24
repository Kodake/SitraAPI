using BackEnd.DTOs;
using BackEnd.Entities.AdministrativeTools;
using BackEnd.Enums;

namespace BackEnd.Repositories.AdministrativeTools
{
    public interface IFiscalSupportRepository
    {
        Task<List<FiscalSupport>> GetAll(PaginationDTO paginationDTO);
        Task<List<FiscalSupport>> GetAll(StatusType status);
        Task<FiscalSupport?> GetById(long id);
        Task<long> Create(FiscalSupport fiscalSupport);
        Task Update(FiscalSupport fiscalSupport);
        Task Activate(long id);
        Task Deactivate(long id);
        Task<bool> Exists(long id);
        Task<bool> Exists(long id, string code);
        Task<List<FiscalSupport>> GetByDocumentTypeAndWarehouseId(FiscalSupportType supportType, long warehouseId);
    }
}
