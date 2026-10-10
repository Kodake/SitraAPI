using SITRA.Core.DTOs;
using SITRA.Core.Entities.Products;
using SITRA.Core.Interfaces.Generics;

namespace SITRA.Core.Interfaces.Products
{
    public interface ISubCategoryRepository : IRepositoryBase<SubCategory>
    {

        new Task<SubCategory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<SubCategory>> GetByCategoryId(Guid categoryId, CancellationToken cancellationToken = default);
        Task<bool> Exists(Guid id, string name, CancellationToken cancellationToken = default);
    }
}
