using SITRA.Core.DTOs;
using SITRA.Core.Entities.Accounts;
using SITRA.Core.Interfaces.Generics;

namespace SITRA.Core.Interfaces.Accounts
{
    public interface IPayableRepository : IRepositoryBase<Payable>
    {

        Task<Payable?> GetAllByIdAsync(Guid id, CancellationToken cancellationToken = default);
    }
}

