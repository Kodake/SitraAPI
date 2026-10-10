using AutoMapper;
using QuestPDF.Fluent;
using SITRA.Core.Documents;
using SITRA.Core.Entities.Auths;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces;
using SITRA.Core.Models;

namespace SITRA.Core.Mediator.Queries.Invoices.Quotations
{
    public class GetQuotationPdfQuery : GetByIdQuery<(byte[] FileBytes, string FileName)?>
    {
        public GetQuotationPdfQuery(Guid invoiceId) : base(invoiceId) { }

        public override async Task<(byte[] FileBytes, string FileName)?> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
            var quotation = await unitOfWork.SaleInvoiceRepository.GetByIdAsync(Id);

            if (quotation == null)
            {
                return null;
            }

            CompanyMetadata? company = null;

            try
            {
                var ownerCandidates =
                    await unitOfWork.Repository<User>()
                        .GetReadOnlyAsync(
                            u => u.TenantId == quotation.TenantId && u.IsOwner,
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
                            LocationMappingHelper.GetMunicipalityName(owner.Municipality),
                        CompanyLogoUrl = owner.CompanyLogoUrl
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            var invoiceDocument = new QuotationDocument(quotation, company);
            
            using var stream = new MemoryStream();
            invoiceDocument.GeneratePdf(stream);
            var fileBytes = stream.ToArray();
            
            // Sanitize filename
            var invalidChars = Path.GetInvalidFileNameChars();
            var sanitizedNcf = new string(
                (quotation.Quotation?.QuotationNumber ?? Id.ToString())
                .Select(ch => invalidChars.Contains(ch) ? '-' : ch)
                .ToArray());
                
            var finalRnc = company?.RucNumber ?? "Cotizacion";
            var fileName = $"{sanitizedNcf}.pdf";

            return (fileBytes, fileName);
        }
    }
}
