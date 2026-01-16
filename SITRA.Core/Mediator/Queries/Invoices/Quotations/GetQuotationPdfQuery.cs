using AutoMapper;
using SITRA.Core.DTOs;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.Invoices.Quotations
{
    public class GetQuotationPdfQuery : GetByIdQuery<(byte[] FileBytes, string FileName)?>

    {
        public GetQuotationPdfQuery(int invoiceId) : base(invoiceId) { }

        public override async Task<(byte[] FileBytes, string FileName)?> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
            var quotation = await unitOfWork.SaleInvoiceRepository.GetByIdAsync(Id);

            if (quotation == null)
            {
                return null;
            }

            var invoiceDocument = new QuotationDocumentDTO(quotation);
            var filePath = Path.Combine(Path.GetTempPath(), $"Quotation_{Id}.pdf");
            invoiceDocument.GeneratePdf(filePath);

            var fileBytes = await File.ReadAllBytesAsync(filePath);
            var file = (fileBytes, $"Quotation_{Id}.pdf");
            return file;
        }
    }
}
