using SITRA.Core.DTOs.Admin;
using SITRA.Core.Enums;

namespace SITRA.Core.Interfaces.Services.Admin
{
    public interface IAdminService
    {
        Task<bool> IsSuperAdminAsync(string? userEmail, string? userRnc);
        Task<AdminMetricsDTO> GetMetricsAsync(DateTime? fromDate, DateTime? toDate, CancellationToken cancellationToken = default);
        Task<(List<AdminUserDTO> Users, int TotalCount)> GetUsersAsync(
            string? searchTerm,
            string? status,
            PlanType? plan,
            string? subscriptionStatus,
            DateTime? fromDate,
            DateTime? toDate,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default);
        Task<bool> UpdateUserStatusAsync(string userId, UpdateUserStatusDTO dto, CancellationToken cancellationToken = default);
        Task<bool> EnableUserAsync(string userId, CancellationToken cancellationToken = default);
        Task<bool> DisableUserAsync(string userId, string? reason = null, CancellationToken cancellationToken = default);
        Task<bool> UpdateSubscriptionAsync(string userId, PlanType plan, DateTime? startDate, DateTime? endDate, CancellationToken cancellationToken = default);
    }
}
