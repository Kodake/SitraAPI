using AutoMapper;
using BackEnd.DTOs;
using BackEnd.DTOs.Contacts.Contacts;
using BackEnd.Repositories;

namespace BackEnd.Mediator.Queries.Contacts
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
