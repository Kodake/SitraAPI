using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using SITRA.Core.Documents.Components;
using SITRA.Core.Entities.Auths;
using SITRA.Core.Entities.Invoices;
using SITRA.Core.Enums;
using System;
using System.IO;

namespace SITRA.Core.Documents
{
    public class InvoiceReceiptDocument : ThermalDocumentBase
    {
        public Invoice Invoice { get; }
        public User User { get; }
        private readonly SITRA.Core.Models.Dgii.DgiiPdfMetadata? _dgiiMetadata;
        private readonly SITRA.Core.Models.Dgii.DgiiSettings? _dgiiSettings;

        public InvoiceReceiptDocument(Invoice invoice, User user, SITRA.Core.Models.Dgii.DgiiPdfMetadata? dgiiMetadata = null, SITRA.Core.Models.Dgii.DgiiSettings? dgiiSettings = null)
        {
            Invoice = invoice;
            User = user;
            _dgiiMetadata = dgiiMetadata;
            _dgiiSettings = dgiiSettings;
        }

        protected override void BuildHeader(IContainer container)
        {
            container.Column(column =>
            {
                string logoPath = Path.Combine(AppContext.BaseDirectory, "Resources", "logo-black.png");
                if (File.Exists(logoPath))
                {
                    column.Item().AlignCenter().PaddingBottom(10).Height(60).Image(logoPath, ImageScaling.FitArea);
                }

                if (User != null)
                {
                    column.Item().AlignCenter().Text(User.CompanyName).SemiBold();
                    if (!string.IsNullOrEmpty(User.Address))
                        column.Item().AlignCenter().Text($"Dirección: {User.Address}");
                    if (!string.IsNullOrEmpty(User.RucNumber))
                        column.Item().AlignCenter().Text($"RNC: {User.RucNumber}");
                }

                column.Item().PaddingVertical(8).Text("***********************************************************").AlignCenter();

                column.Item().AlignCenter().Text(SITRA.Core.Helpers.Utilities.InvoiceReceiptTitle).SemiBold();
                column.Item().AlignCenter().Text($"e-NCF: {Invoice.FiscalSupportNumber ?? "-"}").SemiBold();

                column.Item().PaddingVertical(8).Text("***********************************************************").AlignCenter();
            });
        }

