using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using SITRA.Core.Documents.Components;
using SITRA.Core.Entities.Invoices;

namespace SITRA.Core.Documents
{
    public class PurchaseInvoiceReportDocument : ThermalDocumentBase
    {
        public List<Invoice> Invoices { get; }
        public DateTime From { get; }
        public DateTime To { get; }

        public PurchaseInvoiceReportDocument(List<Invoice> invoices, DateTime from, DateTime to)
        {
            Invoices = invoices;
            From = from;
            To = to;
        }

        public override DocumentMetadata GetMetadata()
        {
            return new DocumentMetadata
            {
                Title = $"Reporte de Compras ({From:dd-MM-yyyy} - {To:dd-MM-yyyy})",
                Subject = "Reporte de Compras",
                Author = "SITRA"
            };
        }

        protected override void BuildHeader(IContainer container)
        {
            container.Component(new ThermalHeaderComponent(null, SITRA.Core.Helpers.Utilities.PurchasesReportTitle));
        }

        protected override void BuildContent(IContainer container)
        {
            container.Column(column =>
            {
                column.Spacing(2);

                column.Item().Text($"{SITRA.Core.Helpers.Utilities.FromLabel} {From:dd-MM-yyyy}");
                column.Item().Text($"{SITRA.Core.Helpers.Utilities.ToLabel} {To:dd-MM-yyyy}");

                column.Item().PaddingVertical(5).LineHorizontal(0.5f);

                column.Item().Row(row =>
                {
                    row.RelativeItem().Text(SITRA.Core.Helpers.Utilities.DateLabel).SemiBold();
                    row.RelativeItem().AlignRight().Text(SITRA.Core.Helpers.Utilities.TotalLabel).SemiBold();
                });

                column.Item().PaddingVertical(5).LineHorizontal(0.5f);

                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(colDef =>
                    {
                        colDef.RelativeColumn(); // FECHA
                        colDef.RelativeColumn(); // TOTAL
                    });

                    foreach (var item in Invoices)
                    {
                        table.Cell().Element(ThermalStyles.CellStyleRow).Text(item.CreatedAt.ToString("dd/MM/yyyy"));
                        table.Cell().Element(ThermalStyles.CellStyleRow).AlignRight().Text($"{item.TotalPrice:N2}");
                    }
                });

                column.Item().PaddingVertical(5).AlignRight().Column(totals =>
                {
                    totals.Item().Text($"{SITRA.Core.Helpers.Utilities.SubtotalLabel}: {Invoices.Sum(x => x.TotalAmount):N2}");
                    totals.Item().Text($"{SITRA.Core.Helpers.Utilities.TaxesLabel}: {Invoices.Sum(x => x.TotalTaxes):N2}");
                    totals.Item().Text($"{SITRA.Core.Helpers.Utilities.TotalLabel}: {Invoices.Sum(x => x.TotalPrice):N2}").SemiBold();
                });
            });
        }

        protected override void BuildFooter(IContainer container)
        {
            container.Component(new ThermalFooterComponent());
        }
    }
}
