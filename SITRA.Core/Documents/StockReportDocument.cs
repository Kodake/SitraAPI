using SITRA.Core.Entities.Warehouses;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using SITRA.Core.Documents.Components;

namespace SITRA.Core.Documents
{
    public class StockReportDocument : ThermalDocumentBase
    {
        public List<Stock> Stocks { get; }
        public List<long> WarehouseIds { get; }

        public StockReportDocument(List<Stock> stocks, List<long> warehouseIds)
        {
            Stocks = stocks;
            WarehouseIds = warehouseIds;
        }

        protected override void BuildHeader(IContainer container)
        {
            container.Component(new ThermalHeaderComponent(null, SITRA.Core.Helpers.Utilities.StockReportTitle));
        }

        protected override void BuildContent(IContainer container)
        {
            container.Column(column =>
            {
                column.Item().Text($"{DateTimeOffset.UtcNow:dd-MM-yyyy}");
                column.Item().PaddingVertical(5);

                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(column =>
                    {
                        column.RelativeColumn(); // PROD
                        column.RelativeColumn(); // CANT
                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(ThermalStyles.CellStyleHeader).Text(SITRA.Core.Helpers.Utilities.ProductLabel);
                        header.Cell().Element(ThermalStyles.CellStyleHeader).AlignRight().Text(SITRA.Core.Helpers.Utilities.QuantityLabel);
                    });

                    foreach (var item in Stocks)
                    {
                        table.Cell().Element(ThermalStyles.CellStyleRow).Text(item.Item.Name ?? "N/A");
                        table.Cell().Element(ThermalStyles.CellStyleRow).AlignRight().Text($"{item.Qty:N0}");
                    }
                });

                column.Item().PaddingTop(10).AlignRight()
                    .Text($"{SITRA.Core.Helpers.Utilities.TotalLabel}: {Stocks.Sum(x => x.Qty):N0}").SemiBold();
            });
        }

        protected override void BuildFooter(IContainer container)
        {
            container.Component(new ThermalFooterComponent());
        }
    }
}
