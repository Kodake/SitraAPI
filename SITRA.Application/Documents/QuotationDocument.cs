using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SITRA.Core.Entities.Invoices;
using SITRA.Core.Models;

namespace SITRA.Core.Documents
{
    public class QuotationDocument : IDocument
    {
        private readonly Invoice _invoice;
        private readonly CompanyMetadata? _company;

        // Color definition matching the provided sample's aesthetics
        private const string AccentColor = "#306090"; // Deep Blue

        public QuotationDocument(Invoice invoice, CompanyMetadata? company = null)
        {
            _invoice = invoice;
            _company = company;
        }

        public DocumentMetadata GetMetadata()
        {
            var rnc = _company?.RucNumber ?? "Cotizacion";
            var quotationNumber = _invoice.Quotation?.QuotationNumber ?? _invoice.Id.ToString();

            return new DocumentMetadata
            {
                Title = $"{quotationNumber}",
                Subject = "Cotización",
                Author = _company?.CompanyName ?? "SITRA",
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

                // Right Side: Quotation Registration Box
                row.ConstantItem(220).PaddingTop(5).Border(1f).BorderColor(AccentColor).Background(Colors.White).Column(col =>
                {
                    col.Item().Padding(10).AlignCenter().Column(sub =>
                    {
                        sub.Item().PaddingBottom(5).AlignCenter().Text("COTIZACIÓN").SemiBold().FontSize(10).FontColor(AccentColor).AlignCenter();

                        sub.Item().PaddingTop(5).AlignCenter().Text(_invoice.Quotation?.QuotationNumber ?? "N/A").SemiBold().FontSize(20).FontColor(Colors.Black);

                        sub.Item().PaddingTop(8).AlignCenter().Text(x =>
                        {
                            x.Span("Fecha Emisión: ").FontSize(8);
                            string emission = _invoice.Date.ToString("dd-MM-yyyy");
                            x.Span(emission).SemiBold().FontSize(8).FontColor(Colors.Black);
                        });

                        if (_invoice.DueDate.HasValue)
                        {
                            sub.Item().AlignCenter().Text(x =>
                            {
                                x.Span("Válido Hasta: ").FontSize(8);
                                string due = _invoice.DueDate.Value.ToString("dd-MM-yyyy");
                                x.Span(due).SemiBold().FontSize(8).FontColor(Colors.Black);
                            });
                        }
                    });
                });
            });
        }

