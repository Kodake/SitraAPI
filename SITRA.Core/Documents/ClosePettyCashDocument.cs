using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using SITRA.Core.Documents.Components;
using SITRA.Core.DTOs.CashBoxes;
using System;
using System.IO;
using System.Linq;

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
            container.Column(column =>
            {
                string logoPath = Path.Combine(AppContext.BaseDirectory, "Resources", "logo-black.png");
                if (File.Exists(logoPath))
                {
                    column.Item().AlignCenter().PaddingBottom(10).Height(60).Image(logoPath).FitArea();
                }

                var user = PettyCash.User;
                if (user != null)
                {
                    column.Item().AlignCenter().Text(user.CompanyName ?? "Mi Empresa").SemiBold();
                    if (!string.IsNullOrEmpty(user.Address))
                        column.Item().AlignCenter().Text($"Dirección: {user.Address}");
                    if (!string.IsNullOrEmpty(user.RucNumber))
                        column.Item().AlignCenter().Text($"RNC: {user.RucNumber}");
                }

                column.Item().PaddingVertical(8).Text("***********************************************************").AlignCenter();

                column.Item().AlignCenter().Text(SITRA.Core.Helpers.Utilities.ClosePettyCashTitle).SemiBold();
                string cashier = user?.Email ?? PettyCash.UserId;
                column.Item().AlignCenter().Text($"{SITRA.Core.Helpers.Utilities.CashierLabel}: {cashier}").SemiBold();

                column.Item().PaddingVertical(8).Text("***********************************************************").AlignCenter();
            });
        }

        protected override void BuildContent(IContainer container)
        {
            container.Column(column =>
            {
                column.Item().PaddingBottom(5).Text($"{SITRA.Core.Helpers.Utilities.OpeningDateLabel}: {PettyCash.OpeningDate.ToString("dd-MM-yyyy HH:mm")}");
                
                if (PettyCash.ClosingDate.HasValue)
                {
                    column.Item().PaddingBottom(5).Text($"{SITRA.Core.Helpers.Utilities.ClosingDateLabel}: {PettyCash.ClosingDate.Value.ToString("dd-MM-yyyy HH:mm")}");
                }

                column.Item().PaddingVertical(5).Text("----------------------------------------------------------------").AlignCenter();

                column.Item().Row(row =>
                {
                    row.RelativeItem().Text(SITRA.Core.Helpers.Utilities.OpeningAmountLabel);
                    row.ConstantItem(70).AlignRight().Text($"{PettyCash.OpeningAmount:N2}");
                });

                column.Item().Row(row =>
                {
                    row.RelativeItem().Text(SITRA.Core.Helpers.Utilities.BilledAmountLabel);
                    row.ConstantItem(70).AlignRight().Text($"{PettyCash.TotalBilled:N2}");
                });

                column.Item().Row(row =>
                {
                    row.RelativeItem().Text(SITRA.Core.Helpers.Utilities.WithdrawalsLabel);
                    row.ConstantItem(70).AlignRight().Text($"{PettyCash.WithdrawalAmount:N2}");
                });

                column.Item().PaddingVertical(5).Text("----------------------------------------------------------------").AlignCenter();

                if (PettyCash.PaymentMethods != null && PettyCash.PaymentMethods.Any())
                {
                    column.Item().PaddingVertical(2).Text(SITRA.Core.Helpers.Utilities.SummaryByMethodLabel).SemiBold();
                    foreach (var method in PettyCash.PaymentMethods)
                    {
                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Text(SITRA.Core.Helpers.Utilities.GetPaymentMethodLabel(method.MethodType));
                            row.ConstantItem(70).AlignRight().Text($"{method.Amount:N2}");
                        });
                    }
                    column.Item().PaddingVertical(5).Text("----------------------------------------------------------------").AlignCenter();
                }

                column.Item().AlignRight().Column(totalCol =>
                {
                    var expectedCash = PettyCash.OpeningAmount + PettyCash.CashBilled - PettyCash.WithdrawalAmount;
                    
                    totalCol.Item().Row(r =>
                    {
                        r.RelativeItem().PaddingRight(5).AlignRight().Text(SITRA.Core.Helpers.Utilities.ExpectedCashLabel).SemiBold();
                        r.ConstantItem(70).AlignRight().Text($"{expectedCash:N2}").SemiBold();
                    });

                    totalCol.Item().Row(r =>
                    {
                        r.RelativeItem().PaddingRight(5).AlignRight().Text(SITRA.Core.Helpers.Utilities.ClosingAmountLabel).SemiBold();
                        r.ConstantItem(70).AlignRight().Text($"{PettyCash.ClosingAmount:N2}").SemiBold();
                    });

                    var diff = PettyCash.ClosingAmount - expectedCash;
                    totalCol.Item().Row(r =>
                    {
                        r.RelativeItem().PaddingRight(5).AlignRight().Text(SITRA.Core.Helpers.Utilities.DifferenceLabel);
                        r.ConstantItem(70).AlignRight().Text($"{diff:N2}");
                    });
                });

            });
        }

        protected override void BuildFooter(IContainer container)
        {
            container.Component(new ThermalFooterComponent(SITRA.Core.Helpers.Utilities.CashierSignatureLabel));
        }
    }
}
