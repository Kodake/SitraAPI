using SITRA.Core.Entities.Products;
using SITRA.Core.Interfaces.Generics;

namespace SITRA.Core.Interfaces.Products
{
    public interface IPricingRepository : IRepositoryBase<Pricing>
    {
        Task<List<Pricing>> GetAll(CancellationToken cancellationToken = default);
        Task<Pricing?> GetPricingByName(string? name, CancellationToken cancellationToken = default);
        Task<bool> Exists(Guid id, string name, CancellationToken cancellationToken = default);
    }
}