        private void ComposeContent(IContainer container)
        {
            container.Column(column =>
            {
                // Buyer info and Payment Method line
                column.Item().PaddingBottom(20).Row(row =>
                {
                    // Left: Comprador
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text("CLIENTE").FontSize(8).SemiBold().FontColor(Colors.Grey.Darken2);
                        col.Item().Text(_invoice.Buyer?.Name ?? "CLIENTE FINAL").SemiBold().FontSize(11).FontColor(Colors.Black);
                        col.Item().Text($"RNC/Cédula: {_invoice.Buyer?.Identification ?? "N/A"}").FontSize(9);
                    });

                    // Right: Payment details
                    row.ConstantItem(150).AlignRight().Column(col =>
                    {
                        col.Item().Text("DETALLES").FontSize(8).SemiBold().FontColor(Colors.Grey.Darken2);
                        col.Item().Text(x =>
                        {
                            x.Span("Condición: ").FontSize(9);
                            x.Span(_invoice.ConditionType == Enums.InvoiceCondition.Spot ? "Contado" : "Crédito").FontSize(9).SemiBold().FontColor(Colors.Black);
                        });
                        col.Item().Text(x =>
                        {
                            x.Span("Moneda: ").FontSize(9);
                            x.Span(_invoice.Currency?.Code ?? "DOP").FontSize(9).SemiBold().FontColor(Colors.Black);
                        });
                    });
                });

                // Items Table
                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(1); // CANT
                        columns.RelativeColumn(4); // DESCRIPCION
                        columns.RelativeColumn(1.5f); // PRECIO UNIT
                        columns.RelativeColumn(1.5f); // TOTAL
                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(HeaderStyle).Text("CANT.");
                        header.Cell().Element(HeaderStyle).Text("DESCRIPCIÓN");
                        header.Cell().Element(HeaderStyle).AlignRight().Text("PRECIO UNIT.");
                        header.Cell().Element(HeaderStyle).AlignRight().Text("TOTAL");

                        static IContainer HeaderStyle(IContainer c) =>
                            c.BorderBottom(1.5f).BorderColor(AccentColor).PaddingVertical(5).DefaultTextStyle(x => x.SemiBold().FontSize(8).FontColor(AccentColor));
                    });

                    foreach (var item in _invoice.InvoiceItems)
                    {
                        table.Cell().Element(RowStyle).Text(item.Qty.ToString("N2"));
                        table.Cell().Element(RowStyle).Text(item.Item?.Name ?? "N/A");
                        table.Cell().Element(RowStyle).AlignRight().Text(item.BasePrice.ToString("N2"));
                        table.Cell().Element(RowStyle).AlignRight().Text((item.BasePrice * item.Qty).ToString("N2")).SemiBold().FontColor(Colors.Black);
                    }

                    static IContainer RowStyle(IContainer c) =>
                        c.BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).PaddingVertical(6);
                });

                // Custom spacer before total section footer block
                column.Item().PaddingTop(25);

                // BOTTOM SECTION: Totals (right)
                column.Item().Row(row =>
                {
                    // Empty Side (No QR code for quotations)
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Height(100);
                    });

                    // Totals side
                    row.ConstantItem(250).Column(col =>
                    {
                        double totalExento = _invoice.InvoiceItems.Where(i => i.Item?.ItemTaxes == null || !i.Item.ItemTaxes.Any()).Sum(i => i.TotalPrice);
                        double totalGeneral = (double)_invoice.TotalPrice;
                        double totalItbis = (double)_invoice.TotalTaxes;
                        double totalGravado = (double)_invoice.TotalAmount - totalExento;

                        col.Item().Element(TotalLineStyle).Row(r =>
                        {
                            r.RelativeItem().Text("MONTO EXENTO").FontSize(8).SemiBold();
                            r.RelativeItem().AlignRight().Text(totalExento.ToString("N2")).SemiBold().FontColor(Colors.Black);
                        });

                        col.Item().Element(TotalLineStyle).Row(r =>
                        {
                            r.RelativeItem().Text("MONTO GRAVADO").FontSize(8).SemiBold();
                            r.RelativeItem().AlignRight().Text(totalGravado.ToString("N2")).SemiBold().FontColor(Colors.Black);
                        });

                        col.Item().Element(TotalLineStyle).Row(r =>
                        {
                            r.RelativeItem().Text("TOTAL ITBIS").FontSize(8).SemiBold();
                            r.RelativeItem().AlignRight().Text(totalItbis.ToString("N2")).SemiBold().FontColor(Colors.Black);
                        });

                        // The final Heavy Line for TOTAL GENERAL
                        col.Item().PaddingTop(3).BorderBottom(1.5f).BorderColor(AccentColor).Row(r =>
                        {
                            r.RelativeItem().PaddingBottom(2).Text("TOTAL GENERAL").FontSize(9).SemiBold();
                            r.RelativeItem().AlignRight().PaddingBottom(2).Text(totalGeneral.ToString("N2")).FontSize(10).SemiBold().FontColor(Colors.Black);
                        });

                        static IContainer TotalLineStyle(IContainer c) =>
                            c.BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).PaddingVertical(3);
                    });
                });

                // Notes
                if (!string.IsNullOrEmpty(_invoice.Note))
                {
                    column.Item().PaddingTop(30).Column(sub =>
                    {
                        sub.Item().Text("NOTAS:").SemiBold().FontSize(8);
                        sub.Item().Text(_invoice.Note).FontSize(8).Italic().FontColor(Colors.Grey.Darken2);
                    });
                }
            });
        }
    }
}
