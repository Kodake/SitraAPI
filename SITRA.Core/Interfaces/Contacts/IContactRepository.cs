using SITRA.Core.Entities.Contacts;
using SITRA.Core.Interfaces.Generics;

namespace SITRA.Core.Interfaces.Contacts
{
    public interface IContactRepository : IRepositoryBase<Contact>
    {
        Task<List<Contact>> GetSuppliers();
        Task<List<Contact>> GetCustomers();
        Task<List<Contact>> GetBillers();
        Task<bool> Exists(long id, string code);
        Task<bool> Exists(long id);
        Task<int> TotalContactsAddedAsync();
    }
}
