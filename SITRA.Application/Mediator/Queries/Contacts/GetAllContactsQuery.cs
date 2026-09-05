using AutoMapper;
using Mediator;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Contacts.Contacts;
using SITRA.Core.Interfaces.Services.Contacts;

namespace SITRA.Core.Mediator.Queries.Contacts
{
    public class GetAllContactsQuery : IRequest<List<ContactListDTO>>
    {
        public PaginationDTO Pagination { get; set; }

        public GetAllContactsQuery(PaginationDTO pagination)
        {
            Pagination = pagination;
        }
    }

    public class GetAllContactsQueryHandler : IRequestHandler<GetAllContactsQuery, List<ContactListDTO>>
    {
        private readonly IContactService _contactService;
        private readonly IMapper _mapper;

        public GetAllContactsQueryHandler(IContactService contactService, IMapper mapper)
        {
            _contactService = contactService;
            _mapper = mapper;
        }

        public async ValueTask<List<ContactListDTO>> Handle(GetAllContactsQuery request, CancellationToken cancellationToken)
        {
            var contacts = await _contactService.GetPaginatedAsync(request.Pagination, cancellationToken);
            return _mapper.Map<List<ContactListDTO>>(contacts);
        }
    }
}
