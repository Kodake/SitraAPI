using BackEnd.Entities.Products;
using BackEnd.Repositories.Generics;

namespace BackEnd.Repositories.Products
{
    public interface IPricingRepository : IRepositoryBase<Pricing>
    {
        Task<List<Pricing>> GetAll();
        Task<Pricing?> GetPricingByName(string? name);
        Task<IEnumerable<Pricing>> GetUnregisteredPricingsForItem(long itemId);
        Task<bool> Exists(long id, string name);
    }
}
