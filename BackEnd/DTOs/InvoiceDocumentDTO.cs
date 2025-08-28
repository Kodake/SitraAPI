using BackEnd.Entities.Invoices;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Reflection;

namespace BackEnd.DTOs
{
    public class InvoiceDocumentDTO : IDocument
    {
        public InvoiceDocumentDTO(Invoice invoice)
        {
            Invoice = invoice;
        }

        public Invoice Invoice { get; }

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
            container.Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    var fontWeight = 0.8f;

                    if (!string.IsNullOrEmpty(Invoice.Buyer?.Identification))
                    {
                        column.Item().Scale(fontWeight).Text($"IDENTIFICACIÓN: {Invoice.Buyer.Identification}" ?? "N/A");
                    }

                    column.Item().Scale(fontWeight).Text($"FECHA: {Invoice.Date.ToString("dd-MM-yyyy")}");
                    column.Item().Scale(fontWeight).Text($"NOMBRE / RAZÓN SOCIAL: {Invoice.Buyer?.Name ?? "N/A"}");

                    if (Invoice.Biller is not null)
                    {
                        column.Item().Scale(fontWeight).Text($"VENDEDOR: {Invoice.Biller.Name}");
                    }

                    if (Invoice.Buyer is not null)
                    {
                        column.Item().Scale(fontWeight).Text($"SUPLIDOR: {Invoice.Buyer.Name}");
                    }
                });

                row.RelativeItem().Border(1).Column(column =>
                {
                    var padding = 3;
                    var assembly = Assembly.GetExecutingAssembly();
                    var resourcePath = "BackEnd.Resources.logo-black.png";
                    var stream = assembly.GetManifestResourceStream(resourcePath);

                    if (stream != null)
                    {
                        column.Item().Height(1.5f, Unit.Centimetre).Image(stream);
                    }

                    column.Item().BorderBottom(1).Padding(padding).Text("FACTURA DE CONSUMO FISCAL").SemiBold();
                    column.Item().Padding(padding).Text($"NÚMERO # {Invoice.Id}").SemiBold();
                    column.Item().Padding(padding).Text($"COMPROBANTE FISCAL").SemiBold();
                    column.Item().Text("");
                    column.Item().Padding(padding).Text(string.IsNullOrEmpty(Invoice.FiscalSupportNumber) ? "N/A" : Invoice.FiscalSupportNumber).SemiBold();

                    if (Invoice.DueDate.HasValue)
                    {
                        column.Item().Padding(padding).Text($"VENCIMIENTO: {Invoice.DueDate.Value.ToString("dd-MM-yyyy")}");
                    }
                });
            });
        }

        private void BuildContent(IContainer container)
        {
            container.PaddingVertical(15).Column(column =>
            {
                column.Spacing(5);
                column.Item().Element(BuildTable);

                column.Item().AlignRight().Text($"SUBTOTAL: {Invoice.TotalAmount:N}");

                var taxSummary = new Dictionary<string, (double Amount, double Percentage)>();
                foreach (var item in Invoice.InvoiceItems)
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

                column.Item().AlignRight().Text($"IMPUESTO: {Invoice.TotalTaxes:N}");
                var subTaxes = Invoice.TotalAmount + Invoice.TotalTaxes;
                column.Item().AlignRight().Text($"SUB + IMPUESTO: {subTaxes:N}");

                if (Invoice.Shipping.HasValue)
                {
                    column.Item().AlignRight().Text($"ENVÍO: {Invoice.Shipping:N}");
                }

                if (Invoice.Discount.HasValue)
                {
                    var discountAmount = (subTaxes + (Invoice.Shipping ?? 0)) * Invoice.Discount.Value / 100;
                    column.Item().AlignRight().Text($"DESCUENTO ({Invoice.Discount:N}%): {discountAmount:N}");
                }

                column.Item().AlignRight().Text($"TOTAL: {(Invoice.TotalPrice):N}");
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
                    header.Cell().Element(CellStyle).AlignRight().Text("MONTO");
                    header.Cell().Element(CellStyle).AlignRight().Text("IMPUESTO");
                    header.Cell().Element(CellStyle).AlignRight().Text("TOTAL");

                    static IContainer CellStyle(IContainer container)
                    {
                        return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(1).BorderBottom(1).BorderColor(Colors.Black);
                    }
                });

                foreach (var item in Invoice.InvoiceItems)
                {
                    table.Cell().Element(CellStyle).Text(item.Item.Name);
                    table.Cell().Element(CellStyle).AlignRight().Text($"{item.Qty} x {item.BasePrice:N}");
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