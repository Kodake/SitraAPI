using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SITRA.Core.DTOs.Admin;
using SITRA.Core.Entities.Auths;
using SITRA.Core.Entities.Tenants;
using SITRA.Core.Enums;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces.Services.Admin;
using SITRA.Infrastructure.Persistence;

namespace SITRA.Infrastructure.Services.Admin
{
    public class AdminService : IAdminService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AdminService> _logger;

        private string SuperAdminRnc => _configuration["SuperAdmin:Rnc"] ?? _configuration["Dgii:DefaultRNC"] ?? "133556545";

        private bool IsSuperAdminRnc(string? rnc)
        {
            if (string.IsNullOrWhiteSpace(rnc)) return false;
            var trimmed = rnc.Trim();
            return trimmed == "133556545" || trimmed == SuperAdminRnc.Trim();
        }

        public AdminService(
            ApplicationDbContext context,
            UserManager<User> userManager,
            IConfiguration configuration,
            ILogger<AdminService> logger)
        {
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> IsSuperAdminAsync(string? userEmail, string? userRnc)
        {
            if (string.IsNullOrWhiteSpace(userEmail) && string.IsNullOrWhiteSpace(userRnc))
                return false;

            if (IsSuperAdminRnc(userRnc))
                return true;

            // Also check DB for matching user with the SuperAdmin RNC
            if (!string.IsNullOrWhiteSpace(userEmail))
            {
                var user = await _userManager.Users
                    .IgnoreQueryFilters()
                    .FirstOrDefaultAsync(u => u.Email == userEmail);

                if (user != null && IsSuperAdminRnc(user.RucNumber))
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<AdminMetricsDTO> GetMetricsAsync(DateTime? fromDate, DateTime? toDate, CancellationToken cancellationToken = default)
        {
            var now = DateTimeOffset.UtcNow;
            var today = DateTime.UtcNow.Date;

            // Fetch all users across tenants
            var usersQuery = _userManager.Users
                .IgnoreQueryFilters()
                .Where(u => u.IsOwner); // Only tenant owners count as primary accounts

            var tenantsQuery = _context.Tenants.IgnoreQueryFilters().AsNoTracking();

            var allUsers = await usersQuery.ToListAsync(cancellationToken);
            var allTenants = await tenantsQuery.ToListAsync(cancellationToken);
            var tenantDict = allTenants
                .GroupBy(t => t.TenantId)
                .ToDictionary(g => g.Key, g => g.First());

            int totalUsers = 0;
            int activeUsers = 0;
            int disabledUsers = 0;
            int activeMemberships = 0;
            int expiringSoonMemberships = 0;
            int expiredMemberships = 0;
            int recentRegistrations = 0;

            var planCounts = new Dictionary<string, int>
            {
                { PlanType.Free.ToString(), 0 },
                { PlanType.Standard.ToString(), 0 },
                { PlanType.Premium.ToString(), 0 },
                { PlanType.Full.ToString(), 0 }
            };

            foreach (var user in allUsers)
            {
                bool isSuperAdmin = IsSuperAdminRnc(user.RucNumber);
                tenantDict.TryGetValue(user.TenantId, out var tenant);

                // Date filter based on tenant subscription start or default
                DateTime userCreated = tenant?.SubscriptionStartDate ?? DateTime.UtcNow.AddMonths(-1);
                if (fromDate.HasValue && userCreated < fromDate.Value)
                    continue;
                if (toDate.HasValue && userCreated > toDate.Value)
                    continue;

                totalUsers++;

                // Active vs Disabled
                bool isDisabled = user.LockoutEnd.HasValue && user.LockoutEnd.Value > now;
                if (isDisabled)
                {
                    disabledUsers++;
                }
                else
                {
                    activeUsers++;
                }

                // Superadmin does not count towards subscription alerts/expiry
                if (isSuperAdmin)
                {
                    activeMemberships++;
                    continue;
                }

                // Plan distribution
                var plan = tenant?.Plan ?? PlanType.Free;
                var planKey = plan.ToString();
                if (planCounts.ContainsKey(planKey))
                {
                    planCounts[planKey]++;
                }
                else
                {
                    planCounts[planKey] = 1;
                }

                // Membership evaluation
                if (tenant?.SubscriptionEndDate.HasValue == true)
                {
                    var days = (tenant.SubscriptionEndDate.Value.Date - today).Days;
                    if (days < 0)
                    {
                        expiredMemberships++;
                    }
                    else if (days <= 15)
                    {
                        expiringSoonMemberships++;
                        activeMemberships++;
                    }
                    else
                    {
                        activeMemberships++;
                    }
                }
                else
                {
                    // Default to active for active plan
                    activeMemberships++;
                }
            }

            // Growth trends (last 6 months or based on tenant counts)
            var growthTrends = new List<GrowthTrendItemDTO>();
            for (int i = 5; i >= 0; i--)
            {
                var monthStart = new DateTime(today.Year, today.Month, 1).AddMonths(-i);
                var monthEnd = monthStart.AddMonths(1).AddDays(-1);
                var label = monthStart.ToString("MMM yyyy");

                var countInMonth = allTenants.Count(t =>
                    t.SubscriptionStartDate.HasValue &&
                    t.SubscriptionStartDate.Value >= monthStart &&
                    t.SubscriptionStartDate.Value <= monthEnd);

                growthTrends.Add(new GrowthTrendItemDTO
                {
                    Period = label,
                    Count = countInMonth > 0 ? countInMonth : (i == 0 ? totalUsers : Math.Max(1, totalUsers / 6))
                });
            }

            return new AdminMetricsDTO
            {
                TotalUsers = totalUsers,
                ActiveUsers = activeUsers,
                DisabledUsers = disabledUsers,
                ActiveMemberships = activeMemberships,
                ExpiringSoonMemberships = expiringSoonMemberships,
                ExpiredMemberships = expiredMemberships,
                RecentRegistrations = recentRegistrations,
                PlanDistribution = planCounts,
                GrowthTrends = growthTrends
            };
        }

        public async Task<(List<AdminUserDTO> Users, int TotalCount)> GetUsersAsync(
            string? searchTerm,
            string? status,
            PlanType? plan,
            string? subscriptionStatus,
            DateTime? fromDate,
            DateTime? toDate,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var now = DateTimeOffset.UtcNow;
            var today = DateTime.UtcNow.Date;

            var users = await _userManager.Users
                .IgnoreQueryFilters()
                .Where(u => u.IsOwner) // Primary tenant accounts
                .ToListAsync(cancellationToken);

            var tenants = await _context.Tenants
                .IgnoreQueryFilters()
                .AsNoTracking()
                .ToListAsync(cancellationToken);
            var tenantDict = tenants
                .GroupBy(t => t.TenantId)
                .ToDictionary(g => g.Key, g => g.First());

            var list = new List<AdminUserDTO>();

            foreach (var u in users)
            {
                tenantDict.TryGetValue(u.TenantId, out var tenant);
                bool isSuperAdmin = IsSuperAdminRnc(u.RucNumber);
                bool isActive = !u.LockoutEnd.HasValue || u.LockoutEnd.Value <= now;

                var subStart = tenant?.SubscriptionStartDate;
                var subEnd = tenant?.SubscriptionEndDate;
                int? daysRemaining = null;
                string subStatus = "Active";

                if (isSuperAdmin)
                {
                    subStatus = "SuperAdmin";
                    daysRemaining = null;
                }
                else if (subEnd.HasValue)
                {
                    daysRemaining = (subEnd.Value.Date - today).Days;
                    if (daysRemaining < 0) subStatus = "Expired";
                    else if (daysRemaining <= 15) subStatus = "ExpiringSoon";
                    else subStatus = "Active";
                }
                else
                {
                    subStatus = "Active";
                }

                var item = new AdminUserDTO
                {
                    Id = u.Id,
                    Email = u.Email ?? string.Empty,
                    UserName = u.UserName ?? string.Empty,
                    CompanyName = u.CompanyName,
                    RucNumber = u.RucNumber,
                    PhoneNumber = u.PhoneNumber,
                    Address = u.Address,
                    Province = u.Province,
                    Municipality = u.Municipality,
                    TenantId = u.TenantId,
                    Plan = tenant?.Plan ?? PlanType.Free,
                    SubscriptionStartDate = subStart,
                    SubscriptionEndDate = subEnd,
                    DaysRemaining = daysRemaining,
                    SubscriptionStatus = subStatus,
                    IsActive = isActive,
                    LockoutEnd = u.LockoutEnd,
                    IsOwner = u.IsOwner,
                    IsSuperAdmin = isSuperAdmin
                };

                list.Add(item);
            }

            // Apply In-Memory Filtering
            var filtered = list.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim().ToLower();
                filtered = filtered.Where(x =>
                    x.Email.ToLower().Contains(term) ||
                    x.UserName.ToLower().Contains(term) ||
                    (x.CompanyName != null && x.CompanyName.ToLower().Contains(term)) ||
                    (x.RucNumber != null && x.RucNumber.ToLower().Contains(term))
                );
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                if (status.Equals("active", StringComparison.OrdinalIgnoreCase))
                    filtered = filtered.Where(x => x.IsActive);
                else if (status.Equals("disabled", StringComparison.OrdinalIgnoreCase))
                    filtered = filtered.Where(x => !x.IsActive);
            }

            if (plan.HasValue)
            {
                filtered = filtered.Where(x => x.Plan == plan.Value);
            }

            if (!string.IsNullOrWhiteSpace(subscriptionStatus))
            {
                if (subscriptionStatus.Equals("active", StringComparison.OrdinalIgnoreCase))
                    filtered = filtered.Where(x => x.SubscriptionStatus == "Active" || x.IsSuperAdmin);
                else if (subscriptionStatus.Equals("expiringSoon", StringComparison.OrdinalIgnoreCase))
                    filtered = filtered.Where(x => x.SubscriptionStatus == "ExpiringSoon" && !x.IsSuperAdmin);
                else if (subscriptionStatus.Equals("expired", StringComparison.OrdinalIgnoreCase))
                    filtered = filtered.Where(x => x.SubscriptionStatus == "Expired" && !x.IsSuperAdmin);
            }

            if (fromDate.HasValue)
            {
                filtered = filtered.Where(x => !x.SubscriptionStartDate.HasValue || x.SubscriptionStartDate.Value >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                filtered = filtered.Where(x => !x.SubscriptionStartDate.HasValue || x.SubscriptionStartDate.Value <= toDate.Value);
            }

            // Put SuperAdmin at the very top, then sort by company name / username
            var sorted = filtered
                .OrderByDescending(x => x.IsSuperAdmin)
                .ThenBy(x => x.CompanyName ?? x.UserName)
                .ToList();

            int totalCount = sorted.Count;

            int page = pageIndex > 0 ? pageIndex : 1;
            int size = pageSize > 0 ? pageSize : 10;
            var paged = sorted.Skip((page - 1) * size).Take(size).ToList();

            return (paged, totalCount);
        }

        public async Task<bool> UpdateUserStatusAsync(string userId, UpdateUserStatusDTO dto, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.Users
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

            if (user == null)
            {
                _logger.LogWarning("Admin update: User {UserId} not found.", userId);
                return false;
            }

            bool isSuperAdmin = IsSuperAdminRnc(user.RucNumber);
            if (isSuperAdmin && !dto.IsActive)
            {
                _logger.LogWarning("Admin update: Attempted to disable SuperAdmin SISTRA SRL user.");
                throw new InvalidOperationException(Utilities.CannotDisableSuperAdminMessage);
            }

            // Update Lockout status
            if (dto.IsActive)
            {
                await _userManager.SetLockoutEndDateAsync(user, null);
                _logger.LogInformation("Admin user {UserId} enabled.", userId);
            }
            else
            {
                await _userManager.SetLockoutEnabledAsync(user, true);
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
                _logger.LogInformation("Admin user {UserId} disabled manually.", userId);
            }

            // Update Tenant subscription dates & plan if provided
            var tenant = await _context.Tenants
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(t => t.TenantId == user.TenantId, cancellationToken);

            if (tenant != null)
            {
                if (dto.SubscriptionEndDate.HasValue)
                {
                    tenant.SubscriptionEndDate = dto.SubscriptionEndDate.Value;
                }
                if (dto.Plan.HasValue)
                {
                    tenant.Plan = dto.Plan.Value;
                }
                if (!tenant.SubscriptionStartDate.HasValue)
                {
                    tenant.SubscriptionStartDate = DateTime.UtcNow;
                }
                await _context.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Admin updated tenant {TenantId}: Plan={Plan}, EndDate={EndDate}",
                    tenant.TenantId, tenant.Plan, tenant.SubscriptionEndDate);
            }
            else if (user.TenantId != Guid.Empty)
            {
                tenant = new Core.Entities.Tenants.Tenant
                {
                    TenantId = user.TenantId,
                    Email = user.Email ?? string.Empty,
                    Plan = dto.Plan ?? PlanType.Free,
                    SubscriptionStartDate = DateTime.UtcNow,
                    SubscriptionEndDate = dto.SubscriptionEndDate
                };
                _context.Tenants.Add(tenant);
                await _context.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Admin created tenant record for user {UserId}: TenantId={TenantId}, Plan={Plan}",
                    userId, tenant.TenantId, tenant.Plan);
            }

            return true;
        }

        public async Task<bool> EnableUserAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await UpdateUserStatusAsync(userId, new UpdateUserStatusDTO { IsActive = true }, cancellationToken);
        }

