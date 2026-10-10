using SITRA.Core.DTOs;
using SITRA.Core.Entities.Accounts;
using SITRA.Core.Interfaces.Generics;

namespace SITRA.Core.Interfaces.Accounts
{
    public interface IReceivableRepository : IRepositoryBase<Receivable>
    {

        Task<Receivable?> GetAllByIdAsync(Guid id, CancellationToken cancellationToken = default);
    }
}

