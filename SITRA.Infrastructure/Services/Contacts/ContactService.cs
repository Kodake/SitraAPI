using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SITRA.Core.DTOs;
using SITRA.Core.Entities.Contacts;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces.Services.Contacts;
using SITRA.Infrastructure.Persistence;
using System.Linq;

namespace SITRA.Infrastructure.Services.Contacts
{
    public class ContactService : IContactService
    {
        private readonly ApplicationDbContext _context;
        private readonly HttpContext? _httpContext;

        public ContactService(
            ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContext = httpContextAccessor.HttpContext;
        }

        public async Task<List<Contact>> GetPaginatedAsync(PaginationDTO paginationDTO, CancellationToken cancellationToken = default)
        {
            var queryable = _context.Contacts.AsNoTracking().AsQueryable();

            if (!string.IsNullOrEmpty(paginationDTO.SearchTerm))
            {
                var searchTerm = paginationDTO.SearchTerm.ToLower();
                queryable = queryable.Where(x => 
                    x.Name.ToLower().Contains(searchTerm) || 
                    (x.Identification != null && x.Identification.ToLower().Contains(searchTerm)) ||
                    (x.Email != null && x.Email.ToLower().Contains(searchTerm)) ||
                    (x.Phone != null && x.Phone.ToLower().Contains(searchTerm))
                );
            }

            queryable = queryable.OrderBy(x => x.Name);

            if (_httpContext != null)
            {
                await _httpContext.AddPaginationParamsToHeaders(queryable);
            }

            return await queryable
                .Paginate(paginationDTO)
                .ToListAsync(cancellationToken);
        }
    }
}
