using SITRA.Core.Entities.Products;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces.Generics;

namespace SITRA.Core.Interfaces.Products
{
    public interface IUnitRepository : IRepositoryBase<Unit>
    {
        Task<List<Unit>> GetAll(StatusType status, CancellationToken cancellationToken = default);
        Task<bool> Exists(Guid id, string name, CancellationToken cancellationToken = default);
    }
}
