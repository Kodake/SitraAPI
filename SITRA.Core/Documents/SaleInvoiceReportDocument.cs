using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SITRA.Core.Entities.Invoices;
using SITRA.Core.Models;

namespace SITRA.Core.Documents
{
    public class SaleInvoiceReportDocument : IDocument
    {
        private readonly List<Invoice> _invoices;
        private readonly DateTime _from;
        private readonly DateTime _to;
        private readonly CompanyMetadata? _company;

        private const string AccentColor = "#306090"; // Deep Blue

        public SaleInvoiceReportDocument(List<Invoice> invoices, DateTime from, DateTime to, CompanyMetadata? company = null)
        {
            _invoices = invoices;
            _from = from;
            _to = to;
            _company = company;
        }

        public DocumentMetadata GetMetadata()
        {
            return new DocumentMetadata
            {
                Title = $"Sale_{_from:yyyy-MM-dd}_{_to:yyyy-MM-dd}",
                Subject = "Reporte de Ventas",
                Author = _company?.CompanyName ?? "SITRA"
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
                    string logoPath = Path.Combine(AppContext.BaseDirectory, "Resources", "logo-black.png");
                    if (File.Exists(logoPath))
                    {
                        col.Item().Height(60).Image(logoPath).FitArea();
                    }
                    else
                    {
                        col.Item().Height(60).Placeholder();
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

                // Right Side: Report Details Box
                row.ConstantItem(220).PaddingTop(5).Border(1f).BorderColor(AccentColor).Background(Colors.White).Column(col =>
                {
                    col.Item().Padding(10).AlignCenter().Column(sub =>
                    {
                        sub.Item().PaddingBottom(5).AlignCenter().Text("REPORTE DE VENTAS").SemiBold().FontSize(10).FontColor(AccentColor).AlignCenter();
                        sub.Item().PaddingTop(8).AlignCenter().Text(x =>
                        {
                            x.Span("Desde: ").FontSize(8);
                            x.Span(_from.ToString("dd-MM-yyyy")).SemiBold().FontSize(8).FontColor(Colors.Black);
                        });
                        sub.Item().AlignCenter().Text(x =>
                        {
                            x.Span("Hasta: ").FontSize(8);
                            x.Span(_to.ToString("dd-MM-yyyy")).SemiBold().FontSize(8).FontColor(Colors.Black);
                        });
                    });
                });
            });
        }

        private void ComposeContent(IContainer container)
        {
            container.Column(column =>
            {
                // Items Table
                column.Item().PaddingTop(10).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(1.5f); // FECHA
                        columns.RelativeColumn(3); // CLIENTE
                        columns.RelativeColumn(2); // NCF
                        columns.RelativeColumn(1.5f); // TOTAL
                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(HeaderStyle).Text("FECHA");
                        header.Cell().Element(HeaderStyle).Text("CLIENTE");
                        header.Cell().Element(HeaderStyle).Text("NCF");
                        header.Cell().Element(HeaderStyle).AlignRight().Text("TOTAL");

                        static IContainer HeaderStyle(IContainer c) =>
                            c.BorderBottom(1.5f).BorderColor(AccentColor).PaddingVertical(5).DefaultTextStyle(x => x.SemiBold().FontSize(8).FontColor(AccentColor));
                    });

                    foreach (var item in _invoices.OrderBy(i => i.CreatedAt))
                    {
                        table.Cell().Element(RowStyle).Text(item.CreatedAt.ToString("dd-MM-yyyy"));
                        table.Cell().Element(RowStyle).Text(item.InvoiceBuyer?.Buyer?.Name ?? "N/A");
                        table.Cell().Element(RowStyle).Text(item.FiscalSupportNumber ?? "N/A");
                        table.Cell().Element(RowStyle).AlignRight().Text(item.TotalPrice.ToString("N2")).SemiBold().FontColor(Colors.Black);
                    }

                    static IContainer RowStyle(IContainer c) =>
                        c.BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).PaddingVertical(6);
                });

                // Custom spacer before total section footer block
                column.Item().PaddingTop(25);

                // BOTTOM SECTION: Totals (right)
                column.Item().Row(row =>
                {
                    row.RelativeItem().Column(col => { col.Item().Height(50); });

                    row.ConstantItem(250).Column(col =>
                    {
                        double totalSub = _invoices.Sum(x => (double)x.TotalAmount);
                        double totalTaxes = _invoices.Sum(x => (double)x.TotalTaxes);
                        double totalGeneral = _invoices.Sum(x => (double)x.TotalPrice);

                        col.Item().Element(TotalLineStyle).Row(r =>
                        {
                            r.RelativeItem().Text("SUBTOTAL").FontSize(8).SemiBold();
                            r.RelativeItem().AlignRight().Text(totalSub.ToString("N2")).SemiBold().FontColor(Colors.Black);
                        });

                        col.Item().Element(TotalLineStyle).Row(r =>
                        {
                            r.RelativeItem().Text("TOTAL IMPUESTOS").FontSize(8).SemiBold();
                            r.RelativeItem().AlignRight().Text(totalTaxes.ToString("N2")).SemiBold().FontColor(Colors.Black);
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
            });
        }
    }
}
