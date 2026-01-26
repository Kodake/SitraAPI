using SITRA.Core.Entities.Auths;
using AutoMapper;
using SITRA.Core.DTOs;
using SITRA.Core.Entities.Invoices;
using SITRA.Core.Interfaces;
using SITRA.Core.Documents;

namespace SITRA.Core.Mediator.Queries.Invoices.SaleInvoices
{
    public class GetSaleInvoiceReceiptPdfQuery : GetByIdQuery<(byte[] FileBytes, string FileName)?>

    {
        public GetSaleInvoiceReceiptPdfQuery(int invoiceId) : base(invoiceId) { }

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
            var filePath = Path.Combine(Path.GetTempPath(), $"Invoice-Receipt_{Id}.pdf");
            invoiceDocument.GeneratePdf(filePath);

            var fileBytes = await File.ReadAllBytesAsync(filePath, cancellationToken);
            var file = (fileBytes, $"Invoice-Receipt_{Id}.pdf");
            return file;
        }
    }
}

