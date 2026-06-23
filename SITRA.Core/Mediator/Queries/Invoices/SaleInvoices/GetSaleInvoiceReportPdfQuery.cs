using AutoMapper;
using Mediator;
using QuestPDF.Fluent;
using SITRA.Core.Documents;
using SITRA.Core.Entities.Auths;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces;
using SITRA.Core.Interfaces.Services.Invoices;
using SITRA.Core.Models;

namespace SITRA.Core.Mediator.Queries.Invoices.SaleInvoices
{
    public class GetSaleInvoiceReportPdfQuery : IRequest<(byte[] FileBytes, string FileName)?>
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public GetSaleInvoiceReportPdfQuery(DateTime from, DateTime to)
        {
            To = to;
            From = from;
        }
    }

    public class GetSaleInvoiceReportPdfQueryHandler : IRequestHandler<GetSaleInvoiceReportPdfQuery, (byte[] FileBytes, string FileName)?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISaleInvoiceService _saleInvoiceService;

        public GetSaleInvoiceReportPdfQueryHandler(IUnitOfWork unitOfWork, ISaleInvoiceService saleInvoiceService)
        {
            _unitOfWork = unitOfWork;
            _saleInvoiceService = saleInvoiceService;
        }

        public async ValueTask<(byte[] FileBytes, string FileName)?> Handle(GetSaleInvoiceReportPdfQuery request, CancellationToken cancellationToken)
        {
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

            var saleInvoices = await _saleInvoiceService.GetSalesReportByDateRangeAsync(request.From, request.To, cancellationToken);

            if (saleInvoices == null || !saleInvoices.Any())
            {
                return null;
            }

            CompanyMetadata? company = null;

            try
            {
                var firstInvoice = saleInvoices.First();
                var ownerCandidates = await _unitOfWork.Repository<User>()
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

            var fileName = $"Sale_{request.From:yyyy-MM-dd}_{request.To:yyyy-MM-dd}.pdf";
            
            var invoiceDocument = new SaleInvoiceReportDocument(saleInvoices, request.From, request.To, company);
            
            using var stream = new MemoryStream();
            invoiceDocument.GeneratePdf(stream);
            var fileBytes = stream.ToArray();

            var file = (fileBytes, fileName);
            return file;
        }
    }
}
