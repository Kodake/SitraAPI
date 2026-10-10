using Mediator;
using SITRA.Core.Enums;

namespace SITRA.Core.Mediator.Commands.Admin
{
    public record UpdateUserSubscriptionCommand(
        string UserId,
        PlanType Plan,
        DateTime? SubscriptionStartDate,
        DateTime? SubscriptionEndDate
    ) : IRequest<bool>;
}
