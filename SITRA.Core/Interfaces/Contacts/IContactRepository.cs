using SITRA.Core.Entities.Contacts;
using SITRA.Core.Interfaces.Generics;

namespace SITRA.Core.Interfaces.Contacts
{
    public interface IContactRepository : IRepositoryBase<Contact>
    {
        Task<List<Contact>> GetSuppliers();
        Task<List<Contact>> GetCustomers();
        Task<List<Contact>> GetBillers();
        Task<bool> Exists(Guid id, string code);
        Task<bool> Exists(Guid id);
        Task<int> TotalContactsAddedAsync();
    }
}
