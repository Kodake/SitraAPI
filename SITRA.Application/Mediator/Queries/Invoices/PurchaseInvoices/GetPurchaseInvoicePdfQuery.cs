using AutoMapper;
using QuestPDF.Fluent;
using SITRA.Core.Documents;
using SITRA.Core.Entities.Auths;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces;
using SITRA.Core.Models;
using SITRA.Core.Models.Dgii;
using SITRA.Core.Interfaces.Services;
using SITRA.Core.Interfaces.Services.AdministrativeTools;
using SITRA.Core.Interfaces.Services.Auths;
using SITRA.Core.Interfaces.Services.CashBoxes;
using SITRA.Core.Interfaces.Services.Dashboards;
using SITRA.Core.Interfaces.Services.Finances;
using SITRA.Core.Interfaces.Services.Invoices;
using SITRA.Core.Interfaces.Services.Products;
using SITRA.Core.Interfaces.Services.Storage;
using SITRA.Core.Interfaces.Services.Messaging;

namespace SITRA.Core.Mediator.Queries.Invoices.PurchaseInvoices
{
    public class GetPurchaseInvoicePdfQuery : GetByIdQuery<(byte[] FileBytes, string FileName)?>
    {
        private readonly DgiiSettings _dgiiSettings;
        private readonly IDgiiStorageService _dgiiStorageService;

        public GetPurchaseInvoicePdfQuery(Guid invoiceId, DgiiSettings dgiiSettings, IDgiiStorageService dgiiStorageService) : base(invoiceId) 
        {
            _dgiiSettings = dgiiSettings;
            _dgiiStorageService = dgiiStorageService;
        }

        public override async Task<(byte[] FileBytes, string FileName)?> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var purchaseInvoice = await unitOfWork.PurchaseInvoiceRepository.GetByIdAsync(Id);

            if (purchaseInvoice == null)
            {
                return null;
            }

            // Resolve Company Metadata via Tenant Owner
            CompanyMetadata? company = null;
            try 
            {
                var ownerCandidates = await unitOfWork.Repository<User>().GetReadOnlyAsync(u => u.TenantId == purchaseInvoice.TenantId && u.IsOwner, cancellationToken);
                var owner = ownerCandidates.FirstOrDefault();
                if (owner != null)
                {
                    company = new CompanyMetadata
                    {
                        CompanyName = owner.CompanyName,
                        RucNumber = owner.RucNumber,
                        Address = owner.Address,
                        ProvinceCode = owner.Province,
                        MunicipalityCode = owner.Municipality,
                        PhoneNumber = owner.PhoneNumber,
                        Email = owner.Email,
                        ResolvedProvince = LocationMappingHelper.GetProvinceName(owner.Province),
                        ResolvedMunicipality = LocationMappingHelper.GetMunicipalityName(owner.Municipality),
                        CompanyLogoUrl = owner.CompanyLogoUrl
                    };
                }
            }
            catch { /* Fallback */ }

            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

            DgiiPdfMetadata? dgiiMeta = null;
            try
            {
                if (!string.IsNullOrEmpty(purchaseInvoice.XmlUrl))
                {
                    string? xmlContent = await _dgiiStorageService.ReadDocumentXmlFromUrlAsync(purchaseInvoice.XmlUrl);
                    if (!string.IsNullOrEmpty(xmlContent))
                    {
                        dgiiMeta = DgiiXmlMetadataReader.ParseXml(xmlContent);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GetPurchaseInvoicePdfQuery] XML extraction error: {ex}");
            }
            
            if (dgiiMeta == null && !string.IsNullOrEmpty(purchaseInvoice.FiscalSupportNumber))
            {
                string effectiveRnc = company?.RucNumber ?? _dgiiSettings.DefaultRNC;

                dgiiMeta = DgiiXmlMetadataReader.TryExtractMetadata(
                    _dgiiSettings.StoragePath,
                    effectiveRnc,
                    purchaseInvoice.FiscalSupportNumber
                );
            }

            var invoiceDocument = new InvoiceDocument(purchaseInvoice, dgiiMeta, company, _dgiiSettings);
            var finalRnc = company?.RucNumber ?? dgiiMeta?.EmisorRnc ?? _dgiiSettings.DefaultRNC;

            var invalidChars = Path.GetInvalidFileNameChars();
            var sanitizedNcf = new string(
                (purchaseInvoice.FiscalSupportNumber ?? Id.ToString())
                .Select(ch => invalidChars.Contains(ch) ? '-' : ch)
                .ToArray());

            var fileName = $"{finalRnc}{sanitizedNcf}.pdf";

            using var stream = new MemoryStream();

            invoiceDocument.GeneratePdf(stream);

            var fileBytes = stream.ToArray();

            var file = (fileBytes, fileName);
            return file;
        }
    }
}

