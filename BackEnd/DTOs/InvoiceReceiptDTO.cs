using BackEnd.Entities.Auths;
using BackEnd.Entities.Invoices;
using BackEnd.Enums;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace BackEnd.DTOs
{
    public class InvoiceReceiptDocumentDTO : IDocument
    {
        public InvoiceReceiptDocumentDTO(Invoice invoice, User user)
        {
            Invoice = invoice;
            User = user;
        }

        public Invoice Invoice { get; }
        public User User { get; }

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.ContinuousSize(226.77f); // 80mm
                page.Margin(10);
                page.DefaultTextStyle(x => x.FontSize(9).FontFamily("Arial"));
                page.Content().Element(BuildContent);
            });
        }

        private void BuildContent(IContainer container)
        {
            container.Column(column =>
            {
                //column.Spacing(2);

                //// --------------------------------------------------
                //// LOGO
                //// --------------------------------------------------
                //var logoPath = Path.Combine(
                //    AppContext.BaseDirectory,
                //    "Resources",
                //    "logo-black.png");

                //try
                //{
                //    if (File.Exists(logoPath))
                //    {
                //        column.Item()
                //              .AlignCenter()
                //              .MaxHeight(1.5f, Unit.Centimetre)
                //              .Width(100)
                //              .Image(logoPath)
                //              .FitWidth();
                //    }
                //    else
                //    {
                //        column.Item()
                //              .AlignCenter()
                //              .Text("LOGO NO DISPONIBLE")
                //              .FontSize(8)
                //              .Italic();
                //    }
                //}
                //catch (Exception ex)
                //{
                //    column.Item()
                //          .AlignCenter()
                //          .Text("ERROR AL CARGAR LOGO")
                //          .FontSize(8)
                //          .Italic();

                //    Console.WriteLine($"[InvoiceReceiptDocumentDTO] Error loading logo: {ex.Message}");
                //}

                //column.Item().Text(" ");

                // --------------------------------------------------
                // COMPANY INFO
                // --------------------------------------------------
                column.Item().Column(col =>
                {
                    col.Spacing(1);

                    col.Item().Row(r =>
                    {
                        r.ConstantItem(120).Text(User.CompanyName).FontSize(10).SemiBold();
                    });

                    col.Item().Row(r =>
                    {
                        r.ConstantItem(120).Text(User.Email).FontSize(10).SemiBold();
                    });

                    col.Item().Row(r =>
                    {
                        r.RelativeItem().Text("TELÈFONO:").SemiBold();
                        r.ConstantItem(120).AlignRight().Text(User.PhoneNumber).SemiBold();
                    });

                    col.Item().Row(r =>
                    {
                        r.RelativeItem().Text("RNC:").SemiBold();
                        r.ConstantItem(120).AlignRight().Text(User.RucNumber).SemiBold();
                    });

                    col.Item().Row(r =>
                    {
                        r.ConstantItem(120).Text(User.Address).FontSize(10).SemiBold();
                    });
                });

                column.Item().LineHorizontal(0.8f);

                // --------------------------------------------------
                // LOGIN INFO
                // --------------------------------------------------

                column.Item().Column(col =>
                {
                    col.Spacing(1);

                    col.Item().Row(r =>
                    {
                        r.RelativeItem().Text("Usuario:");
                        r.ConstantItem(120).AlignRight().Text(User.UserName);
                    });

                    col.Item().Row(r =>
                    {
                        r.RelativeItem().Text("Condición:");
                        r.ConstantItem(120)
                         .AlignRight()
                         .Text(Invoice.ConditionType == InvoiceCondition.Spot
                               ? "AL CONTADO"
                               : "CRÊDITO");
                    });
                });

                column.Item().LineHorizontal(0.8f);

                // --------------------------------------------------
                // INVOICE INFO (HEADER DETAILS)
                // --------------------------------------------------
                column.Item().Column(col =>
                {
                    col.Spacing(1);

                    col.Item().Row(r =>
                    {
                        r.RelativeItem().Text("Factura:");
                        r.ConstantItem(120).AlignRight().Text(Invoice.Id.ToString());
                    });

                    col.Item().Row(r =>
                    {
                        r.RelativeItem().Text("Fecha:");
                        r.ConstantItem(120).AlignRight().Text(Invoice.Date.ToString("dd/MM/yyyy"));
                    });

                    col.Item().Row(r =>
                    {
                        r.RelativeItem().Text("NCF:");
                        r.ConstantItem(120)
                         .AlignRight()
                         .Text(Invoice.FiscalSupportNumber ?? "-");
                    });

                    col.Item().Row(r =>
                    {
                        r.RelativeItem().Text("Vence:");
                        r.ConstantItem(120).AlignRight().Text(
                            Invoice.DueDate?.ToString("dd/MM/yyyy") ?? "-"
                        );
                    });

                    col.Item().Row(r =>
                    {
                        r.RelativeItem().Text("RNC / Cédula:");
                        r.ConstantItem(120).AlignRight().Text(
                            Invoice.Buyer?.Identification ?? "-"
                        );
                    });

                    col.Item().Row(r =>
                    {
                        r.RelativeItem().Text("Cliente:");
                        r.ConstantItem(120).AlignRight().Text(
                            Invoice.Buyer?.Name ?? "-"
                        );
                    });
                });

                column.Item().LineHorizontal(0.8f);

                // --------------------------------------------------
                // HEADER
                // --------------------------------------------------
                column.Item().AlignCenter().Text("FACTURA CONSUMIDOR FINAL").SemiBold();
                column.Item().LineHorizontal(0.8f);

                // --------------------------------------------------
                // TABLE HEADER
                // --------------------------------------------------
                column.Item().Row(row =>
                {
                    row.RelativeItem().Text("Artículos");
                    row.ConstantItem(45).AlignRight().Text("Impuestos");
                    row.ConstantItem(45).AlignRight().Text("Total");
                });

                column.Item().LineHorizontal(0.5f);

                // --------------------------------------------------
                // ITEMS
                // --------------------------------------------------
                foreach (var item in Invoice.InvoiceItems)
                {
                    var taxAmount = item.Item.ItemTaxes.Sum(t =>
                        item.Qty * item.BasePrice * t.Tax.Amount / 100);

                    column.Item().Row(row =>
                    {
                        row.ConstantItem(20).Text(item.Qty.ToString());
                        row.RelativeItem().Text(item.Item.Name);
                        row.ConstantItem(45).AlignRight().Text($"{taxAmount:N2}");
                        row.ConstantItem(45).AlignRight().Text($"{item.TotalPrice:N2}");
                    });

                    column.Item().PaddingLeft(20).Row(row =>
                    {
                        row.RelativeItem().Text($"{item.BasePrice:N2}");
                        row.ConstantItem(45);
                        row.ConstantItem(45);
                    });
                }

                column.Item().Text(" ");
                column.Item().LineHorizontal(0.8f);

                // --------------------------------------------------
                // TOTALS
                // --------------------------------------------------
                column.Item().Row(row =>
                {
                    row.RelativeItem();
                    row.ConstantItem(120).Column(col =>
                    {
                        col.Item().Row(r =>
                        {
                            r.RelativeItem().Text("Sub Total:");
                            r.ConstantItem(60).AlignRight().Text($"{Invoice.TotalAmount:N2}");
                        });

                        col.Item().Row(r =>
                        {
                            r.RelativeItem().Text("ITBIS:");
                            r.ConstantItem(60).AlignRight().Text($"{Invoice.TotalTaxes:N2}");
                        });

                        col.Item().Row(r =>
                        {
                            r.RelativeItem().Text("Descuento:");
                            r.ConstantItem(60).AlignRight().Text("0.00");
                        });

                        col.Item().PaddingTop(2).Row(r =>
                        {
                            r.RelativeItem().Text("Monto a pagar:").SemiBold();
                            r.ConstantItem(60).AlignRight().Text($"{Invoice.TotalPrice:N2}").SemiBold();
                        });
                    });
                });

                column.Item().Text(" ");
                column.Item().LineHorizontal(0.8f);

                // --------------------------------------------------
                // PAYMENT METHODS (ALWAYS SHOW ALL)
                // --------------------------------------------------
                var paymentOrder = new[]
                {
                    PaymentMethod.Cash,
                    PaymentMethod.Transfer,
                    PaymentMethod.Card,
                };

                var realPayments = Invoice.SpotPayment?.Payments?
                    .GroupBy(p => p.PaymentMethod)
                    .Select(g => new PaymentSummary(
                        g.Key,
                        g.Sum(x => x.Amount)
                    ))
                    .ToList()
                    ?? new List<PaymentSummary>();

                var allPayments = paymentOrder
                    .Select(method =>
                    {
                        var existing = realPayments.FirstOrDefault(p => p.Method == method);
                        return new PaymentSummary(
                            method,
                            existing?.Amount ?? 0
                        );
                    })
                    .ToList();

                column.Item().Column(col =>
                {
                    col.Spacing(1);

                    foreach (var payment in allPayments)
                    {
                        col.Item().Row(r =>
                        {
                            r.RelativeItem().Text(GetPaymentMethodLabel(payment.Method));
                            r.ConstantItem(60)
                             .AlignRight()
                             .Text($"{payment.Amount:N2}");
                        });
                    }
                });

                // --------------------------------------------------
                // CHANGE
                // --------------------------------------------------
                var totalPaid = allPayments.Sum(p => p.Amount);
                var change = totalPaid - Invoice.TotalPrice;

                column.Item().Text(" ");

                column.Item().Row(row =>
                {
                    row.RelativeItem().Text("Cambio").SemiBold();
                    row.ConstantItem(60)
                       .AlignRight()
                       .Text($"{Math.Max(change, 0):N2}")
                       .SemiBold();
                });

                column.Item().LineHorizontal(0.8f);

                // --------------------------------------------------
                // OPTIONAL DESCRIPTION
                // --------------------------------------------------
                if (!string.IsNullOrWhiteSpace(Invoice.Description))
                {
                    column.Item()
                          .PaddingTop(4)
                          .AlignCenter()
                          .Text(Invoice.Description)
                          .FontSize(8)
                          .Italic();
                }

                // --------------------------------------------------
                // FOOTER
                // --------------------------------------------------
                column.Item().PaddingTop(4)
                      .AlignCenter()
                      .Text("No hacemos devoluciones en efectivo,\nserán emitidas notas de crédito en caso de cambio")
                      .FontSize(7)
                      .Italic();
            });
        }

        // --------------------------------------------------
        // HELPERS
        // --------------------------------------------------
        private record PaymentSummary(PaymentMethod Method, double Amount);

        private static string GetPaymentMethodLabel(PaymentMethod method)
        {
            return method switch
            {
                PaymentMethod.Cash => "Efectivo",
                PaymentMethod.Transfer => "Transferencia",
                PaymentMethod.Card => "Tarjeta",
                _ => "Desconocido"
            };
        }

        public void GeneratePdf(string filePath)
        {
            Document.Create(container => Compose(container))
                    .GeneratePdf(filePath);
        }

        public byte[] GenerateToBytes()
        {
            return Document.Create(container => Compose(container))
                           .GeneratePdf();
        }
    }
}
