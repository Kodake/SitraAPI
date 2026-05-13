using AutoMapper;
using SITRA.Core.Documents;
using SITRA.Core.Interfaces;

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

            var fileName = $"Reporte-Compras-{From:dd-MM-yyyy}_al_{To:dd-MM-yyyy}.pdf";
            var filePath = Path.Combine(Path.GetTempPath(), fileName);

            var invoiceDocument = new PurchaseInvoiceReportDocument(saleInvoices, From, To);
            invoiceDocument.GeneratePdf(filePath);

            var fileBytes = await File.ReadAllBytesAsync(filePath);
            
            if (File.Exists(filePath)) File.Delete(filePath);

            var file = (fileBytes, fileName);
            return file;
        }
    }
}
