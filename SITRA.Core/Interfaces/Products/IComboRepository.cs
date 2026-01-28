using SITRA.Core.DTOs;
using SITRA.Core.Entities.Products;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces.Generics;

namespace SITRA.Core.Interfaces.Products;

public interface IComboRepository : IRepositoryBase<Item>
{
    new Task<Item?> GetByIdAsync(Guid id);
    new Task<List<Item>> GetAllAsync(PaginationDTO paginationDTO);
    Task<List<Item>> GetAll(StatusType status);
    Task<IEnumerable<Item>> GetAll();
    new Task ActivateAsync(Guid id);
    new Task DeactivateAsync(Guid id);
    new Task<bool> ExistsAsync(Guid id);
    Task<bool> Exists(Guid id, string name);
}
