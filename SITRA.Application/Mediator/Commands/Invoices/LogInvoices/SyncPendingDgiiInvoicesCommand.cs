using Mediator;
using SITRA.Core.DTOs.Invoices.LogInvoices;

namespace SITRA.Core.Mediator.Commands.Invoices.LogInvoices
{
    public record SyncPendingDgiiInvoicesCommand(Guid? TenantId = null) : IRequest<List<SyncDgiiStatusResultDTO>>;
}
