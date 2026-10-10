using AutoMapper;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Invoices.LogInvoices;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.Invoices.LogInvoices
{
    public class GetAllLogInvoicesQuery : GetAllQuery<LogInvoiceListDTO>
    {
        public DgiiStatus? Status { get; set; }

        public GetAllLogInvoicesQuery(PaginationDTO pagination, DgiiStatus? status = null) : base(pagination) 
        { 
            Status = status;
        }

        public override async Task<List<LogInvoiceListDTO>> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var statusString = Status?.ToDatabaseString();
            var logInvoices = await unitOfWork.LogInvoiceRepository.GetReadOnlyAsync(Pagination, statusString, cancellationToken);
            return mapper.Map<List<LogInvoiceListDTO>>(logInvoices);
        }
    }
}
