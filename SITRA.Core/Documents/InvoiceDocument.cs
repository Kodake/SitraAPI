using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SITRA.Core.Entities.Invoices;
using SITRA.Core.Helpers;
using SITRA.Core.Models;
using SITRA.Core.Models.Dgii;

namespace SITRA.Core.Documents
{
    public class InvoiceDocument : IDocument
    {
        private readonly Invoice _invoice;
        private readonly DgiiPdfMetadata? _dgiiMetadata;
        private readonly CompanyMetadata? _company;
        
        // Color definition matching the provided sample's aesthetics
        private const string AccentColor = "#306090"; // Deep Blue

        public InvoiceDocument(Invoice invoice, DgiiPdfMetadata? dgiiMetadata = null, CompanyMetadata? company = null)
        {
            _invoice = invoice;
            _dgiiMetadata = dgiiMetadata;
            _company = company;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

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
                    string logoPath = Path.Combine(AppContext.BaseDirectory, "Resources", "logo-black.png");
                    if (File.Exists(logoPath))
                    {
                        col.Item().Height(60).Image(logoPath, ImageScaling.FitArea);
                    }
                    else
                    {
                        col.Item().Height(60).Placeholder(); // Just placeholder height if no logo
                    }

                    string companyName = _company?.CompanyName ?? _dgiiMetadata?.RazonSocialEmisor ?? "MI EMPRESA";
                    string rncNumber = _company?.RucNumber ?? _dgiiMetadata?.EmisorRnc ?? "N/A";

                    col.Item().PaddingTop(5).Text(companyName).FontSize(13).SemiBold().FontColor(Colors.Black);
                    col.Item().PaddingTop(1).Text($"RNC: {rncNumber}").FontSize(10).SemiBold().FontColor(Colors.Black);
                    
                    col.Item().PaddingTop(3).Column(sub =>
                    {
                        if (!string.IsNullOrEmpty(_company?.Address))
                        {
                            sub.Item().Text(_company.Address).FontSize(8).FontColor(Colors.Grey.Darken2);
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
                        
                        if (!string.IsNullOrEmpty(_company?.Email))
                        {
                            sub.Item().Text($"Email: {_company.Email}").FontSize(8).FontColor(Colors.Grey.Darken2);
                        }
                    });
                });

