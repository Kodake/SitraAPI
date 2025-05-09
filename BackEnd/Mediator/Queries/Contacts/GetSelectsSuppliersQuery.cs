using AutoMapper;
using BackEnd.DTOs.Contacts.Contacts;
using BackEnd.Enums;
using BackEnd.Repositories;

namespace BackEnd.Mediator.Queries.Contacts
{
    public class GetSelectsSuppliersQuery : GetSelectsQuery<ContactListDTO>
    {
        public GetSelectsSuppliersQuery(StatusType status = StatusType.All) : base(status) { }

        public override async Task<List<ContactListDTO>> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var contacts = await unitOfWork.ContactRepository.GetSuppliers();
            return mapper.Map<List<ContactListDTO>>(contacts);
        }
    }
}
