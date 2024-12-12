using BackEnd.Entities.Contacts;
using BackEnd.Repositories.Generics;

namespace BackEnd.Repositories.Contacts
{
    public interface IContactRepository : IRepositoryBase<Contact>
    {
        Task<List<Contact>> GetCustomers();
        Task<List<Contact>> GetBillers();
        Task<bool> Exists(long id, string code);
    }
}
