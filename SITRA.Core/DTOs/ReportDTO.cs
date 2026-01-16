using SITRA.Core.Entities.Invoices;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Globalization;

namespace SITRA.Core.DTOs
{
    public class ReportDTO : IDocument
    {
        public ReportDTO(List<Invoice> invoices, DateTime from, DateTime to, string reportName)
        {
            Invoices = invoices;
            From = from;
            To = to;
            ReportName = reportName;
        }

        public List<Invoice> Invoices { get; }
        public DateTime From { get; }
        public DateTime To { get; }
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

                    Console.WriteLine($"[ReportDTO] Error loading logo: {ex.Message}");
                }

                column.Item().PaddingTop(5).BorderBottom(1).PaddingBottom(5).Column(info =>
                {
                    info.Item().AlignCenter().Text(ReportName).FontSize(10).SemiBold();
                    info.Item().AlignCenter().Text($"{DateTime.Today:dd-MM-yyyy}");
                    info.Item().AlignCenter().Text($"DESDE: {From:dd-MM-yyyy}");
                    info.Item().AlignCenter().Text($"HASTA: {To:dd-MM-yyyy}");
                });
            });
        }

        private void BuildContent(IContainer container)
        {
            container.PaddingVertical(15).Column(column =>
            {
                column.Spacing(5);
                column.Item().Element(BuildTable);

                // Summary section
                column.Item().Text("");
                column.Item().Text("");
                column.Item().AlignRight().Text($"SUBTOTAL: {Invoices.Sum(x => x.TotalAmount):N}");
                column.Item().AlignRight().Text($"IMPUESTOS TOTALES: {Invoices.Sum(x => x.TotalTaxes):N}");

                var subTaxes = Invoices.Sum(x => x.TotalAmount) + Invoices.Sum(x => x.TotalTaxes);
                column.Item().AlignRight().Text($"SUB + IMPUESTOS: {subTaxes:N}");
                column.Item().AlignRight().Text($"TOTAL: {Invoices.Sum(x => x.TotalPrice):N}");
            });
        }

        private void BuildTable(IContainer container)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(column =>
                {
                    column.RelativeColumn(1); // NO.
                    column.RelativeColumn(2); // FECHA
                    column.RelativeColumn(4); // COMPROBANTE FISCAL
                    column.RelativeColumn(4); // MONTO
                    column.RelativeColumn(4); // IMPUESTO
                    column.RelativeColumn(4); // TOTAL
                });

                table.Header(header =>
                {
                    header.Cell().Element(CellStyle).Text("NO.");
                    header.Cell().Element(CellStyle).Text("FECHA");
                    header.Cell().Element(CellStyle).Text("COMPROBANTE FISCAL");
                    header.Cell().Element(CellStyle).AlignRight().Text("MONTO");
                    header.Cell().Element(CellStyle).AlignRight().Text("IMPUESTO");
                    header.Cell().Element(CellStyle).AlignRight().Text("TOTAL");

                    static IContainer CellStyle(IContainer container)
                        => container.DefaultTextStyle(x => x.SemiBold())
                                    .PaddingVertical(1)
                                    .BorderBottom(1)
                                    .BorderColor(Colors.Black);
                });

                // Data rows
                foreach (var item in Invoices)
                {
                    table.Cell().Element(CellStyle).Text(item.Id.ToString());
                    table.Cell().Element(CellStyle)
                        .Text(item.CreatedAt.ToString("dd/MM/yyyy", CultureInfo.GetCultureInfo("es-DO")));
                    table.Cell().Element(CellStyle)
                        .Text(string.IsNullOrEmpty(item.FiscalSupportNumber) ? "N/A" : item.FiscalSupportNumber);
                    table.Cell().Element(CellStyle).AlignRight().Text($"{item.TotalAmount:N}");
                    table.Cell().Element(CellStyle).AlignRight().Text($"{item.TotalTaxes:N}");
                    table.Cell().Element(CellStyle).AlignRight().Text($"{item.TotalPrice:N}");

                    static IContainer CellStyle(IContainer container)
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
