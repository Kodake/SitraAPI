using SITRA.Core.Entities.Invoices;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using SITRA.Core.Documents.Components;

namespace SITRA.Core.Documents
{
    public class InvoiceDocument : ThermalDocumentBase
    {
        public Invoice Invoice { get; }

        public InvoiceDocument(Invoice invoice)
        {
            Invoice = invoice;
        }

        protected override void BuildHeader(IContainer container)
        {
            // Note: InvoiceDocument usually doesn't have the full company user passed in the same way as Receipt,
            // but we can pass null or find a way to get it if needed. 
            // For now, using the title.
            container.Component(new ThermalHeaderComponent(null, SITRA.Core.Helpers.Utilities.InvoiceTitle));
        }

        protected override void BuildContent(IContainer container)
        {
            container.Column(column =>
            {
                column.Spacing(2);

                if (!string.IsNullOrEmpty(Invoice.InvoiceBuyer?.Buyer?.Identification))
                    column.Item().Text($"{SITRA.Core.Helpers.Utilities.IdentificationLabel}: {Invoice.InvoiceBuyer?.Buyer.Identification}");

                column.Item().Text($"{SITRA.Core.Helpers.Utilities.DateLabel}: {Invoice.Date:dd-MM-yyyy}");
                column.Item().Text($"{SITRA.Core.Helpers.Utilities.NameLabel}: {Invoice.InvoiceBuyer?.Buyer?.Name ?? "N/A"}");

                column.Item().PaddingVertical(5).LineHorizontal(0.8f);

                column.Item().Text($"{SITRA.Core.Helpers.Utilities.NumberLabel} {Invoice.Id}").SemiBold();
                column.Item().Text(SITRA.Core.Helpers.Utilities.FiscalSupportLabel).SemiBold();
                column.Item().Text(string.IsNullOrEmpty(Invoice.FiscalSupportNumber) ? "N/A" : Invoice.FiscalSupportNumber).SemiBold();

                if (Invoice.DueDate.HasValue)
                    column.Item().Text($"{SITRA.Core.Helpers.Utilities.DueDateLabel}: {Invoice.DueDate:dd-MM-yyyy}");

                column.Item().PaddingVertical(5).LineHorizontal(0.8f);

                // Table
                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(column =>
                    {
                        column.RelativeColumn(3);
                        column.RelativeColumn();
                        column.RelativeColumn();
                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(ThermalStyles.CellStyleHeader).Text(SITRA.Core.Helpers.Utilities.DescriptionLabel);
                        header.Cell().Element(ThermalStyles.CellStyleHeader).AlignRight().Text(SITRA.Core.Helpers.Utilities.QuantityLabel);
                        header.Cell().Element(ThermalStyles.CellStyleHeader).AlignRight().Text(SITRA.Core.Helpers.Utilities.TotalLabel);
                    });

                    foreach (var item in Invoice.InvoiceItems)
                    {
                        table.Cell().Element(ThermalStyles.CellStyleRow).Text(item.Item.Name);
                        table.Cell().Element(ThermalStyles.CellStyleRow).AlignRight().Text(item.Qty.ToString());
                        table.Cell().Element(ThermalStyles.CellStyleRow).AlignRight().Text($"{item.TotalPrice:N2}");
                    }
                });

                column.Item().PaddingVertical(5).AlignRight().Column(totalCol =>
                {
                    totalCol.Item().Text($"{SITRA.Core.Helpers.Utilities.SubtotalLabel}: {Invoice.TotalAmount:N2}");
                    totalCol.Item().Text($"{SITRA.Core.Helpers.Utilities.TaxesLabel}: {Invoice.TotalTaxes:N2}");
                    totalCol.Item().Text($"{SITRA.Core.Helpers.Utilities.TotalLabel}: {Invoice.TotalPrice:N2}").SemiBold();
                });
            });
        }

        protected override void BuildFooter(IContainer container)
        {
            container.Component(new ThermalFooterComponent());
        }
    }
}
