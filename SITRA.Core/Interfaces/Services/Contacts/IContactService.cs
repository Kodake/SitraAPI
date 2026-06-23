using SITRA.Core.DTOs;
using SITRA.Core.Entities.Contacts;

namespace SITRA.Core.Interfaces.Services.Contacts
{
    public interface IContactService
    {
        Task<List<Contact>> GetPaginatedAsync(PaginationDTO paginationDTO, CancellationToken cancellationToken = default);
    }
}
