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
                    ?? throw new Exception($"Invoice with id {request.InvoiceId} not found.");

                var invoiceDocument = new QuotationDocumentDTO(saleInvoice);
                var filePath = Path.Combine(Path.GetTempPath(), $"Quotation_{request.InvoiceId}.pdf");
                invoiceDocument.GeneratePdf(filePath);

                var fileBytes = await File.ReadAllBytesAsync(filePath);
                return (fileBytes, $"Quotation_{request.InvoiceId}.pdf");
            }
        }
    }
}
