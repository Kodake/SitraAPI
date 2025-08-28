using BackEnd.Entities.Invoices;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Globalization;
using System.Reflection;

namespace BackEnd.DTOs
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
            container.Page(
                page =>
                {
                    page.Margin(30); // Márgenes reducidos para más espacio
                    page.DefaultTextStyle(x => x.FontSize(8)); // Fuente pequeña para todo el documento
                    page.Header().Element(BuildHeader);
                    page.Content().Element(BuildContent);
                    page.Footer().Element(BuildFooter);
                });
        }

        private void BuildHeader(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem().Border(1).Column(column =>
                {
                    var padding = 3; // Padding reducido
                    var assembly = Assembly.GetExecutingAssembly();
                    var resourcePath = "BackEnd.Resources.logo-black.png";

                    var stream = assembly.GetManifestResourceStream(resourcePath);

                    if (stream != null)
                    {
                        column.Item().Height(1.5f, Unit.Centimetre).Image(stream); // Logo un poco más pequeño
                    }
                    column.Item().BorderBottom(1).Padding(padding).Row(row =>
                    {
                        row.RelativeItem().Text(ReportName).SemiBold();
                        row.ConstantItem(100).AlignRight().Text($"{DateTime.Today:dd-MM-yyyy}");
                    });
                    column.Item().Padding(padding).Text($"DESDE: {From:dd-MM-yyyy}");
                    column.Item().Padding(padding).Text($"HASTA: {To:dd-MM-yyyy}");
                });
            });
        }

        private void BuildContent(IContainer container)
        {
            container.PaddingVertical(15).Column(column => // Padding vertical reducido
            {
                column.Spacing(5);
                column.Item().Element(BuildTable);
                column.Item().Text("");
                column.Item().Text("");
                column.Item().AlignRight().Text($"SUBTOTAL: {Invoices.Sum(x => x.TotalAmount):N}");
                column.Item().AlignRight().Text($"IMPUESTOS TOTALES: {Invoices.Sum(x => x.TotalTaxes):N}");
                var subTaxes = Invoices.Sum(x => x.TotalAmount) + Invoices.Sum(x => x.TotalTaxes);
                column.Item().AlignRight().Text($"SUB + IMPUESTOS: {subTaxes:N}");
                column.Item().AlignRight().Text($"TOTAL: {(Invoices.Sum(x => x.TotalPrice)):N}");
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
                    {
                        return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(1).BorderBottom(1).BorderColor(Colors.Black);
                    }
                });

                foreach (var item in Invoices)
                {
                    table.Cell().Element(CellStyle).Text(item.Id.ToString());
                    table.Cell().Element(CellStyle).Text(item.CreatedAt.ToString("dd/MM/yyyy", CultureInfo.GetCultureInfo("es-DO")));
                    table.Cell().Element(CellStyle).Text(string.IsNullOrEmpty(item.FiscalSupportNumber) ? "N/A" : item.FiscalSupportNumber);
                    table.Cell().Element(CellStyle).AlignRight().Text($"{item.TotalAmount:N}");
                    table.Cell().Element(CellStyle).AlignRight().Text($"{item.TotalTaxes:N}");
                    table.Cell().Element(CellStyle).AlignRight().Text($"{item.TotalPrice:N}");

                    static IContainer CellStyle(IContainer container)
                    {
                        return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(1);
                    }
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
            Document.Create(container => Compose(container))
                    .GeneratePdf(filePath);
        }
    }
}