                // Right Side: E-CF Registration Box
                row.ConstantItem(220).PaddingTop(5).Border(1f).BorderColor(AccentColor).Background(Colors.White).Column(col =>
                {
                    col.Item().Padding(10).AlignCenter().Column(sub =>
                    {
                        string typeCode = _dgiiMetadata?.TipoEcf ?? (_invoice.FiscalSupportNumber?.Length >= 3 ? _invoice.FiscalSupportNumber.Substring(1, 2) : "31");
                        string eCftypeStr = GetEcfTypeName(typeCode).ToUpper();

                        sub.Item().PaddingBottom(5).AlignCenter().Text(eCftypeStr).SemiBold().FontSize(8).FontColor(AccentColor).AlignCenter();
                        
                        sub.Item().PaddingTop(5).AlignCenter().Text(_invoice.FiscalSupportNumber ?? "N/A").SemiBold().FontSize(20).FontColor(Colors.Black);
                        
                        sub.Item().PaddingTop(8).AlignCenter().Text(x => 
                        {
                            x.Span("Fecha Emisión: ").FontSize(8);
                            x.Span(_invoice.Date.ToString("dd-MM-yyyy")).SemiBold().FontSize(8).FontColor(Colors.Black);
                        });

                        sub.Item().AlignCenter().Text(x => 
                        {
                            x.Span("Fecha Vencimiento: ").FontSize(8);
                            x.Span(_invoice.DueDate?.ToString("dd-MM-yyyy") ?? "31-12-2028").SemiBold().FontSize(8).FontColor(Colors.Black);
                        });
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
                        col.Item().Text("COMPRADOR").FontSize(8).SemiBold().FontColor(Colors.Grey.Darken2);
                        col.Item().Text(_dgiiMetadata?.RazonSocialComprador ?? _invoice.InvoiceBuyer?.Buyer?.Name ?? "CLIENTE FINAL").SemiBold().FontSize(11).FontColor(Colors.Black);
                        col.Item().Text($"RNC/Cedula: {_dgiiMetadata?.CompradorRnc ?? _invoice.InvoiceBuyer?.Buyer?.Identification ?? "N/A"}").FontSize(9);
                    });

                    // Right: Payment details
                    row.ConstantItem(150).AlignRight().Column(col =>
                    {
                        col.Item().Text("DETALLES DE PAGO").FontSize(8).SemiBold().FontColor(Colors.Grey.Darken2);
                        col.Item().Text(x => {
                            x.Span("Condicion: ").FontSize(9);
                            x.Span(_invoice.ConditionType == Enums.InvoiceCondition.Spot ? "Contado" : "Crédito").FontSize(9).SemiBold().FontColor(Colors.Black);
                        });
                        col.Item().Text(x => {
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
                        header.Cell().Element(HeaderStyle).Text("DESCRIPCION");
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

                        static IContainer RowStyle(IContainer c) => 
                            c.BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).PaddingVertical(6);
                    }
                });

                // Custom spacer before total section footer block
                column.Item().PaddingTop(25);

                // BOTTOM SECTION: QR (left) + Totals (right)
                column.Item().Row(row =>
                {
                    // QR Side
                    row.RelativeItem().Column(col =>
                    {
                        if (_dgiiMetadata != null)
                        {
                            try
                            {
                                string qrUrl = DgiiQrUrlHelper.BuildDgiiQrUrl(_dgiiMetadata);
                                byte[] qrBytes = QrCodeGeneratorService.GenerateQrCodeBytes(qrUrl);
                                col.Item().Width(100).Height(100).Image(qrBytes, ImageScaling.FitArea);
                                
                                col.Item().PaddingTop(5).Text(x => {
                                    x.Span("Codigo Seguridad: ").SemiBold().FontSize(7);
                                    x.Span(_dgiiMetadata.SecurityCode ?? "N/A").FontSize(7);
                                });

                                if (!string.IsNullOrEmpty(_dgiiMetadata.FechaFirma))
                                {
                                    col.Item().Text(x => {
                                        x.Span("Fecha Firma: ").SemiBold().FontSize(7);
                                        x.Span(_dgiiMetadata.FechaFirma).FontSize(7);
                                    });
                                }
                            }
                            catch { }
                        }
                        else
                        {
                            // Dummy placeholder area for consistency
                            col.Item().Height(100);
                        }
                    });

                    // Totals side
                    row.ConstantItem(250).Column(col =>
                    {
                        // To reflect the screenshot visual exactly, we create lines with borders
                        
                        // 1. Calculo de montos
                        double totalExento = _invoice.InvoiceItems.Where(i => i.Item?.ItemTaxes == null || !i.Item.ItemTaxes.Any()).Sum(i => i.TotalPrice);
                        double totalGravado = _invoice.TotalAmount - totalExento;

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
                            r.RelativeItem().AlignRight().Text(_invoice.TotalTaxes.ToString("N2")).SemiBold().FontColor(Colors.Black);
                        });

                        // The final Heavy Line for TOTAL GENERAL
                        col.Item().PaddingTop(3).BorderBottom(1.5f).BorderColor(AccentColor).Row(r =>
                        {
                            r.RelativeItem().PaddingBottom(2).Text("TOTAL GENERAL").FontSize(9).SemiBold();
                            r.RelativeItem().AlignRight().PaddingBottom(2).Text($"{_invoice.TotalPrice:N2}").FontSize(10).SemiBold().FontColor(Colors.Black);
                        });
                        
                        static IContainer TotalLineStyle(IContainer c) => 
                            c.BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).PaddingVertical(3);
                    });
                });

                // Signature Placeholder or Custom Disclaimer
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

        private string GetEcfTypeName(string? typeCode)
        {
            return typeCode switch
            {
                "31" => "Factura de Crédito Fiscal Electrónica",
                "32" => "Factura de Consumo Electrónica",
                "33" => "Nota de Débito Electrónica",
                "34" => "Nota de Crédito Electrónica",
                "41" => "Registro de Compras Electrónico",
                "43" => "Gastos Menores Electrónicos",
                "44" => "Regímenes Especiales Electrónicos",
                "45" => "Gubernamentales Electrónicos",
                _ => "Factura Electrónica"
            };
        }

        public void GeneratePdf(string filePath)
        {
            Document.Create(container => Compose(container)).GeneratePdf(filePath);
        }
    }
}


