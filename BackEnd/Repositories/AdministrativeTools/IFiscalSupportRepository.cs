using BackEnd.DTOs;
using BackEnd.Entities.AdministrativeTools;
using BackEnd.Enums;

namespace BackEnd.Repositories.AdministrativeTools
{
    public interface IFiscalSupportRepository
    {
        Task<List<FiscalSupport>> GetAll(PaginationDTO paginationDTO);
        Task<List<FiscalSupport>> GetAll(StatusType status);
        Task<FiscalSupport?> GetById(int id);
        Task<int> Create(FiscalSupport fiscalSupport);
        Task Update(FiscalSupport fiscalSupport);
        Task Activate(int id);
        Task Deactivate(int id);
        Task<bool> Exists(int id);
        Task<bool> Exists(int id, string code);
        Task<List<FiscalSupport>> GetByDocumentTypeAndWarehouseId(FiscalSupportType supportType, int warehouseId);
    }
}
