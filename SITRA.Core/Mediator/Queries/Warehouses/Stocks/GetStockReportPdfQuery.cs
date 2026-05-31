using AutoMapper;
using QuestPDF.Fluent;
using SITRA.Core.Documents;
using SITRA.Core.Entities.Auths;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces;
using SITRA.Core.Models;

namespace SITRA.Core.Mediator.Queries.Warehouses.Stocks
{
    public class GetStockReportPdfQuery : IQuery<(byte[] FileBytes, string FileName)?>
    {
        public List<Guid> WarehouseIds { get; }

        public GetStockReportPdfQuery(List<Guid> warehouseIds)
        {
            WarehouseIds = warehouseIds ?? new List<Guid>();
        }

        public async Task<(byte[] FileBytes, string FileName)?> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            if (WarehouseIds == null || !WarehouseIds.Any())
                return null;

            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

            var stocks = await unitOfWork.StockRepository.GetStockByWarehouseIdsAsync(WarehouseIds);

            if (stocks == null || !stocks.Any())
                return null;

            CompanyMetadata? company = null;

            try
            {
                var firstStock = stocks.First();
                var ownerCandidates = await unitOfWork.Repository<User>()
                    .GetReadOnlyAsync(
                        u => u.TenantId == firstStock.TenantId && u.IsOwner,
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

            var fileName = $"Stock_{DateTime.UtcNow:yyyy-MM-dd}.pdf";
            var reportDocument = new StockReportDocument(stocks, WarehouseIds, company);

            using var stream = new MemoryStream();
            reportDocument.GeneratePdf(stream);
            var fileBytes = stream.ToArray();

            return (fileBytes, fileName);
        }
    }
}