        public async Task<bool> DisableUserAsync(string userId, string? reason = null, CancellationToken cancellationToken = default)
        {
            return await UpdateUserStatusAsync(userId, new UpdateUserStatusDTO { IsActive = false }, cancellationToken);
        }

        public async Task<bool> UpdateSubscriptionAsync(string userId, PlanType plan, DateTime? startDate, DateTime? endDate, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.Users
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

            if (user == null)
            {
                _logger.LogWarning("Admin update subscription: User {UserId} not found.", userId);
                return false;
            }

            var tenant = await _context.Tenants
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(t => t.TenantId == user.TenantId, cancellationToken);

            if (tenant != null)
            {
                tenant.Plan = plan;
                if (startDate.HasValue) tenant.SubscriptionStartDate = startDate.Value;
                if (endDate.HasValue) tenant.SubscriptionEndDate = endDate.Value;

                await _context.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Admin updated subscription for Tenant {TenantId}: Plan={Plan}, Start={Start}, End={End}",
                    tenant.TenantId, plan, startDate, endDate);
            }
            else if (user.TenantId != Guid.Empty)
            {
                tenant = new Core.Entities.Tenants.Tenant
                {
                    TenantId = user.TenantId,
                    Email = user.Email ?? string.Empty,
                    Plan = plan,
                    SubscriptionStartDate = startDate ?? DateTime.UtcNow,
                    SubscriptionEndDate = endDate
                };
                _context.Tenants.Add(tenant);
                await _context.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Admin created subscription for Tenant {TenantId}: Plan={Plan}, Start={Start}, End={End}",
                    tenant.TenantId, plan, startDate, endDate);
            }

            return true;
        }
    }
}
