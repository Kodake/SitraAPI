using AutoMapper;
using QuestPDF.Fluent;
using SITRA.Core.Documents;
using SITRA.Core.Entities.Auths;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces;
using SITRA.Core.Models;
using SITRA.Core.Models.Dgii;

namespace SITRA.Core.Mediator.Queries.Invoices.SaleInvoices
{
    public class GetSaleInvoicePdfQuery 
        : GetByIdQuery<(byte[] FileBytes, string FileName)?>
    {
        private readonly DgiiSettings _dgiiSettings;
        private readonly SITRA.Core.Interfaces.Services.IDgiiStorageService _dgiiStorageService;

        public GetSaleInvoicePdfQuery(Guid invoiceId, DgiiSettings dgiiSettings, SITRA.Core.Interfaces.Services.IDgiiStorageService dgiiStorageService) : base(invoiceId) 
        {
            _dgiiSettings = dgiiSettings;
            _dgiiStorageService = dgiiStorageService;
        }

        public override async Task<(byte[] FileBytes, string FileName)?> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var saleInvoice =
                await unitOfWork.SaleInvoiceRepository.GetByIdAsync(Id);

            if (saleInvoice == null)
                return null;

            CompanyMetadata? company = null;

            try
            {
                var ownerCandidates =
                    await unitOfWork.Repository<User>()
                        .GetReadOnlyAsync(
                            u => u.TenantId == saleInvoice.TenantId && u.IsOwner,
                            cancellationToken);

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
                        ResolvedProvince =
                            LocationMappingHelper.GetProvinceName(owner.Province),
                        ResolvedMunicipality =
                            LocationMappingHelper.GetMunicipalityName(owner.Municipality)
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            DgiiPdfMetadata? dgiiMeta = null;
            if (!string.IsNullOrEmpty(saleInvoice.XmlUrl))
            {
                string? xmlContent = await _dgiiStorageService.ReadDocumentXmlFromUrlAsync(saleInvoice.XmlUrl);
                if (!string.IsNullOrEmpty(xmlContent))
                {
                    dgiiMeta = DgiiXmlMetadataReader.ParseXml(xmlContent);
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
                        var baseMeta = DgiiXmlMetadataReader.ParseXml(baseXmlContent);
                        if (baseMeta != null && !string.IsNullOrEmpty(baseMeta.FechaFirma))
                        {
                            dgiiMeta.FechaFirma = baseMeta.FechaFirma;
                        }
                    }
                }
            }
            
            if (dgiiMeta == null && !string.IsNullOrEmpty(saleInvoice.FiscalSupportNumber))
            {
                string effectiveRnc =
                    company?.RucNumber ?? _dgiiSettings.DefaultRNC;

                dgiiMeta = DgiiXmlMetadataReader.TryExtractMetadata(
                    _dgiiSettings.StoragePath,
                    effectiveRnc,
                    saleInvoice.FiscalSupportNumber
                );
            }

            var invoiceDocument =
                new InvoiceDocument(
                    saleInvoice,
                    dgiiMeta,
                    company,
                    _dgiiSettings);

            var finalRnc =
                company?.RucNumber
                ?? dgiiMeta?.EmisorRnc
                ?? _dgiiSettings.DefaultRNC;

            bool isRfce = dgiiMeta?.IsRfce == true;

            if (!isRfce &&
                !string.IsNullOrEmpty(saleInvoice.FiscalSupportNumber) &&
                saleInvoice.FiscalSupportNumber.StartsWith("E32",
                    StringComparison.OrdinalIgnoreCase) &&
                saleInvoice.TotalAmount < 250000)
            {
                isRfce = true;
            }

            var suffix = isRfce ? "_RFCE" : "";

            // Sanitize filename
            var invalidChars = Path.GetInvalidFileNameChars();

            var sanitizedNcf = new string(
                (saleInvoice.FiscalSupportNumber ?? Id.ToString())
                .Select(ch => invalidChars.Contains(ch) ? '-' : ch)
                .ToArray());

            var fileName =
                $"{finalRnc}{sanitizedNcf}{suffix}.pdf";

            using var stream = new MemoryStream();

            invoiceDocument.GeneratePdf(stream);

            var fileBytes = stream.ToArray();

            return (fileBytes, fileName);
        }
    }
}