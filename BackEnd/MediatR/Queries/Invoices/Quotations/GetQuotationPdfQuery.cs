using AutoMapper;
using BackEnd.DTOs;
using BackEnd.Repositories;
using MediatR;

namespace BackEnd.MediatR.Queries.Invoices.Quotations
{
    public class GetQuotationPdfQuery : IRequest<(byte[] FileBytes, string FileName)>
    {
        public int InvoiceId { get; }

        public GetQuotationPdfQuery(int invoiceId)
        {
            InvoiceId = invoiceId;
        }

        public class Handler : IRequestHandler<GetQuotationPdfQuery, (byte[] FileBytes, string FileName)>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IMapper _mapper;

            public Handler(IUnitOfWork unitOfWork, IMapper mapper)
            {
                _unitOfWork = unitOfWork;
                _mapper = mapper;
            }

            public async Task<(byte[] FileBytes, string FileName)> Handle(GetQuotationPdfQuery request, CancellationToken cancellationToken)
            {
                QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
                var saleInvoice = await _unitOfWork.SaleInvoiceRepository.GetByIdAsync(request.InvoiceId)
                    ?? throw new Exception($"Quotation with number {request.InvoiceId} not found.");

                var quotationDocument = new QuotationDocumentDTO(saleInvoice);
                var quotationNumber = saleInvoice.Quotation?.QuotationNumber ?? "Unknown";

                var filePath = Path.Combine(Path.GetTempPath(), $"Quotation_{quotationNumber}.pdf");
                quotationDocument.GeneratePdf(filePath);

                var fileBytes = await File.ReadAllBytesAsync(filePath);
                return (fileBytes, $"Quotation_{quotationNumber}.pdf");
            }
        }
    }
}
