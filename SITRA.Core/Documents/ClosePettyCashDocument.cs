using SITRA.Core.DTOs.CashBoxes;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using SITRA.Core.Documents.Components;

namespace SITRA.Core.Documents
{
    public class ClosePettyCashDocument : ThermalDocumentBase
    {
        public PettyCashDTO PettyCash { get; }

        public ClosePettyCashDocument(PettyCashDTO pettyCash)
        {
            PettyCash = pettyCash;
        }

        protected override void BuildHeader(IContainer container)
        {
            // Mapping UserDTO to User entity loosely or just using title
            container.Component(new ThermalHeaderComponent(null, SITRA.Core.Helpers.Utilities.ClosePettyCashTitle));
        }

        protected override void BuildContent(IContainer container)
        {
            container.Column(column =>
            {
                column.Spacing(2);

                column.Item().Row(row =>
                {
                    row.RelativeItem().Text(SITRA.Core.Helpers.Utilities.CashierLabel);
                    row.RelativeItem().AlignRight().Text(PettyCash.User?.Email ?? PettyCash.UserId);
                });

                column.Item().Row(row =>
                {
                    row.RelativeItem().Text(SITRA.Core.Helpers.Utilities.OpeningDateLabel);
                    row.RelativeItem().AlignRight().Text(PettyCash.OpeningDate.ToString("dd/MM/yyyy HH:mm"));
                });

                if (PettyCash.ClosingDate.HasValue)
                {
                    column.Item().Row(row =>
                    {
                        row.RelativeItem().Text(SITRA.Core.Helpers.Utilities.ClosingDateLabel);
                        row.RelativeItem().AlignRight().Text(PettyCash.ClosingDate.Value.ToString("dd/MM/yyyy HH:mm"));
                    });
                }

                column.Item().PaddingVertical(5).LineHorizontal(0.8f);

                column.Item().Row(row =>
                {
                    row.RelativeItem().Text(SITRA.Core.Helpers.Utilities.OpeningAmountLabel);
                    row.RelativeItem().AlignRight().Text($"{PettyCash.OpeningAmount:N2}");
                });

                column.Item().Row(row =>
                {
                    row.RelativeItem().Text(SITRA.Core.Helpers.Utilities.BilledAmountLabel);
                    row.RelativeItem().AlignRight().Text($"{PettyCash.TotalBilled:N2}");
                });

                column.Item().Row(row =>
                {
                    row.RelativeItem().Text(SITRA.Core.Helpers.Utilities.WithdrawalsLabel);
                    row.RelativeItem().AlignRight().Text($"{PettyCash.WithdrawalAmount:N2}");
                });

                column.Item().PaddingVertical(5).LineHorizontal(0.8f);

                if (PettyCash.PaymentMethods.Any())
                {
                    column.Item().Text(SITRA.Core.Helpers.Utilities.SummaryByMethodLabel).SemiBold();
                    foreach (var method in PettyCash.PaymentMethods)
                    {
                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Text(SITRA.Core.Helpers.Utilities.GetPaymentMethodLabel(method.MethodType));
                            row.RelativeItem().AlignRight().Text($"{method.Amount:N2}");
                        });
                    }
                    column.Item().PaddingVertical(5).LineHorizontal(0.8f);
                }

                column.Item().Row(row =>
                {
                    row.RelativeItem().Text(SITRA.Core.Helpers.Utilities.ExpectedCashLabel).SemiBold();
                    var expectedCash = PettyCash.OpeningAmount + PettyCash.CashBilled - PettyCash.WithdrawalAmount;
                    row.RelativeItem().AlignRight().Text($"{expectedCash:N2}").SemiBold();
                });

                column.Item().Row(row =>
                {
                    row.RelativeItem().Text(SITRA.Core.Helpers.Utilities.ClosingAmountLabel).SemiBold();
                    row.RelativeItem().AlignRight().Text($"{PettyCash.ClosingAmount:N2}").SemiBold();
                });

                var diff = PettyCash.ClosingAmount - (PettyCash.OpeningAmount + PettyCash.CashBilled - PettyCash.WithdrawalAmount);
                column.Item().Row(row =>
                {
                    row.RelativeItem().Text(SITRA.Core.Helpers.Utilities.DifferenceLabel);
                    row.RelativeItem().AlignRight().Text($"{diff:N2}");
                });
            });
        }

        protected override void BuildFooter(IContainer container)
        {
            container.Component(new ThermalFooterComponent(SITRA.Core.Helpers.Utilities.CashierSignatureLabel));
        }
    }
}
