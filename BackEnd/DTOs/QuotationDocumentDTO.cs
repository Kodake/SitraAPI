using BackEnd.Entities.Invoices;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace BackEnd.DTOs
{
    public class QuotationDocumentDTO : IDocument
    {
        public QuotationDocumentDTO(Invoice saleInvoice)
        {
            SaleInvoice = saleInvoice;
        }

        public Invoice SaleInvoice { get; }

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

        private void BuildHeader(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    var fontWeight = 0.8f;
                    if (SaleInvoice.Buyer.Identification != null)
                    {
                        column.Item().Scale(fontWeight).Text($"IDENTIFICACIÓN: {SaleInvoice.Buyer.Identification}" ?? "N/A");
                    }
                    column.Item().Scale(fontWeight).Text($"FECHA: {SaleInvoice.Date.ToString("dd-MM-yyyy")}");
                    column.Item().Scale(fontWeight).Text($"NOMBRE / RAZÓN SOCIAL: {SaleInvoice.Buyer.Name}");
                });

                row.RelativeItem().Border(1).Column(column =>
                {
                    var padding = 5;
                    var logo = @"C:\Users\Kodake\Desktop\logo-black.png";

                    column.Item().Height(2, Unit.Centimetre).Image(logo);
                    column.Item().BorderBottom(1).Padding(padding).Text("COTIZACIÓN").SemiBold();

                    column.Item().Padding(padding).Text("NÚMERO DE COTIZACIÓN").SemiBold();
                    column.Item().Padding(padding).Text(SaleInvoice?.Quotation?.QuotationNumber);
                    column.Item().Text("");

                    if (SaleInvoice != null && SaleInvoice.DueDate.HasValue)
                    {
                        column.Item().Padding(padding).Text(text =>
                        {
                            text.Span("VENCIMIENTO: ").SemiBold();
                            text.Span(SaleInvoice.DueDate.Value.ToString("dd-MM-yyyy"));
                        });
                    }
                });
            });
        }

        private void BuildContent(IContainer container)
        {
            container.PaddingVertical(40).Column(column =>
            {
                column.Spacing(5);

                column.Item().Element(BuildTable);
                column.Item().Text("");
                column.Item().Text("");
                column.Item().AlignRight().Text($"SUBTOTAL: {SaleInvoice.TotalAmount:N}");

                var taxSummary = new Dictionary<string, (double Amount, double Percentage)>();
                foreach (var item in SaleInvoice.InvoiceItems)
                {
                    foreach (var tax in item.Item.ItemTaxes)
                    {
                        var calculatedTaxAmount = item.Qty * item.BasePrice * tax.Tax.Amount / 100;

                        if (taxSummary.ContainsKey(tax.Tax.Name))
                        {
                            taxSummary[tax.Tax.Name] = (
                                Amount: taxSummary[tax.Tax.Name].Amount + calculatedTaxAmount,
                                Percentage: tax.Tax.Amount
                            );
                        }
                        else
                        {
                            taxSummary[tax.Tax.Name] = (calculatedTaxAmount, tax.Tax.Amount);
                        }
                    }
                }

                foreach (var tax in taxSummary)
                {
                    column.Item().AlignRight().Text($"{tax.Key} {tax.Value.Percentage:N}%: {tax.Value.Amount:N}");
                }

                column.Item().AlignRight().Text($"IMPUESTOS: {SaleInvoice.TotalTaxes:N}");
                var subTaxes = SaleInvoice.TotalAmount + SaleInvoice.TotalTaxes;
                column.Item().AlignRight().Text($"SUB + IMPUESTOS: {subTaxes:N}");
                if (SaleInvoice.Shipping.HasValue)
                {
                    column.Item().AlignRight().Text($"ENVÍO: {SaleInvoice.Shipping:N}");
                }

                if (SaleInvoice.Discount.HasValue)
                {
                    var discountAmount = (subTaxes + (SaleInvoice.Shipping ?? 0)) * SaleInvoice.Discount.Value / 100;
                    column.Item().AlignRight().Text($"DESCUENTO ({SaleInvoice.Discount:N}%): {discountAmount:N}");
                }

                column.Item().AlignRight().Text($"TOTAL: {(SaleInvoice.TotalPrice):N}");
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
                    column.RelativeColumn();
                });

                table.Header(header =>
                {
                    header.Cell().Element(CellStyle).Text("DESCRIPCIÓN");
                    header.Cell().Element(CellStyle).AlignRight().Text("CANTIDAD");
                    header.Cell().Element(CellStyle).AlignRight().Text("VALOR");
                    header.Cell().Element(CellStyle).AlignRight().Text("IMPUESTOS");
                    header.Cell().Element(CellStyle).AlignRight().Text("PRECIO");

                    static IContainer CellStyle(IContainer container)
                    {
                        return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(1).BorderBottom(1).BorderColor(Colors.Black);
                    }
                });

                foreach (var item in SaleInvoice.InvoiceItems)
                {
                    table.Cell().Element(CellStyle).Text(item.Item.Name);
                    table.Cell().Element(CellStyle).AlignRight().Text($"{item.Qty} x {item.BasePrice}");
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
