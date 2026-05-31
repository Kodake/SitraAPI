using AutoMapper;
using QuestPDF.Fluent;
using SITRA.Core.Documents;
using SITRA.Core.Entities.Auths;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces;
using SITRA.Core.Models;

namespace SITRA.Core.Mediator.Queries.Invoices.PurchaseInvoices
{

    public class GetPurchaseInvoiceReportPdfQuery : IQuery<(byte[] FileBytes, string FileName)?>
    {
        public DateTime From { get; }
        public DateTime To { get; }
        public GetPurchaseInvoiceReportPdfQuery(DateTime from, DateTime to)
        {
            To = to;
            From = from;
        }

        public async Task<(byte[] FileBytes, string FileName)?> GetData(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        CancellationToken cancellationToken)
        {
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

            var saleInvoices = await unitOfWork.PurchaseInvoiceRepository.GetPurchasesReportByDateRangeAsync(From, To);

            if (saleInvoices == null || !saleInvoices.Any())
            {
                return null;
            }

            CompanyMetadata? company = null;

            try
            {
                var firstInvoice = saleInvoices.First();
                var ownerCandidates = await unitOfWork.Repository<User>()
                    .GetReadOnlyAsync(
                        u => u.TenantId == firstInvoice.TenantId && u.IsOwner,
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
                        ResolvedProvince = LocationMappingHelper.GetProvinceName(owner.Province),
                        ResolvedMunicipality = LocationMappingHelper.GetMunicipalityName(owner.Municipality)
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            var fileName = $"Purchase_{From:yyyy-MM-dd}_{To:yyyy-MM-dd}.pdf";

            var invoiceDocument = new PurchaseInvoiceReportDocument(saleInvoices, From, To, company);
            
            using var stream = new MemoryStream();
            invoiceDocument.GeneratePdf(stream);
            var fileBytes = stream.ToArray();

            var file = (fileBytes, fileName);
            return file;
        }
    }
}
