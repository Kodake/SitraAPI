using SITRA.Core.DTOs;
using SITRA.Core.Entities.Products;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces.Generics;

namespace SITRA.Core.Interfaces.Products;

public interface IComboRepository : IRepositoryBase<Item>
{
    new Task<Item?> GetByIdAsync(long id);
    new Task<List<Item>> GetAllAsync(PaginationDTO paginationDTO);
    Task<List<Item>> GetAll(StatusType status);
    Task<IEnumerable<Item>> GetAll();
    new Task ActivateAsync(long id);
    new Task DeactivateAsync(long id);
    new Task<bool> ExistsAsync(long id);
    Task<bool> Exists(long id, string name);
}
