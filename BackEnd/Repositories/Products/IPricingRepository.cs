using BackEnd.DTOs;
using BackEnd.Entities.Products;

namespace BackEnd.Repositories.Products
{
    public interface IPricingRepository
    {
        Task<List<Pricing>> GetAll(PaginationDTO paginationDTO);
        Task<Pricing?> GetById(int id);
        Task<Pricing> Create(Pricing pricing);
        Task Update(Pricing pricing);
        Task<Pricing?> GetPricingByName(string? name);
        Task<IEnumerable<Pricing>> GetUnregisteredPricingsForItem(int itemId);
        Task<bool> Exists(int id);
        Task<bool> Exists(int id, string name);
    }
}
