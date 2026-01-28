using AutoMapper;
using SITRA.Core.Interfaces;
using SITRA.Core.Documents;
using SITRA.Core.Mediator.Queries;
using System.Threading;
using System.Threading.Tasks;

namespace SITRA.Core.Mediator.Queries.Invoices.PurchaseInvoices
{
    public class GetPurchaseInvoicePdfQuery : GetByIdQuery<(byte[] FileBytes, string FileName)?>
    {
        public GetPurchaseInvoicePdfQuery(Guid invoiceId) : base(invoiceId) { }

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

            var invoiceDocument = new InvoiceDocument(purchaseInvoice);
            var filePath = Path.Combine(Path.GetTempPath(), $"Purchase-Invoice_{Id}.pdf");
            invoiceDocument.GeneratePdf(filePath);

            var fileBytes = await File.ReadAllBytesAsync(filePath);
            var file = (fileBytes, $"Purchase-Invoice_{Id}.pdf");
            return file;
        }
    }
}
