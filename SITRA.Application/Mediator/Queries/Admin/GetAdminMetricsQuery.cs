using Mediator;
using SITRA.Core.DTOs.Admin;

namespace SITRA.Core.Mediator.Queries.Admin
{
    public record GetAdminMetricsQuery(DateTime? FromDate, DateTime? ToDate) : IRequest<AdminMetricsDTO>;
}
