using MediatR;
using Microsoft.Extensions.Configuration;
using SITRA.Core.DTOs.AdministrativeTools;

namespace SITRA.Core.Mediator.Queries.AdministrativeTools.Roles
{
    public record GetPermissionConfigQuery : IRequest<PermissionManagementConfigDTO>;

    public class GetPermissionConfigHandler : IRequestHandler<GetPermissionConfigQuery, PermissionManagementConfigDTO>
    {
        private readonly IConfiguration _configuration;

        public GetPermissionConfigHandler(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task<PermissionManagementConfigDTO> Handle(GetPermissionConfigQuery request, CancellationToken cancellationToken)
        {
            var config = _configuration.GetSection("PermissionManagement").Get<PermissionManagementConfigDTO>();
            return Task.FromResult(config ?? new PermissionManagementConfigDTO());
        }
    }
}
