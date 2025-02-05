using AutoMapper;
using BackEnd.DTOs;
using BackEnd.Repositories;
using MediatR;

namespace BackEnd.MediatR.Queries.Invoices.PurchaseInvoices
{
    public class GetPurchaseInvoicePdfQuery : IRequest<(byte[] FileBytes, string FileName)>
    {
        public int InvoiceId { get; }

        public GetPurchaseInvoicePdfQuery(int invoiceId)
        {
            InvoiceId = invoiceId;
        }

        public class Handler : IRequestHandler<GetPurchaseInvoicePdfQuery, (byte[] FileBytes, string FileName)>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IMapper _mapper;

            public Handler(IUnitOfWork unitOfWork, IMapper mapper)
            {
                _unitOfWork = unitOfWork;
                _mapper = mapper;
            }

            public async Task<(byte[] FileBytes, string FileName)> Handle(GetPurchaseInvoicePdfQuery request, CancellationToken cancellationToken)
            {
                QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
                var purchaseInvoice = await _unitOfWork.PurchaseInvoiceRepository.GetByIdAsync(request.InvoiceId)
                    ?? throw new Exception($"Invoice with id {request.InvoiceId} not found.");

                var invoiceDocument = new InvoiceDocumentDTO(purchaseInvoice);
                var filePath = Path.Combine(Path.GetTempPath(), $"Invoice_{request.InvoiceId}.pdf");
                invoiceDocument.GeneratePdf(filePath);

                var fileBytes = await File.ReadAllBytesAsync(filePath);
                return (fileBytes, $"Invoice_{request.InvoiceId}.pdf");
            }
        }
    }
}