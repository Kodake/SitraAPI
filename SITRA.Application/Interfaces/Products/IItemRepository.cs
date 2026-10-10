using SITRA.Core.Entities.Products;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces.Generics;

namespace SITRA.Core.Interfaces.Products
{
    public interface IItemRepository : IRepositoryBase<Item>
    {
        Task<bool> Exists(Guid id, ItemType itemType, CancellationToken cancellationToken = default);
    }
}
