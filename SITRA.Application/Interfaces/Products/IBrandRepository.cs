using SITRA.Core.Entities.Products;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces.Generics;

namespace SITRA.Core.Interfaces.Products
{
    public interface IBrandRepository : IRepositoryBase<Brand>
    {
        Task<bool> Exists(Guid id, string code, CancellationToken cancellationToken = default);
    }
}
