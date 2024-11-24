using BackEnd.DTOs;
using BackEnd.Entities.Products;

namespace BackEnd.Repositories.Products
{
    public interface IPricingRepository
    {
        Task<List<Pricing>> GetAll(PaginationDTO paginationDTO);
        Task<Pricing?> GetById(long id);
        Task<List<Pricing>> GetPricings();
        Task<Pricing> Create(Pricing pricing);
        Task Update(Pricing pricing);
        Task<Pricing?> GetPricingByName(string? name);
        Task<IEnumerable<Pricing>> GetUnregisteredPricingsForItem(long itemId);
        Task<bool> Exists(long id);
        Task<bool> Exists(long id, string name);
    }
}
