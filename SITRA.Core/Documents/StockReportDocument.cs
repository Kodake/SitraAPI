using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using SITRA.Core.Documents.Components;
using SITRA.Core.Entities.Warehouses;

namespace SITRA.Core.Documents
{
    public class StockReportDocument : ThermalDocumentBase
    {
        public List<Stock> Stocks { get; }
        public List<Guid> WarehouseIds { get; }

        public StockReportDocument(List<Stock> stocks, List<Guid> warehouseIds)
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
                column.Spacing(2);

                column.Item().Text($"{DateTimeOffset.UtcNow:dd-MM-yyyy}");

                column.Item().PaddingVertical(5).LineHorizontal(0.5f);

                column.Item().Row(row =>
                {
                    row.RelativeItem().Text(SITRA.Core.Helpers.Utilities.ProductLabel).SemiBold();
                    row.RelativeItem().AlignRight().Text(SITRA.Core.Helpers.Utilities.QuantityLabel).SemiBold();
                });

                column.Item().PaddingVertical(5).LineHorizontal(0.5f);

                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(column =>
                    {
                        column.RelativeColumn(); // PROD
                        column.RelativeColumn(); // CANT
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
