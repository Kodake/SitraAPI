using SITRA.Core.Entities.Contacts;
using SITRA.Core.Interfaces.Generics;

namespace SITRA.Core.Interfaces.Contacts
{
    public interface IContactRepository : IRepositoryBase<Contact>
    {
        Task<List<Contact>> GetSuppliers(CancellationToken cancellationToken = default);
        Task<List<Contact>> GetCustomers(CancellationToken cancellationToken = default);
        Task<List<Contact>> GetBillers(CancellationToken cancellationToken = default);
        Task<bool> Exists(Guid id, string code, CancellationToken cancellationToken = default);
        Task<bool> Exists(Guid id, CancellationToken cancellationToken = default);
        Task<bool> SetDefault(Guid id, CancellationToken cancellationToken = default);
    }
}
