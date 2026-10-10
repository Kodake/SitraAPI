using Mediator;
using SITRA.Core.DTOs.Admin;
using SITRA.Core.Enums;

namespace SITRA.Core.Mediator.Queries.Admin
{
    public record GetAdminUsersQuery(
        string? SearchTerm,
        string? Status,
        PlanType? Plan,
        string? SubscriptionStatus,
        DateTime? FromDate,
        DateTime? ToDate,
        int PageIndex,
        int PageSize
    ) : IRequest<(List<AdminUserDTO> Users, int TotalCount)>;

    public record CheckSuperAdminAccessQuery(string? Email, string? Ruc) : IRequest<bool>;
}
