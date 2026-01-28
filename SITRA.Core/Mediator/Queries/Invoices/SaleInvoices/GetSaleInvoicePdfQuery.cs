using AutoMapper;
using SITRA.Core.DTOs;
using SITRA.Core.Interfaces;
using SITRA.Core.Documents;
using SITRA.Core.Mediator.Queries;
using System.Threading;
using System.Threading.Tasks;

namespace SITRA.Core.Mediator.Queries.Invoices.SaleInvoices
{
    public class GetSaleInvoicePdfQuery : GetByIdQuery<(byte[] FileBytes, string FileName)?>
    {
        public GetSaleInvoicePdfQuery(Guid invoiceId) : base(invoiceId) { }

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

            var invoiceDocument = new InvoiceDocument(saleInvoice);
            var filePath = Path.Combine(Path.GetTempPath(), $"Sale-Invoice_{Id}.pdf");
            invoiceDocument.GeneratePdf(filePath);

            var fileBytes = await File.ReadAllBytesAsync(filePath, cancellationToken);
            var file = (fileBytes, $"Sale-Invoice_{Id}.pdf");
            return file;
        }
    }
}
