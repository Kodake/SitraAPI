using AutoMapper;
using SITRA.Core.Interfaces;
using SITRA.Core.Documents;

namespace SITRA.Core.Mediator.Queries.Invoices.PurchaseInvoices
{

    public class GetPurchaseInvoiceReportPdfQuery : IQuery<(byte[] FileBytes, string FileName)?>
    {
        public DateTime From { get; }
        public DateTime To { get; }
        public GetPurchaseInvoiceReportPdfQuery(DateTime from, DateTime to)
        {
            To = to;
            From = from;
        }

        public async Task<(byte[] FileBytes, string FileName)?> GetData(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        CancellationToken cancellationToken)
        {
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

            var saleInvoices = await unitOfWork.PurchaseInvoiceRepository.GetPurchasesReportByDateRangeAsync(From, To);

            if (saleInvoices == null)
            {
                return null;
            }

            var invoiceDocument = new PurchaseInvoiceReportDocument(saleInvoices, From, To);
            var filePath = Path.Combine(Path.GetTempPath(), $"Purchases-Report_{Guid.NewGuid()}.pdf");
            invoiceDocument.GeneratePdf(filePath);

            var fileBytes = await File.ReadAllBytesAsync(filePath);
            var file = (fileBytes, $"Purchases-Report-{From:dd-MM-yyyy}_to_{To:dd-MM-yyyy}.pdf");

            return file;
        }
    }
}
