using AutoMapper;
using SITRA.Core.Documents;
using SITRA.Core.Interfaces;

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

            var reportDocument = new StockReportDocument(stocks, WarehouseIds);

            var filePath = Path.Combine(Path.GetTempPath(), $"Stocks-Report_{Guid.NewGuid()}.pdf");
            reportDocument.GeneratePdf(filePath);

            var fileBytes = await File.ReadAllBytesAsync(filePath, cancellationToken);
            return (fileBytes, "Stocks-Report.pdf");
        }
    }
}
