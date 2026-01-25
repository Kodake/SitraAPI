using SITRA.Core.Entities.Invoices;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SITRA.Core.Helpers;

namespace SITRA.Core.DTOs
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

                    if (!string.IsNullOrEmpty(Invoice.InvoiceBuyer?.Buyer?.Identification))
                        column.Item().Scale(fontWeight).Text($"IDENTIFICACIÓN: {Invoice.InvoiceBuyer?.Buyer.Identification}");

                    column.Item().Scale(fontWeight).Text($"FECHA: {Invoice.Date:dd-MM-yyyy}");
                    column.Item().Scale(fontWeight).Text($"NOMBRE / RAZÓN SOCIAL: {Invoice.InvoiceBuyer?.Buyer?.Name ?? "N/A"}");

                    if (Invoice.InvoiceBiller?.Biller is not null)
                        column.Item().Scale(fontWeight).Text($"VENDEDOR: {Invoice.InvoiceBiller?.Biller.Name}");

                    if (Invoice.InvoiceBuyer?.Buyer is not null)
                        column.Item().Scale(fontWeight).Text($"SUPLIDOR: {Invoice.InvoiceBuyer?.Buyer.Name}");
                });

                row.RelativeItem().Border(1).Column(column =>
                {
                    var padding = 3;
                    var logoPath = Path.Combine(AppContext.BaseDirectory, "Resources", "logo-black.png");

                    try
                    {
                        if (File.Exists(logoPath))
                        {
                            column.Item()
                                  .AlignCenter()
                                  .MaxHeight(1.8f, Unit.Centimetre)
                                  .Width(100)
                                  .Image(logoPath)
                                  .FitWidth();

                            column.Item().Text(" ");
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

                        Console.WriteLine(Utilities.InvoiceDocumentErrorLoadingLogo(ex.Message));
                    }

                    column.Item().BorderBottom(1).Padding(padding).Text("FACTURA DE CONSUMO FISCAL").SemiBold();
                    column.Item().Padding(padding).Text($"NÚMERO # {Invoice.Id}").SemiBold();
                    column.Item().Padding(padding).Text("COMPROBANTE FISCAL").SemiBold();
                    column.Item().Text("");
                    column.Item().Padding(padding)
                          .Text(string.IsNullOrEmpty(Invoice.FiscalSupportNumber)
                              ? "N/A"
                              : Invoice.FiscalSupportNumber)
                          .SemiBold();

                    if (Invoice.DueDate.HasValue)
                        column.Item().Padding(padding).Text($"VENCIMIENTO: {Invoice.DueDate:dd-MM-yyyy}");
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

                // ? Tax summary
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
                    column.Item().AlignRight().Text($"{tax.Key} {tax.Value.Percentage:N}%: {tax.Value.Amount:N}");

                column.Item().AlignRight().Text($"IMPUESTO: {Invoice.TotalTaxes:N}");
                var subTaxes = Invoice.TotalAmount + Invoice.TotalTaxes;
                column.Item().AlignRight().Text($"SUB + IMPUESTO: {subTaxes:N}");

                if (Invoice.Shipping.HasValue)
                    column.Item().AlignRight().Text($"ENVÍO: {Invoice.Shipping:N}");

                if (Invoice.Discount.HasValue)
                {
                    var discountAmount = (subTaxes + (Invoice.Shipping ?? 0)) * Invoice.Discount.Value / 100;
                    column.Item().AlignRight().Text($"DESCUENTO ({Invoice.Discount:N}%): {discountAmount:N}");
                }

                column.Item().AlignRight().Text($"TOTAL: {Invoice.TotalPrice:N}");
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
                        => container.DefaultTextStyle(x => x.SemiBold())
                                    .PaddingVertical(1)
                                    .BorderBottom(1)
                                    .BorderColor(Colors.Black);
                });

                foreach (var item in Invoice.InvoiceItems)
                {
                    table.Cell().Element(CellStyle).Text(item.Item.Name);
                    table.Cell().Element(CellStyle).AlignRight().Text($"{item.Qty} x {item.BasePrice:N}");
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
            Document.Create(container => Compose(container))
                    .GeneratePdf(filePath);
        }
    }
}
