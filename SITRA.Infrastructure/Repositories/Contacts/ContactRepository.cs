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

        public async Task<List<Contact>> GetSuppliers(CancellationToken cancellationToken = default)
        {
            return await context.Contacts
                .Where(x => x.ContactType == ContactType.Provider && x.IsActive)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Contact>> GetCustomers(CancellationToken cancellationToken = default)
        {
            return await context.Contacts
                .Where(x => x.ContactType == ContactType.Customer && x.IsActive)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Contact>> GetBillers(CancellationToken cancellationToken = default)
        {
            return await context.Contacts
                .Where(x => x.ContactType == ContactType.Biller && x.IsActive)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> Exists(Guid id, string name, CancellationToken cancellationToken = default)
        {
            return await context.Contacts.AnyAsync(x => x.Id != id && x.Name == name, cancellationToken);
        }

        public async Task<bool> Exists(Guid id, CancellationToken cancellationToken = default)
        {
            return await context.Contacts.AnyAsync(x => x.Id == id, cancellationToken);
        }

    }
}


