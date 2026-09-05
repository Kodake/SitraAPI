using AutoMapper;
using Mediator;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.AdministrativeTools.Roles;
using SITRA.Core.Interfaces.Services.AdministrativeTools;

namespace SITRA.Core.Mediator.Queries.AdministrativeTools.Roles
{
    public class GetAllRolesQuery : IRequest<List<RoleListDTO>>
    {
        public PaginationDTO Pagination { get; set; }
        public GetAllRolesQuery(PaginationDTO pagination)
        {
            Pagination = pagination;
        }
    }

    public class GetAllRolesQueryHandler : IRequestHandler<GetAllRolesQuery, List<RoleListDTO>>
    {
        private readonly IRoleService _roleService;
        private readonly IMapper _mapper;

        public GetAllRolesQueryHandler(IRoleService roleService, IMapper mapper)
        {
            _roleService = roleService;
            _mapper = mapper;
        }

        public async ValueTask<List<RoleListDTO>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
        {
            var roles = await _roleService.GetAllAsync(request.Pagination, cancellationToken);
            return _mapper.Map<List<RoleListDTO>>(roles);
        }
    }
}
