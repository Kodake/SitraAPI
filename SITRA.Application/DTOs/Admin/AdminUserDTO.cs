using SITRA.Core.Enums;

namespace SITRA.Core.DTOs.Admin
{
    public class AdminUserDTO
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string? CompanyName { get; set; }
        public string? RucNumber { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Province { get; set; }
        public string? Municipality { get; set; }
        public Guid TenantId { get; set; }
        public PlanType Plan { get; set; } = PlanType.Free;
        public string PlanName => Plan.ToString();
        public DateTime? SubscriptionStartDate { get; set; }
        public DateTime? SubscriptionEndDate { get; set; }
        public int? DaysRemaining { get; set; }
        public string SubscriptionStatus { get; set; } = "Active"; // Active, ExpiringSoon, Expired, SuperAdmin
        public bool IsActive { get; set; } = true;
        public DateTimeOffset? LockoutEnd { get; set; }
        public bool IsOwner { get; set; }
        public bool IsSuperAdmin { get; set; }
    }

    public class UpdateUserStatusDTO
    {
        public bool IsActive { get; set; }
        public DateTime? SubscriptionEndDate { get; set; }
        public PlanType? Plan { get; set; }
    }

    public class UpdateSubscriptionDTO
    {
        public PlanType Plan { get; set; } = PlanType.Free;
        public DateTime? SubscriptionStartDate { get; set; }
        public DateTime? SubscriptionEndDate { get; set; }
    }

    public class DisableUserDTO
    {
        public string? Reason { get; set; }
    }
}

