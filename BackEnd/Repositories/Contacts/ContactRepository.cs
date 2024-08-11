using AutoMapper;
using BackEnd.Context;
using BackEnd.DTOs;
using BackEnd.Entities.Contacts;
using BackEnd.Helpers;
using BackEnd.Repositories.Contacts;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories.Products
{
    public class ContactRepository : IContactRepository
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly HttpContext httpContext;

        public ContactRepository(ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
            httpContext = httpContextAccessor.HttpContext!;
        }

        public async Task<List<Contact>> GetAll(PaginationDTO paginationDTO)
        {
            var queryable = context.Contacts.AsQueryable();
            await httpContext.AddPaginationParamsToHeaders(queryable);
            return await queryable
                .OrderBy(x => x.Name)
                .Paginate(paginationDTO)
                .ToListAsync();
        }

        public async Task<Contact?> GetById(int id)
        {
            return await context.Contacts.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<int> Create(Contact contact)
        {
            context.Add(contact);
            await context.SaveChangesAsync();
            return contact.Id;
        }

        public async Task Update(Contact contact)
        {
            context.Update(contact);
            await context.SaveChangesAsync();
        }

        public async Task Activate(int id)
        {
            var contact = await context.Contacts.FirstOrDefaultAsync(x => x.Id == id);
            if (contact != null)
            {
                contact.IsActive = false;
                await context.SaveChangesAsync();
            }
        }

        public async Task Deactivate(int id)
        {
            var contact = await context.Contacts.FirstOrDefaultAsync(x => x.Id == id);
            if (contact != null)
            {
                contact.IsActive = true;
                await context.SaveChangesAsync();
            }
        }

        public async Task<bool> Exists(int id)
        {
            return await context.Contacts.AnyAsync(x => x.Id == id);
        }

        public async Task<bool> Exists(int id, string name)
        {
            return await context.Contacts.AnyAsync(x => x.Id != id && x.Name == name);
        }
    }
}
