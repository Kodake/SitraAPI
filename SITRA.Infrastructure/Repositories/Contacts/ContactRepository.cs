using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SITRA.Core.Entities.Contacts;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces.Contacts;
using SITRA.Infrastructure.Persistence;
using SITRA.Infrastructure.Repositories.Generics;

namespace SITRA.Infrastructure.Repositories.Contacts
{
    public class ContactRepository : RepositoryBase<Contact>, IContactRepository
    {
        public ContactRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor) { }

        public async Task<List<Contact>> GetSuppliers()
        {
            return await context.Contacts
                .Where(x => x.ContactType == ContactType.Provider && x.IsActive)
                .ToListAsync();
        }

        public async Task<List<Contact>> GetCustomers()
        {
            return await context.Contacts
                .Where(x => x.ContactType == ContactType.Customer && x.IsActive)
                .ToListAsync();
        }

        public async Task<List<Contact>> GetBillers()
        {
            return await context.Contacts
                .Where(x => x.ContactType == ContactType.Biller && x.IsActive)
                .ToListAsync();
        }

        public async Task<bool> Exists(Guid id, string name)
        {
            return await context.Contacts.AnyAsync(x => x.Id != id && x.Name == name);
        }

        public async Task<bool> Exists(Guid id)
        {
            return await context.Contacts.AnyAsync(x => x.Id == id);
        }

        public async Task<int> TotalContactsAddedAsync()
        {
            return await context.Contacts.CountAsync(x => x.IsActive);
        }
    }
}


