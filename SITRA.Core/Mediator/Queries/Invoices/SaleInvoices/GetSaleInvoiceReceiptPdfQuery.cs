using AutoMapper;
using SITRA.Core.Documents;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.Invoices.SaleInvoices
{
    public class GetSaleInvoiceReceiptPdfQuery : GetByIdQuery<(byte[] FileBytes, string FileName)?>

    {
        private readonly SITRA.Core.Models.Dgii.DgiiSettings _dgiiSettings;
        private readonly SITRA.Core.Interfaces.Services.Storage.IDgiiStorageService _dgiiStorageService;

        public GetSaleInvoiceReceiptPdfQuery(
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

            SITRA.Core.Models.Dgii.DgiiPdfMetadata? dgiiMeta = null;
            if (!string.IsNullOrEmpty(saleInvoice.XmlUrl))
            {
                string? xmlContent = await _dgiiStorageService.ReadDocumentXmlFromUrlAsync(saleInvoice.XmlUrl);
                if (!string.IsNullOrEmpty(xmlContent))
                {
                    dgiiMeta = SITRA.Core.Helpers.DgiiXmlMetadataReader.ParseXml(xmlContent);
                }

                // For Type 32 (RFCE) invoices, XmlUrl points to the RFCE XML which does NOT
                // contain FechaHoraFirma. The base ECF lives in a different folder (e.g. /32/ not /RFCE/).
                if (dgiiMeta != null && string.IsNullOrEmpty(dgiiMeta.FechaFirma) && saleInvoice.XmlUrl.Contains("_RFCE"))
                {
                    // Derive doc-type folder from the fiscal number: "E320000000508" → "32"
                    string? fiscalNumber = saleInvoice.FiscalSupportNumber;
                    string docTypeFolder = fiscalNumber?.Length >= 3 ? fiscalNumber.Substring(1, 2) : "32";

                    string baseEcfUrl = saleInvoice.XmlUrl
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

            var invoiceDocument = new InvoiceReceiptDocument(saleInvoice, user, dgiiMeta, _dgiiSettings);
            
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

