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

        public GetSaleInvoicePdfQuery(Guid invoiceId, DgiiSettings dgiiSettings)
            : base(invoiceId)
        {
            _dgiiSettings = dgiiSettings;
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
                        .GetAllAsync(
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

            if (!string.IsNullOrWhiteSpace(saleInvoice.FiscalSupportNumber))
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