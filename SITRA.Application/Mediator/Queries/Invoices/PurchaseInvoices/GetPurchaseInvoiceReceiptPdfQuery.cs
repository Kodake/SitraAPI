using AutoMapper;
using SITRA.Core.Documents;
using SITRA.Core.Interfaces;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SITRA.Core.Mediator.Queries.Invoices.PurchaseInvoices
{
    public class GetPurchaseInvoiceReceiptPdfQuery : GetByIdQuery<(byte[] FileBytes, string FileName)?>
    {
        private readonly SITRA.Core.Models.Dgii.DgiiSettings _dgiiSettings;
        private readonly SITRA.Core.Interfaces.Services.Storage.IDgiiStorageService _dgiiStorageService;

        public GetPurchaseInvoiceReceiptPdfQuery(
            Guid invoiceId, 
            SITRA.Core.Models.Dgii.DgiiSettings dgiiSettings, 
            SITRA.Core.Interfaces.Services.Storage.IDgiiStorageService dgiiStorageService) : base(invoiceId) 
        { 
            _dgiiSettings = dgiiSettings;
            _dgiiStorageService = dgiiStorageService;
        }

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

            var user = await unitOfWork.UserService.GetUserAsync();

            if (user == null)
            {
                return null;
            }

            SITRA.Core.Models.Dgii.DgiiPdfMetadata? dgiiMeta = null;
            if (!string.IsNullOrEmpty(purchaseInvoice.XmlUrl))
            {
                string? xmlContent = await _dgiiStorageService.ReadDocumentXmlFromUrlAsync(purchaseInvoice.XmlUrl);
                if (!string.IsNullOrEmpty(xmlContent))
                {
                    dgiiMeta = SITRA.Core.Helpers.DgiiXmlMetadataReader.ParseXml(xmlContent);
                }

                // Evaluate if 31 or 41 is the applied case for the query class
                if (dgiiMeta != null && string.IsNullOrEmpty(dgiiMeta.FechaFirma) && purchaseInvoice.XmlUrl.Contains("_RFCE"))
                {
                    string? fiscalNumber = purchaseInvoice.FiscalSupportNumber;
                    string docTypeFolder = "31"; // Default to 31
                    if (fiscalNumber?.Length >= 3)
                    {
                        var derived = fiscalNumber.Substring(1, 2);
                        if (derived == "31" || derived == "41")
                        {
                            docTypeFolder = derived;
                        }
                    }

                    string baseEcfUrl = purchaseInvoice.XmlUrl
                        .Replace("/RFCE/", $"/{docTypeFolder}/")
                        .Replace("_RFCE.xml", ".xml");

                    string? baseXmlContent = await _dgiiStorageService.ReadDocumentXmlFromUrlAsync(baseEcfUrl);
                    if (!string.IsNullOrEmpty(baseXmlContent))
                    {
                        var baseMeta = SITRA.Core.Helpers.DgiiXmlMetadataReader.ParseXml(baseXmlContent);
                        if (baseMeta != null && !string.IsNullOrEmpty(baseMeta.FechaFirma))
                        {
                            dgiiMeta.FechaFirma = baseMeta.FechaFirma;
                        }
                    }
                }
            }

            var invoiceDocument = new InvoiceReceiptDocument(purchaseInvoice, user, dgiiMeta, _dgiiSettings);
            
            // Derive consistent user-friendly filenames
            var finalRnc = user.RucNumber ?? "RNC";
            var sanitizedNcf = purchaseInvoice.FiscalSupportNumber?.Replace("/", "-") ?? Id.ToString();
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
