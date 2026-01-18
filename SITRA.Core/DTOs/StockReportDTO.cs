using SITRA.Core.Entities.Warehouses;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
namespace SITRA.Core.DTOs
{
    public class StockReportDTO : IDocument
    {
        public StockReportDTO(List<Stock> stocks, List<long> warehouseIds, string reportName)
        {
            Stocks = stocks;
            WarehouseIds = warehouseIds;
            ReportName = reportName;
        }

        public List<Stock> Stocks { get; }
        public List<long> WarehouseIds { get; }
        public string ReportName { get; }

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Margin(30);
                page.DefaultTextStyle(x => x.FontSize(8));
                page.Header().Element(BuildHeader);
                page.Content().Element(BuildContent);
                page.Footer().Element(BuildFooter);
            });
        }

        private void BuildHeader(IContainer container)
        {
            container.Column(column =>
            {
                var logoPath = Path.Combine(AppContext.BaseDirectory, "Resources", "logo-black.png");

                try
                {
                    if (File.Exists(logoPath))
                    {
                        column.Item()
                              .AlignCenter()
                              .Image(logoPath)
                              .FitWidth();
                    }
                    else
                    {
                        column.Item()
                              .AlignCenter()
                              .Text("LOGO NO DISPONIBLE")
                              .FontSize(8)
                              .Italic();
                    }
                }
                catch (Exception ex)
                {
                    column.Item()
                          .AlignCenter()
                          .Text("ERROR AL CARGAR LOGO")
                          .FontSize(8)
                          .Italic();

                    Console.WriteLine($"[StockReportDTO] Error loading logo: {ex.Message}");
                }

                column.Item().PaddingTop(5).BorderBottom(1).PaddingBottom(5).Column(info =>
                {
                    info.Item().AlignCenter().Text(ReportName).FontSize(10).SemiBold();
                    info.Item().AlignCenter().Text($"{DateTimeOffset.UtcNow:dd-MM-yyyy}");
                });
            });
        }

        private void BuildContent(IContainer container)
        {
            container.PaddingVertical(15).Column(column =>
            {
                column.Spacing(5);
                column.Item().Element(BuildTable);

                // Totals
                column.Item().PaddingTop(10).AlignRight()
                    .Text($"TOTAL: {Stocks.Sum(x => x.Qty):N0}");
            });
        }

        private void BuildTable(IContainer container)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(column =>
                {
                    column.RelativeColumn(1); // NO.
                    column.RelativeColumn(5); // PRODUCTO
                    column.RelativeColumn(5); // ALMAC�N
                    column.RelativeColumn(3); // CANTIDAD
                });

                table.Header(header =>
                {
                    header.Cell().Element(CellStyleHeader).Text("NO.");
                    header.Cell().Element(CellStyleHeader).Text("PRODUCTO");
                    header.Cell().Element(CellStyleHeader).Text("ALMAC�N");
                    header.Cell().Element(CellStyleHeader).AlignRight().Text("CANTIDAD");

                    static IContainer CellStyleHeader(IContainer container)
                        => container.DefaultTextStyle(x => x.SemiBold())
                                    .PaddingVertical(1)
                                    .BorderBottom(1)
                                    .BorderColor(Colors.Black);
                });

                int index = 1;
                foreach (var item in Stocks)
                {
                    table.Cell().Element(CellStyleRow).Text(index.ToString());
                    table.Cell().Element(CellStyleRow).Text(item.Item.Name ?? "N/A");
                    table.Cell().Element(CellStyleRow).Text(item.Warehouse?.Name ?? "N/A");
                    table.Cell().Element(CellStyleRow).AlignRight().Text($"{item.Qty:N0}");

                    index++;

                    static IContainer CellStyleRow(IContainer container)
                        => container.BorderBottom(1)
                                    .BorderColor(Colors.Grey.Lighten2)
                                    .PaddingVertical(1);
                }
            });
        }

        private void BuildFooter(IContainer container)
        {
            container.AlignCenter().Text(x =>
            {
                x.CurrentPageNumber();
                x.Span(" / ");
                x.TotalPages();
            });
        }

        public void GeneratePdf(string filePath)
        {
            Document.Create(container => Compose(container)).GeneratePdf(filePath);
        }
    }
}
