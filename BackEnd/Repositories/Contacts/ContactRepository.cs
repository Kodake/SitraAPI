using BackEnd.Context;
using BackEnd.Entities.Contacts;
using BackEnd.Repositories.Contacts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BackEnd.Repositories.Products
{
    public class ContactRepository : RepositoryBase<Contact>, IContactRepository
    {
        public ContactRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor) { }

        public async Task<List<Contact>> GetCustomers()
        {
            return await context.Contacts
                .Where(x => x.ContactType != Enums.ContactType.Biller && x.IsActive)
                .ToListAsync();
        }

        public async Task<List<Contact>> GetBillers()
        {
            return await context.Contacts
                .Where(x => x.ContactType == Enums.ContactType.Biller && x.IsActive)
                .ToListAsync();
        }

        public async Task<bool> Exists(long id, string name)
        {
            return await context.Contacts.AnyAsync(x => x.Id != id && x.Name == name);
        }
    }
}
