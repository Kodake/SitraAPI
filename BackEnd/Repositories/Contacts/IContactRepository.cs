using BackEnd.DTOs;
using BackEnd.Entities.Contacts;

namespace BackEnd.Repositories.Contacts
{
    public interface IContactRepository
    {
        Task<Contact?> GetById(long id);
        Task<List<Contact>> GetCustomers();
        Task<List<Contact>> GetBillers();
        Task<List<Contact>> GetAll(PaginationDTO paginationDTO);
        Task<long> Create(Contact contact);
        Task Update(Contact contact);
        Task Activate(long id);
        Task Deactivate(long id);
        Task<bool> Exists(long id);
        Task<bool> Exists(long id, string code);
    }
}
