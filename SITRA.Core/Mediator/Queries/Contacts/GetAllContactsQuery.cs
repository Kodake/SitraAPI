using AutoMapper;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Contacts.Contacts;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.Contacts
{
    public class GetAllContactsQuery : GetAllQuery<ContactListDTO>
    {
        public GetAllContactsQuery(PaginationDTO pagination) : base(pagination) { }

        public override async Task<List<ContactListDTO>> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var contacts = await unitOfWork.ContactRepository.GetAllAsync(Pagination);
            return mapper.Map<List<ContactListDTO>>(contacts);
        }
    }
}
