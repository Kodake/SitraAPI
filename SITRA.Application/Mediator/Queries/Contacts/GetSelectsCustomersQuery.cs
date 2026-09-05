using AutoMapper;
using SITRA.Core.DTOs.Contacts.Contacts;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.Contacts
{
    public class GetSelectsCustomersQuery : GetSelectsQuery<ContactListDTO>
    {
        public GetSelectsCustomersQuery(StatusType status = StatusType.All) : base(status) { }

        public override async Task<List<ContactListDTO>> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var contacts = await unitOfWork.ContactRepository.GetCustomers();
            return mapper.Map<List<ContactListDTO>>(contacts);
        }
    }
}
