using AutoMapper;
using SITRA.Core.Documents;
using SITRA.Core.Entities.Auths;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces;
using SITRA.Core.Models;
using SITRA.Core.Models.Dgii;

namespace SITRA.Core.Mediator.Queries.Invoices.SaleInvoices
{
    public class GetSaleInvoicePdfQuery : GetByIdQuery<(byte[] FileBytes, string FileName)?>
    {
        private readonly DgiiSettings _dgiiSettings;

        public GetSaleInvoicePdfQuery(Guid invoiceId, DgiiSettings dgiiSettings) : base(invoiceId) 
        {
            _dgiiSettings = dgiiSettings;
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

            // Resolve Company Metadata via Tenant Owner
            CompanyMetadata? company = null;
            try 
            {
                var ownerCandidates = await unitOfWork.Repository<User>().GetAllAsync(u => u.TenantId == saleInvoice.TenantId && u.IsOwner, cancellationToken);
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
                        ResolvedMunicipality = LocationMappingHelper.GetMunicipalityName(owner.Municipality)
                    };
                }
            }
            catch { /* Fallback if identity not configured in unit of work */ }

            DgiiPdfMetadata? dgiiMeta = null;
            if (!string.IsNullOrEmpty(saleInvoice.FiscalSupportNumber))
            {
                dgiiMeta = DgiiXmlMetadataReader.TryExtractMetadata(
                    _dgiiSettings.StoragePath,
                    _dgiiSettings.DefaultRNC,
                    saleInvoice.FiscalSupportNumber
                );
            }

            var invoiceDocument = new InvoiceDocument(saleInvoice, dgiiMeta, company);
            var filePath = Path.Combine(Path.GetTempPath(), $"Sale-Invoice_{Id}.pdf");
            invoiceDocument.GeneratePdf(filePath);

            var fileBytes = await File.ReadAllBytesAsync(filePath, cancellationToken);
            var file = (fileBytes, $"Sale-Invoice_{Id}.pdf");
            return file;
        }
    }
}

