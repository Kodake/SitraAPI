using SITRA.Core.Entities.Products;
using SITRA.Core.Interfaces.Generics;

namespace SITRA.Core.Interfaces.Products
{
    public interface IPricingRepository : IRepositoryBase<Pricing>
    {
        Task<List<Pricing>> GetAll();
        Task<Pricing?> GetPricingByName(string? name);
        Task<IEnumerable<Pricing>> GetUnregisteredPricingsForItem(Guid itemId);
        Task<bool> Exists(Guid id, string name);
    }
}
