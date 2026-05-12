using AutoMapper;
using SITRA.Core.Documents;
using SITRA.Core.Entities.Auths;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces;
using SITRA.Core.Models;
using SITRA.Core.Models.Dgii;

namespace SITRA.Core.Mediator.Queries.Invoices.PurchaseInvoices
{
    public class GetPurchaseInvoicePdfQuery : GetByIdQuery<(byte[] FileBytes, string FileName)?>
    {
        private readonly DgiiSettings _dgiiSettings;

        public GetPurchaseInvoicePdfQuery(Guid invoiceId, DgiiSettings dgiiSettings) : base(invoiceId) 
        {
            _dgiiSettings = dgiiSettings;
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

            // Resolve Company Metadata via Tenant Owner
            CompanyMetadata? company = null;
            try 
            {
                var ownerCandidates = await unitOfWork.Repository<User>().GetAllAsync(u => u.TenantId == purchaseInvoice.TenantId && u.IsOwner, cancellationToken);
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
            catch { /* Fallback */ }

            DgiiPdfMetadata? dgiiMeta = null;
            if (!string.IsNullOrEmpty(purchaseInvoice.FiscalSupportNumber))
            {
                dgiiMeta = DgiiXmlMetadataReader.TryExtractMetadata(
                    _dgiiSettings.StoragePath,
                    _dgiiSettings.DefaultRNC,
                    purchaseInvoice.FiscalSupportNumber
                );
            }

            var invoiceDocument = new InvoiceDocument(purchaseInvoice, dgiiMeta, company);
            var filePath = Path.Combine(Path.GetTempPath(), $"Purchase-Invoice_{Id}.pdf");
            invoiceDocument.GeneratePdf(filePath);

            var fileBytes = await File.ReadAllBytesAsync(filePath, cancellationToken);
            var file = (fileBytes, $"Purchase-Invoice_{Id}.pdf");
            return file;
        }
    }
}

