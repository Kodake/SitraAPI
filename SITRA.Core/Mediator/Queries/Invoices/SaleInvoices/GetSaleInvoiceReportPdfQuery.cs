using AutoMapper;
using SITRA.Core.Documents;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.Invoices.SaleInvoices
{
    public class GetSaleInvoiceReportPdfQuery : IQuery<(byte[] FileBytes, string FileName)?>
    {
        public DateTime From { get; }
        public DateTime To { get; }
        public GetSaleInvoiceReportPdfQuery(DateTime from, DateTime to)
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

            var saleInvoices = await unitOfWork.SaleInvoiceRepository.GetSalesReportByDateRangeAsync(From, To);

            if (saleInvoices == null)
            {
                return null;
            }

            var fileName = $"Reporte-Ventas-{From:dd-MM-yyyy}_al_{To:dd-MM-yyyy}.pdf";
            var filePath = Path.Combine(Path.GetTempPath(), fileName);
            
            var invoiceDocument = new SaleInvoiceReportDocument(saleInvoices, From, To);
            invoiceDocument.GeneratePdf(filePath);

            var fileBytes = await File.ReadAllBytesAsync(filePath);

            // Final cleanup trigger
            if (File.Exists(filePath)) File.Delete(filePath);

            var file = (fileBytes, fileName);
            return file;
        }
    }
}
