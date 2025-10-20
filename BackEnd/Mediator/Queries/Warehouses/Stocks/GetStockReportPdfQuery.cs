using AutoMapper;
using BackEnd.DTOs;
using BackEnd.Repositories;

namespace BackEnd.Mediator.Queries.Warehouses.Stocks
{
    public class GetStockReportPdfQuery : IQuery<(byte[] FileBytes, string FileName)?>
    {
        public List<long> WarehouseIds { get; }

        public GetStockReportPdfQuery(List<long> warehouseIds)
        {
            WarehouseIds = warehouseIds ?? throw new ArgumentNullException(nameof(warehouseIds));
        }

        public async Task<(byte[] FileBytes, string FileName)?> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

            var stocks = await unitOfWork.StockRepository.GetStockByWarehouseIdsAsync(WarehouseIds);

            if (stocks == null || !stocks.Any())
                return null;

            string reportName = "REPORTE DE INVENTARIO";
            var reportDocument = new StockReportDTO(stocks, WarehouseIds, reportName);

            var filePath = Path.Combine(Path.GetTempPath(), $"Stocks-Report_{Guid.NewGuid()}.pdf");
            reportDocument.GeneratePdf(filePath);

            var fileBytes = await File.ReadAllBytesAsync(filePath, cancellationToken);
            return (fileBytes, "Stocks-Report.pdf");
        }
    }
}
