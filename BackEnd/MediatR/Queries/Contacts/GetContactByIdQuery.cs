using AutoMapper;
using BackEnd.DTOs.Contacts.Contacts;
using BackEnd.Repositories;

namespace BackEnd.MediatR.Queries.Contacts
{
    public class GetContactByIdQuery : GetByIdQuery<ContactDTO>
    {
        public GetContactByIdQuery(long id) : base(id) { }

        public override async Task<ContactDTO> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var contact = await unitOfWork.ContactRepository.GetByIdAsync(Id);
            return mapper.Map<ContactDTO>(contact);
        }
    }
}
