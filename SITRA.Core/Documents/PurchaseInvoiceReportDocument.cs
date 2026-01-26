using SITRA.Core.Entities.Invoices;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using SITRA.Core.Documents.Components;

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

        protected override void BuildHeader(IContainer container)
        {
            container.Component(new ThermalHeaderComponent(null, SITRA.Core.Helpers.Utilities.PurchasesReportTitle));
        }

        protected override void BuildContent(IContainer container)
        {
            container.Column(column =>
            {
                column.Item().Text($"{SITRA.Core.Helpers.Utilities.FromLabel} {From:dd-MM-yyyy}");
                column.Item().Text($"{SITRA.Core.Helpers.Utilities.ToLabel} {To:dd-MM-yyyy}");
                column.Item().PaddingVertical(5);

                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(colDef =>
                    {
                        colDef.RelativeColumn(); // FECHA
                        colDef.RelativeColumn(); // TOTAL
                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(ThermalStyles.CellStyleHeader).Text(SITRA.Core.Helpers.Utilities.DateLabel);
                        header.Cell().Element(ThermalStyles.CellStyleHeader).AlignRight().Text(SITRA.Core.Helpers.Utilities.TotalLabel);
                    });

                    foreach (var item in Invoices)
                    {
                        table.Cell().Element(ThermalStyles.CellStyleRow).Text(item.CreatedAt.ToString("dd/MM/yyyy"));
                        table.Cell().Element(ThermalStyles.CellStyleRow).AlignRight().Text($"{item.TotalPrice:N2}");
                    }
                });

                column.Item().PaddingTop(10).AlignRight().Column(totals =>
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
