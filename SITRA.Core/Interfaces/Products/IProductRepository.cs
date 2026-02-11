using SITRA.Core.DTOs;
using SITRA.Core.Entities.Products;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces.Generics;

namespace SITRA.Core.Interfaces.Products;

public interface IProductRepository : IRepositoryBase<Item>
{
    new Task<Item?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    new Task<List<Item>> GetAllAsync(PaginationDTO paginationDTO, CancellationToken cancellationToken = default);
    Task<List<Item>> GetAll(StatusType status, CancellationToken cancellationToken = default);
    Task<IEnumerable<Item>> GetAll(CancellationToken cancellationToken = default);
    new Task ActivateAsync(Guid id, CancellationToken cancellationToken = default);
    new Task DeactivateAsync(Guid id, CancellationToken cancellationToken = default);
    new Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> Exists(Guid id, string name, CancellationToken cancellationToken = default);
}
