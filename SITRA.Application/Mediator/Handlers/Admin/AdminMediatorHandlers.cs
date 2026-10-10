using Mediator;
using SITRA.Core.DTOs.Admin;
using SITRA.Core.Interfaces.Services.Admin;
using SITRA.Core.Mediator.Commands.Admin;
using SITRA.Core.Mediator.Queries.Admin;

namespace SITRA.Core.Mediator.Handlers.Admin
{
    public class CheckSuperAdminAccessQueryHandler : IRequestHandler<CheckSuperAdminAccessQuery, bool>
    {
        private readonly IAdminService _adminService;

        public CheckSuperAdminAccessQueryHandler(IAdminService adminService)
        {
            _adminService = adminService;
        }

        public ValueTask<bool> Handle(CheckSuperAdminAccessQuery request, CancellationToken cancellationToken)
        {
            return new ValueTask<bool>(_adminService.IsSuperAdminAsync(request.Email, request.Ruc));
        }
    }

    public class GetAdminMetricsQueryHandler : IRequestHandler<GetAdminMetricsQuery, AdminMetricsDTO>
    {
        private readonly IAdminService _adminService;

        public GetAdminMetricsQueryHandler(IAdminService adminService)
        {
            _adminService = adminService;
        }

        public async ValueTask<AdminMetricsDTO> Handle(GetAdminMetricsQuery request, CancellationToken cancellationToken)
        {
            return await _adminService.GetMetricsAsync(request.FromDate, request.ToDate, cancellationToken);
        }
    }

    public class GetAdminUsersQueryHandler : IRequestHandler<GetAdminUsersQuery, (List<AdminUserDTO> Users, int TotalCount)>
    {
        private readonly IAdminService _adminService;

        public GetAdminUsersQueryHandler(IAdminService adminService)
        {
            _adminService = adminService;
        }

        public async ValueTask<(List<AdminUserDTO> Users, int TotalCount)> Handle(GetAdminUsersQuery request, CancellationToken cancellationToken)
        {
            return await _adminService.GetUsersAsync(
                request.SearchTerm,
                request.Status,
                request.Plan,
                request.SubscriptionStatus,
                request.FromDate,
                request.ToDate,
                request.PageIndex,
                request.PageSize,
                cancellationToken);
        }
    }

    public class UpdateUserStatusCommandHandler : IRequestHandler<UpdateUserStatusCommand, bool>
    {
        private readonly IAdminService _adminService;

        public UpdateUserStatusCommandHandler(IAdminService adminService)
        {
            _adminService = adminService;
        }

        public async ValueTask<bool> Handle(UpdateUserStatusCommand request, CancellationToken cancellationToken)
        {
            return await _adminService.UpdateUserStatusAsync(request.UserId, request.Dto, cancellationToken);
        }
    }

    public class EnableUserCommandHandler : IRequestHandler<EnableUserCommand, bool>
    {
        private readonly IAdminService _adminService;

        public EnableUserCommandHandler(IAdminService adminService)
        {
            _adminService = adminService;
        }

        public async ValueTask<bool> Handle(EnableUserCommand request, CancellationToken cancellationToken)
        {
            return await _adminService.EnableUserAsync(request.UserId, cancellationToken);
        }
    }

    public class DisableUserCommandHandler : IRequestHandler<DisableUserCommand, bool>
    {
        private readonly IAdminService _adminService;

        public DisableUserCommandHandler(IAdminService adminService)
        {
            _adminService = adminService;
        }

        public async ValueTask<bool> Handle(DisableUserCommand request, CancellationToken cancellationToken)
        {
            return await _adminService.DisableUserAsync(request.UserId, request.Reason, cancellationToken);
        }
    }

    public class UpdateUserSubscriptionCommandHandler : IRequestHandler<UpdateUserSubscriptionCommand, bool>
    {
        private readonly IAdminService _adminService;

        public UpdateUserSubscriptionCommandHandler(IAdminService adminService)
        {
            _adminService = adminService;
        }

        public async ValueTask<bool> Handle(UpdateUserSubscriptionCommand request, CancellationToken cancellationToken)
        {
            return await _adminService.UpdateSubscriptionAsync(
                request.UserId,
                request.Plan,
                request.SubscriptionStartDate,
                request.SubscriptionEndDate,
                cancellationToken);
        }
    }
}

