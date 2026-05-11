using MediatR;
using SITRA.Core.DTOs.Invoices.SaleInvoices;

namespace SITRA.Core.Mediator.Commands.Invoices.SaleInvoices
{
    /// <summary>
    /// MediatR command to annul an existing NCF by issuing an E34 Credit Note with zero amounts.
    /// Unlike CreateNoteCommand, this command does NOT go through the full invoice creation flow.
    /// It builds a minimal Invoice shell strictly for the ECF XML pipeline.
    /// </summary>
    public class AnnulNcfCommand(AnnulNcfDTO dto) : IRequest<AnnulNcfDTO>
    {
        public AnnulNcfDTO Dto { get; } = dto;
    }
}
