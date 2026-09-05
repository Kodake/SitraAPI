using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SITRA.Core.Entities.Warehouses;
using SITRA.Core.Models;

namespace SITRA.Core.Documents
{
    public class StockReportDocument : IDocument
    {
        private readonly List<Stock> _stocks;
        private readonly List<Guid> _warehouseIds;
        private readonly CompanyMetadata? _company;

        private const string AccentColor = "#306090"; // Deep Blue

        public StockReportDocument(List<Stock> stocks, List<Guid> warehouseIds, CompanyMetadata? company = null)
        {
            _stocks = stocks;
            _warehouseIds = warehouseIds;
            _company = company;
        }

        public DocumentMetadata GetMetadata()
        {
            return new DocumentMetadata
            {
                Title = $"Stock_{DateTime.UtcNow:yyyy-MM-dd}",
                Subject = "Reporte de Inventario",
                Author = _company?.CompanyName ?? "SITRA"
            };
        }

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1.25f, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(9).FontFamily(Fonts.SegoeUI).FontColor(Colors.Grey.Darken4));

                page.Header().Element(ComposeHeader);
                page.Content().Element(ComposeContent);

                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Página ");
                    x.CurrentPageNumber();
                });
            });
        }

        private void ComposeHeader(IContainer container)
        {
            container.PaddingBottom(15).Row(row =>
            {
                // Left Side: Logo and Company Details
                row.RelativeItem().Column(col =>
                {
                    byte[]? logoBytes = _company != null ? SITRA.Core.Helpers.Utilities.GetCompanyLogoBytes(_company.CompanyLogoUrl) : null;
                    if (logoBytes != null)
                    {
                        col.Item().Height(60).Image(logoBytes).FitArea();
                    }

                    string companyName = _company?.CompanyName ?? "MI EMPRESA";
                    string rncNumber = _company?.RucNumber ?? "N/A";

                    col.Item().PaddingTop(5).Text(companyName).FontSize(13).SemiBold().FontColor(Colors.Black);
                    col.Item().PaddingTop(1).Text($"RNC: {rncNumber}").FontSize(10).SemiBold().FontColor(Colors.Black);

                    col.Item().PaddingTop(3).Column(sub =>
                    {
                        string addr = _company?.Address;
                        if (!string.IsNullOrEmpty(addr))
                        {
                            sub.Item().Text(addr).FontSize(8).FontColor(Colors.Grey.Darken2);
                        }

                        string location = string.Join(", ", new[]
                        {
                            _company?.ResolvedMunicipality,
                            _company?.ResolvedProvince
                        }.Where(s => !string.IsNullOrEmpty(s)));

                        if (!string.IsNullOrEmpty(location))
                        {
                            sub.Item().Text(location).FontSize(8).FontColor(Colors.Grey.Darken2);
                        }

                        if (!string.IsNullOrEmpty(_company?.PhoneNumber))
                        {
                            sub.Item().Text($"Tel: {_company.PhoneNumber}").FontSize(8).FontColor(Colors.Grey.Darken2);
                        }

                        string email = _company?.Email;
                        if (!string.IsNullOrEmpty(email))
                        {
                            sub.Item().Text($"Email: {email}").FontSize(8).FontColor(Colors.Grey.Darken2);
                        }
                    });
                });

                // Right Side: Report Details Box
                row.ConstantItem(220).PaddingTop(5).Border(1f).BorderColor(AccentColor).Background(Colors.White).Column(col =>
                {
                    col.Item().Padding(10).AlignCenter().Column(sub =>
                    {
                        sub.Item().PaddingBottom(5).AlignCenter().Text("REPORTE DE INVENTARIO").SemiBold().FontSize(10).FontColor(AccentColor).AlignCenter();
                        sub.Item().PaddingTop(8).AlignCenter().Text(x =>
                        {
                            x.Span("Fecha: ").FontSize(8);
                            x.Span(DateTime.UtcNow.ToString("dd-MM-yyyy")).SemiBold().FontSize(8).FontColor(Colors.Black);
                        });
                    });
                });
            });
        }

        private void ComposeContent(IContainer container)
        {
            container.Column(column =>
            {
                // Items Table
                column.Item().PaddingTop(10).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(3); // ARTICULO
                        columns.RelativeColumn(1.5f); // ALMACEN
                        columns.RelativeColumn(1f); // CANTIDAD
                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(HeaderStyle).Text("ARTÍCULO");
                        header.Cell().Element(HeaderStyle).Text("ALMACÉN");
                        header.Cell().Element(HeaderStyle).AlignRight().Text("CANTIDAD");

                        static IContainer HeaderStyle(IContainer c) =>
                            c.BorderBottom(1.5f).BorderColor(AccentColor).PaddingVertical(5).DefaultTextStyle(x => x.SemiBold().FontSize(8).FontColor(AccentColor));
                    });

                    foreach (var item in _stocks.OrderBy(s => s.Item?.Name))
                    {
                        table.Cell().Element(RowStyle).Text(item.Item?.Name ?? "N/A");
                        table.Cell().Element(RowStyle).Text(item.Warehouse?.Name ?? "N/A");
                        table.Cell().Element(RowStyle).AlignRight().Text(item.Qty.ToString("N0")).SemiBold().FontColor(Colors.Black);
                    }

                    static IContainer RowStyle(IContainer c) =>
                        c.BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).PaddingVertical(6);
                });

                column.Item().PaddingTop(25);

                // BOTTOM SECTION: Totals (right)
                column.Item().Row(row =>
                {
                    row.RelativeItem().Column(col => { col.Item().Height(30); });

                    row.ConstantItem(200).Column(col =>
                    {
                        double totalQty = _stocks.Sum(x => x.Qty);

                        col.Item().PaddingTop(3).BorderBottom(1.5f).BorderColor(AccentColor).Row(r =>
                        {
                            r.RelativeItem().PaddingBottom(2).Text("TOTAL ARTÍCULOS").FontSize(9).SemiBold();
                            r.RelativeItem().AlignRight().PaddingBottom(2).Text(totalQty.ToString("N0")).FontSize(10).SemiBold().FontColor(Colors.Black);
                        });
                    });
                });
            });
        }
    }
}
