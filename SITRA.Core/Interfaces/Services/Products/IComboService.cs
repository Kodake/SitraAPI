using SITRA.Core.Entities.Products;
using SITRA.Core.Enums;

namespace SITRA.Core.Interfaces.Services.Products
{
    public interface IComboService
    {
        Task<List<Item>> GetAllAsync(StatusType status, CancellationToken cancellationToken = default);
        Task<IEnumerable<Item>> GetStockCombosAsync(CancellationToken cancellationToken = default);
    }
}
