using AutoMapper;
using SITRA.Core.DTOs;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.Invoices.SaleInvoices
{
    public class GetSaleInvoicePdfQuery : GetByIdQuery<(byte[] FileBytes, string FileName)?>

    {
        public GetSaleInvoicePdfQuery(int invoiceId) : base(invoiceId) { }

        public override async Task<(byte[] FileBytes, string FileName)?> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
            var purchaseInvoice = await unitOfWork.PurchaseInvoiceRepository.GetByIdAsync(Id);

            if (purchaseInvoice == null)
            {
                return null;
            }

            var invoiceDocument = new InvoiceDocumentDTO(purchaseInvoice);
            var filePath = Path.Combine(Path.GetTempPath(), $"Sale-Invoice_{Id}.pdf");
            invoiceDocument.GeneratePdf(filePath);

            var fileBytes = await File.ReadAllBytesAsync(filePath, cancellationToken);
            var file = (fileBytes, $"Sale-Invoice_{Id}.pdf");
            return file;
        }
    }
}
