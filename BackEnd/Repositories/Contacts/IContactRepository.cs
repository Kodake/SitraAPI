using BackEnd.DTOs;
using BackEnd.Entities.Contacts;

namespace BackEnd.Repositories.Contacts
{
    public interface IContactRepository
    {
        Task<Contact?> GetById(int id);
        Task<List<Contact>> GetCustomers();
        Task<List<Contact>> GetBillers();
        Task<List<Contact>> GetAll(PaginationDTO paginationDTO);
        Task<int> Create(Contact contact);
        Task Update(Contact contact);
        Task Activate(int id);
        Task Deactivate(int id);
        Task<bool> Exists(int id);
        Task<bool> Exists(int id, string code);
    }
}
