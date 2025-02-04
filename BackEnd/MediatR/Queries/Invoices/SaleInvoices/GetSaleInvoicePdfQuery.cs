using AutoMapper;
using BackEnd.DTOs;
using BackEnd.Repositories;
using MediatR;

namespace BackEnd.MediatR.Queries.Invoices.SaleInvoices
{
    public class GetSaleInvoicePdfQuery : IRequest<(byte[] FileBytes, string FileName)>
    {
        public int InvoiceId { get; }

        public GetSaleInvoicePdfQuery(int invoiceId)
        {
            InvoiceId = invoiceId;
        }

        public class Handler : IRequestHandler<GetSaleInvoicePdfQuery, (byte[] FileBytes, string FileName)>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IMapper _mapper;

            public Handler(IUnitOfWork unitOfWork, IMapper mapper)
            {
                _unitOfWork = unitOfWork;
                _mapper = mapper;
            }

            public async Task<(byte[] FileBytes, string FileName)> Handle(GetSaleInvoicePdfQuery request, CancellationToken cancellationToken)
            {
                QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
                var saleInvoice = await _unitOfWork.SaleInvoiceRepository.GetByIdAsync(request.InvoiceId)
                    ?? throw new Exception($"Invoice with id {request.InvoiceId} not found.");

                var invoiceDocument = new InvoiceDocumentDTO(saleInvoice);
                var filePath = Path.Combine(Path.GetTempPath(), $"Invoice_{request.InvoiceId}.pdf");
                invoiceDocument.GeneratePdf(filePath);

                var fileBytes = await File.ReadAllBytesAsync(filePath);
                return (fileBytes, $"Invoice_{request.InvoiceId}.pdf");
            }
        }
    }
}
