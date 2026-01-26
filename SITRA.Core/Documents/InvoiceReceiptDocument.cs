using SITRA.Core.Entities.Auths;
using SITRA.Core.Entities.Invoices;
using SITRA.Core.Enums;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using SITRA.Core.Documents.Components;

namespace SITRA.Core.Documents
{
    public class InvoiceReceiptDocument : ThermalDocumentBase
    {
        public Invoice Invoice { get; }
        public User User { get; }

        public InvoiceReceiptDocument(Invoice invoice, User user)
        {
            Invoice = invoice;
            User = user;
        }

        protected override void BuildHeader(IContainer container)
        {
            container.Component(new ThermalHeaderComponent(User, SITRA.Core.Helpers.Utilities.InvoiceReceiptTitle));
        }

        protected override void BuildContent(IContainer container)
        {
            container.Column(column =>
            {
                // Login/Session Info
                column.Item().Row(row =>
                {
                    row.RelativeItem().Text(SITRA.Core.Helpers.Utilities.UserLabel);
                    row.RelativeItem().AlignRight().Text(User.UserName);
                });

                column.Item().Row(row =>
                {
                    row.RelativeItem().Text(SITRA.Core.Helpers.Utilities.ConditionLabel);
                    row.RelativeItem().AlignRight().Text(Invoice.ConditionType == InvoiceCondition.Spot ? SITRA.Core.Helpers.Utilities.SpotConditionLabel : SITRA.Core.Helpers.Utilities.CreditConditionLabel);
                });

                column.Item().LineHorizontal(0.8f);

                // Invoice Main Details
                column.Item().Row(row =>
                {
                    row.RelativeItem().Text(SITRA.Core.Helpers.Utilities.InvoiceLabel);
                    row.RelativeItem().AlignRight().Text(Invoice.Id.ToString());
                });

                column.Item().Row(row =>
                {
                    row.RelativeItem().Text(SITRA.Core.Helpers.Utilities.DateLabel);
                    row.RelativeItem().AlignRight().Text(Invoice.Date.ToString("dd/MM/yyyy"));
                });

                column.Item().Row(row =>
                {
                    row.RelativeItem().Text(SITRA.Core.Helpers.Utilities.FiscalSupportNumberLabel);
                    row.RelativeItem().AlignRight().Text(Invoice.FiscalSupportNumber ?? "-");
                });

                column.Item().Row(row =>
                {
                    row.RelativeItem().Text(SITRA.Core.Helpers.Utilities.RncCedulaLabel);
                    row.RelativeItem().AlignRight().Text(Invoice.InvoiceBuyer?.Buyer?.Identification ?? "-");
                });

                column.Item().Row(row =>
                {
                    row.RelativeItem().Text(SITRA.Core.Helpers.Utilities.CustomerLabel);
                    row.RelativeItem().AlignRight().Text(Invoice.InvoiceBuyer?.Buyer?.Name ?? "-");
                });

                column.Item().PaddingVertical(5).LineHorizontal(0.8f);

                // Items Table
                column.Item().Row(row =>
                {
                    row.RelativeItem().Text(SITRA.Core.Helpers.Utilities.ItemsLabel);
                    row.ConstantItem(45).AlignRight().Text(SITRA.Core.Helpers.Utilities.TaxLabelShort);
                    row.ConstantItem(45).AlignRight().Text(SITRA.Core.Helpers.Utilities.TotalLabelShort);
                });

                column.Item().LineHorizontal(0.5f);

                foreach (var item in Invoice.InvoiceItems)
                {
                    var taxAmount = item.Item.ItemTaxes.Sum(t => item.Qty * item.BasePrice * t.Tax.Amount / 100);

                    column.Item().Row(row =>
                    {
                        row.ConstantItem(20).Text(item.Qty.ToString());
                        row.RelativeItem().Text(item.Item.Name);
                        row.ConstantItem(45).AlignRight().Text($"{taxAmount:N2}");
                        row.ConstantItem(45).AlignRight().Text($"{item.TotalPrice:N2}");
                    });

                    column.Item().PaddingLeft(20).Text($"{item.BasePrice:N2}").Style(ThermalStyles.SmallStyle);
                }

                column.Item().PaddingVertical(5).LineHorizontal(0.8f);

                // Totals
                column.Item().AlignRight().Column(totalCol =>
                {
                    totalCol.Item().Row(r =>
                    {
                        r.RelativeItem().Text($"{SITRA.Core.Helpers.Utilities.SubtotalLabel}:");
                        r.ConstantItem(60).AlignRight().Text($"{Invoice.TotalAmount:N2}");
                    });

                    totalCol.Item().Row(r =>
                    {
                        r.RelativeItem().Text(SITRA.Core.Helpers.Utilities.ItbisLabel);
                        r.ConstantItem(60).AlignRight().Text($"{Invoice.TotalTaxes:N2}");
                    });

                    totalCol.Item().Row(r =>
                    {
                        r.RelativeItem().Text(SITRA.Core.Helpers.Utilities.AmountToPayLabel).SemiBold();
                        r.ConstantItem(60).AlignRight().Text($"{Invoice.TotalPrice:N2}").SemiBold();
                    });
                });

                // Payment Methods
                if (Invoice.SpotPayment?.Payments != null && Invoice.SpotPayment.Payments.Any())
                {
                    column.Item().PaddingTop(5).Text(SITRA.Core.Helpers.Utilities.PaymentMethodsLabel).SemiBold();
                    foreach (var payment in Invoice.SpotPayment.Payments)
                    {
                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Text(SITRA.Core.Helpers.Utilities.GetPaymentMethodLabel(payment.PaymentMethod));
                            row.RelativeItem().AlignRight().Text($"{payment.Amount:N2}");
                        });
                    }

                    if (Invoice.SpotPayment.AmountReturned > 0)
                    {
                        column.Item().PaddingTop(2).Row(row =>
                        {
                            row.RelativeItem().Text(SITRA.Core.Helpers.Utilities.AmountReturnedLabel).SemiBold();
                            row.RelativeItem().AlignRight().Text($"{Invoice.SpotPayment.AmountReturned:N2}").SemiBold();
                        });
                    }
                }
            });
        }

        protected override void BuildFooter(IContainer container)
        {
            container.Component(new ThermalFooterComponent(SITRA.Core.Helpers.Utilities.ReceiptFooterMessage));
        }
    }
}
