using BackEnd.Entities.Invoices;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace BackEnd.DTOs
{
    public class InvoiceDocumentDTO : IDocument
    {
        public InvoiceDocumentDTO(SaleInvoice saleInvoice)
        {
            SaleInvoice = saleInvoice;
        }

        public SaleInvoice SaleInvoice { get; }

        public void Compose(IDocumentContainer container)
        {
            container.Page(
                page =>
                {
                    page.Margin(50);

                    page.Header().Element(BuildHeader);
                    page.Content().Element(BuildContent);
                    page.Footer().Element(BuildFooter);
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

        private void BuildContent(IContainer container)
        {
            container.PaddingVertical(40).Column(column =>
            {
                column.Spacing(5);

                column.Item().Element(BuildTable);

                column.Item().AlignRight().Text($"TOTAL: {SaleInvoice.TotalAmount:N}");
            });
        }

        private void BuildTable(IContainer container)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(column =>
                {
                    column.RelativeColumn(2);
                    column.RelativeColumn();
                    column.RelativeColumn();
                    column.RelativeColumn();
                });

                table.Header(header =>
                {
                    header.Cell().Element(CellStyle).Text("DESCRIPCION");
                    header.Cell().Element(CellStyle).AlignRight().Text("CANTIDAD");
                    header.Cell().Element(CellStyle).AlignRight().Text("PRECIO");
                    header.Cell().Element(CellStyle).AlignRight().Text("ITBIS");
                    static IContainer CellStyle(IContainer container)
                    {
                        return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                    }
                });

                foreach (var item in SaleInvoice.SaleInvoiceItems)
                {
                    table.Cell().Element(CellStyle).Text(item.Item.Name);
                    table.Cell().Element(CellStyle).Text(item.Qty.ToString());
                    table.Cell().Element(CellStyle).Text($"{item.TotalPrice:N}");
                    table.Cell().Element(CellStyle).Text($"{item.Item.ItemTaxes.FirstOrDefault(x => x.ItemId == item.Id)?.Tax.Amount:N}");

                    static IContainer CellStyle(IContainer container)
                    {
                        return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                    }
                }
            });
        }

        private void BuildHeader(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    var fontWeight = 0.8f;
                    column.Item().Scale(fontWeight).Text($"IDENTIFICATION: {SaleInvoice.Buyer.Identification}" ?? "N/A");
                    column.Item().Scale(fontWeight).Text($"DATE: {SaleInvoice.Date.ToString("dd-MM-yyyy")}");
                    column.Item().Scale(fontWeight).Text($"NOMBRE o RAZON SOCIAL: {SaleInvoice.Buyer.Name}");
                });

                row.RelativeItem().Border(1).Column(column =>
                {
                    var padding = 5;
                    column.Item().BorderBottom(1).Padding(padding).Text("Factura de Consumo Fiscal").SemiBold();

                    column.Item().Padding(padding).Text($"NUMERO # {SaleInvoice.Id}");
                    column.Item().Text("");

                    column.Item().Padding(padding).Text($"COMPROBANTE FISCAL");
                    column.Item().Padding(padding).Text(SaleInvoice.FiscalSupportNumber);
                    column.Item().Text("");

                    column.Item().Padding(padding).Text($"COMPROBANTE FISCAL");
                    column.Item().Padding(padding).Text($"VENCIMIENTO: {SaleInvoice.DueDate?.ToString("dd-MM-yyyy")}");
                });
            });
        }
    }
}