        protected override void BuildContent(IContainer container)
        {
            container.Column(column =>
            {

                // Date
                column.Item().PaddingBottom(5).Text($"{SITRA.Core.Helpers.Utilities.DateLabel} Emisión: {Invoice.Date.ToString("dd-MM-yyyy")}");

                column.Item().PaddingVertical(5).Text("----------------------------------------------------------------").AlignCenter();

                // Items Table Headers
                column.Item().PaddingVertical(2).Column(headerCol =>
                {
                    headerCol.Item().Row(row =>
                    {
                        row.ConstantItem(45).Text(SITRA.Core.Helpers.Utilities.QuantityLabel);
                        row.RelativeItem().Text(SITRA.Core.Helpers.Utilities.DescriptionLabel);
                        row.ConstantItem(60).AlignRight().Text("Unidad de Medida");
                    });
                    
                    headerCol.Item().Row(row =>
                    {
                        row.ConstantItem(45).Text(SITRA.Core.Helpers.Utilities.PriceLabel);
                        row.RelativeItem().Text(SITRA.Core.Helpers.Utilities.ItbisLabel);
                        row.ConstantItem(60).AlignRight().Text(SITRA.Core.Helpers.Utilities.ValueLabel);
                    });
                });

                column.Item().PaddingVertical(5).Text("----------------------------------------------------------------").AlignCenter();
                column.Item().PaddingVertical(2);

                foreach (var item in Invoice.InvoiceItems)
                {
                    var taxAmount = item.Item.ItemTaxes.Sum(t => item.Qty * item.BasePrice * t.Tax.Amount / 100);

                    column.Item().PaddingBottom(2).Column(itemCol =>
                    {
                        itemCol.Item().Row(row =>
                        {
                            row.ConstantItem(45).Text(item.Qty.ToString());
                            row.RelativeItem().Text(item.Item.Name);
                            // CAJ commented out
                            // row.ConstantItem(60).AlignRight().Text("CAJ");
                        });

                        itemCol.Item().Row(row =>
                        {
                            row.ConstantItem(45).Text($"{item.BasePrice:N2}");
                            row.RelativeItem().Text($"{taxAmount:N2}");
                            row.ConstantItem(60).AlignRight().Text($"{item.TotalPrice:N2}");
                        });
                    });
                }

                column.Item().PaddingVertical(5).Text("----------------------------------------------------------------").AlignCenter();

                // Totals
                column.Item().AlignRight().Column(totalCol =>
                {
                    totalCol.Item().Row(r =>
                    {
                        r.RelativeItem().PaddingRight(5).AlignRight().Text(SITRA.Core.Helpers.Utilities.TaxedSubtotalLabel);
                        r.ConstantItem(70).AlignRight().Text($"{Invoice.TotalAmount:N2}");
                    });

                    totalCol.Item().Row(r =>
                    {
                        r.RelativeItem().PaddingRight(5).AlignRight().Text($"Total {SITRA.Core.Helpers.Utilities.ItbisLabel}:");
                        r.ConstantItem(70).AlignRight().Text($"{Invoice.TotalTaxes:N2}");
                    });

                    totalCol.Item().Row(r =>
                    {
                        r.RelativeItem().PaddingRight(5).AlignRight().Text($"{SITRA.Core.Helpers.Utilities.TotalLabel}:");
                        r.ConstantItem(70).AlignRight().Text($"{Invoice.TotalPrice:N2}");
                    });
                });

                column.Item().PaddingVertical(5).Text("----------------------------------------------------------------").AlignCenter();

                // QR Code Section
                if (_dgiiMetadata != null)
                {
                    column.Item().PaddingTop(10).AlignCenter().Column(qrCol =>
                    {
                        try
                        {
                            string qrUrl = SITRA.Core.Helpers.DgiiQrUrlHelper.BuildDgiiQrUrl(_dgiiMetadata, _dgiiSettings?.ConsultaTimbreUrl, _dgiiSettings?.ConsultaTimbreFCUrl);
                            byte[] qrBytes = SITRA.Core.Helpers.QrCodeGeneratorService.GenerateQrCodeBytes(qrUrl);
                            qrCol.Item().AlignCenter().Width(80).Height(80).Image(qrBytes, ImageScaling.FitArea);

                            qrCol.Item().AlignCenter().Text(x =>
                            {
                                x.Span("Código de Seguridad: ").FontSize(8);
                                x.Span(_dgiiMetadata.SecurityCode ?? "N/A").FontSize(8);
                            });

                            if (!string.IsNullOrEmpty(_dgiiMetadata.FechaFirma))
                            {
                                qrCol.Item().AlignCenter().Text(x =>
                                {
                                    x.Span("Fecha Firma: ").FontSize(8);
                                    x.Span(_dgiiMetadata.FechaFirma).FontSize(8);
                                });
                            }
                        }
                        catch { }
                    });
                }

                // Disclaimer
                // column.Item().PaddingTop(15).Text("*Este modelo de RI es exclusivo para fines ilustrativos.").FontSize(7).AlignCenter();

                // Description
                if (!string.IsNullOrWhiteSpace(Invoice.Description))
                {
                    column.Item().PaddingVertical(5).LineHorizontal(0.5f);
                    column.Item().AlignCenter().Text(Invoice.Description);
                }
            });
        }

        protected override void BuildFooter(IContainer container)
        {
            container.Component(new ThermalFooterComponent(SITRA.Core.Helpers.Utilities.ReceiptFooterMessage));
        }
    }
}
