using AutoMapper;
using SITRA.Core.Documents;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.Invoices.SaleInvoices
{
    public class GetSaleInvoiceReceiptPdfQuery : GetByIdQuery<(byte[] FileBytes, string FileName)?>

    {
        public GetSaleInvoiceReceiptPdfQuery(Guid invoiceId) : base(invoiceId) { }

        public override async Task<(byte[] FileBytes, string FileName)?> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
            var saleInvoice = await unitOfWork.SaleInvoiceRepository.GetByIdAsync(Id);

            if (saleInvoice == null)
            {
                return null;
            }

            var user = await unitOfWork.UserService.GetUserAsync();

            if (user == null)
            {
                return null;
            }

            var invoiceDocument = new InvoiceReceiptDocument(saleInvoice, user);
            
            // Derive consistent user-friendly filenames
            var finalRnc = user.RucNumber ?? "RNC";
            var sanitizedNcf = saleInvoice.FiscalSupportNumber?.Replace("/", "-") ?? Id.ToString();
            var fileName = $"Recibo-{finalRnc}{sanitizedNcf}.pdf";

            var filePath = Path.Combine(Path.GetTempPath(), fileName);
            invoiceDocument.GeneratePdf(filePath);

            var fileBytes = await File.ReadAllBytesAsync(filePath, cancellationToken);
            
            // Cleanup temporary filesystem pointer
            if (File.Exists(filePath)) File.Delete(filePath);

            var file = (fileBytes, fileName);
            return file;
        }
    }
}

