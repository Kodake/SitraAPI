using BackEnd.Entities.Invoices;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace BackEnd.DTOs
{
    public class InvoiceReceiptDocumentDTO : IDocument
    {
        public InvoiceReceiptDocumentDTO(Invoice invoice)
        {
            Invoice = invoice;
        }

        public Invoice Invoice { get; }

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.ContinuousSize(226.77f);
                page.Margin(10);
                page.DefaultTextStyle(x => x.FontSize(9).FontFamily("Arial"));
                page.Content().Element(BuildContent);
            });
        }

        private void BuildContent(IContainer container)
        {
            container.Column(column =>
            {
                column.Spacing(2);

                var logoPath = Path.Combine(AppContext.BaseDirectory, "Resources", "logo-black.png");

                try
                {
                    if (File.Exists(logoPath))
                    {
                        column.Item()
                              .AlignCenter()
                              .MaxHeight(1.5f, Unit.Centimetre)
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

                    Console.WriteLine($"[InvoiceReceiptDocumentDTO] Error loading logo: {ex.Message}");
                }

                // Header Info
                column.Item().AlignCenter().Text("FACTURA DE CONSUMO").SemiBold();
                column.Item().Text(" ");
                column.Item().Text($"NÚMERO: {Invoice.Id}");
                column.Item().Text($"FECHA: {Invoice.Date:dd-MM-yyyy}");
                column.Item().Text($"CLIENTE: {Invoice.Buyer?.Name ?? "N/A"}");

                if (!string.IsNullOrWhiteSpace(Invoice.Buyer?.Identification))
                    column.Item().Text($"ID: {Invoice.Buyer.Identification}");

                column.Item().Text(" ");

                // Items
                foreach (var item in Invoice.InvoiceItems)
                {
                    column.Item().Text(item.Item.Name);

                    column.Item().Row(row =>
                    {
                        row.RelativeItem().Text($"{item.Qty} x {item.BasePrice:N2}");
                        row.ConstantItem(80).AlignRight().Text($"{item.TotalPrice:N2}");
                    });

                    // Tax lines
                    foreach (var tax in item.Item.ItemTaxes)
                    {
                        var calculatedTaxAmount = item.Qty * item.BasePrice * tax.Tax.Amount / 100;
                        if (calculatedTaxAmount > 0)
                        {
                            column.Item().Row(row =>
                            {
                                row.RelativeItem().Text("");
                                row.ConstantItem(80)
                                   .AlignRight()
                                   .Text($"{tax.Tax.Name} ({tax.Tax.Amount:N0}%): {calculatedTaxAmount:N2}")
                                   .FontSize(8)
                                   .FontColor(Colors.Grey.Darken2);
                            });
                        }
                    }
                }

                // Totals
                column.Item().Text(" ");
                column.Item().Text($"SUBTOTAL: {Invoice.TotalAmount:N2}").AlignRight();

                //var taxSummary = new Dictionary<string, (double Amount, double Percentage)>();
                //foreach (var item in Invoice.InvoiceItems)
                //{
                //    foreach (var tax in item.Item.ItemTaxes)
                //    {
                //        var taxAmount = item.Qty * item.BasePrice * tax.Tax.Amount / 100;
                //        if (taxSummary.ContainsKey(tax.Tax.Name))
                //        {
                //            var current = taxSummary[tax.Tax.Name];
                //            taxSummary[tax.Tax.Name] = (current.Amount + taxAmount, tax.Tax.Amount);
                //        }
                //        else
                //        {
                //            taxSummary[tax.Tax.Name] = (taxAmount, tax.Tax.Amount);
                //        }
                //    }
                //}

                //foreach (var tax in taxSummary)
                //{
                //    column.Item()
                //          .Text($"{tax.Key} ({tax.Value.Percentage:N0}%): {tax.Value.Amount:N2}")
                //          .AlignRight();
                //}

                column.Item().Text($"IMPUESTOS: {Invoice.TotalTaxes:N2}").AlignRight();

                if (Invoice.Shipping.HasValue)
                    column.Item().Text($"ENVÍO: {Invoice.Shipping.Value:N2}").AlignRight();

                if (Invoice.Discount.HasValue)
                {
                    var subTotalWithTaxes = Invoice.TotalAmount + Invoice.TotalTaxes + (Invoice.Shipping ?? 0);
                    var discountAmount = subTotalWithTaxes * Invoice.Discount.Value / 100;
                    column.Item()
                          .Text($"DESCUENTO ({Invoice.Discount.Value:N0}%): -{discountAmount:N2}")
                          .AlignRight();
                }

                column.Item().Text($"TOTAL: {Invoice.TotalPrice:N2}").SemiBold().AlignRight();

                column.Item().Text(" ");
                column.Item().AlignCenter().Text("¡Gracias por su compra!").Italic();
            });
        }

        public void GeneratePdf(string filePath)
        {
            Document.Create(container => Compose(container)).GeneratePdf(filePath);
        }

        public byte[] GenerateToBytes()
        {
            return Document.Create(container => Compose(container)).GeneratePdf();
        }
    }
}
