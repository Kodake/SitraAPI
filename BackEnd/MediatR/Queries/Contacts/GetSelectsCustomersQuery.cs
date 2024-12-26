using AutoMapper;
using BackEnd.DTOs.Contacts.Contacts;
using BackEnd.Enums;
using BackEnd.Repositories;

namespace BackEnd.MediatR.Queries.Contacts
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
