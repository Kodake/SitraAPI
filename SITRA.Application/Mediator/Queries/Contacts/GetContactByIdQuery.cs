using AutoMapper;
using SITRA.Core.DTOs.Contacts.Contacts;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.Contacts
{
    public class GetContactByIdQuery : GetByIdQuery<ContactDTO>
    {
        public GetContactByIdQuery(Guid id) : base(id) { }

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
