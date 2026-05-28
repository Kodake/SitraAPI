using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using SITRA.Core.Entities.Auths;
using System.IO;
using System;

namespace SITRA.Core.Documents.Components
{
    public class ThermalHeaderComponent : IComponent
    {
        private readonly User? _companyUser;
        private readonly string _title;

        public ThermalHeaderComponent(User? companyUser, string title)
        {
            _companyUser = companyUser;
            _title = title;
        }

        public void Compose(IContainer container)
        {
            container.Column(column =>
            {
                string logoPath = Path.Combine(AppContext.BaseDirectory, "Resources", "logo-black.png");
                if (File.Exists(logoPath))
                {
                    column.Item()
                          .AlignCenter()
                          .PaddingBottom(5)
                          .MaxHeight(1.5f, Unit.Centimetre)
                          .Width(100)
                          .Image(logoPath).FitArea();
                }

                // Company Info
                if (_companyUser != null)
                {
                    column.Item().AlignCenter().Text(_companyUser.CompanyName).Style(ThermalStyles.HeaderStyle);

                    if (!string.IsNullOrEmpty(_companyUser.Email))
                        column.Item().AlignCenter().Text(_companyUser.Email).Style(ThermalStyles.ContentStyle);

                    column.Item().Row(row =>
                    {
                        row.RelativeItem().Text(SITRA.Core.Helpers.Utilities.PhoneLabel).SemiBold();
                        row.RelativeItem().AlignRight().Text(_companyUser.PhoneNumber ?? "-");
                    });

                    column.Item().Row(row =>
                    {
                        row.RelativeItem().Text(SITRA.Core.Helpers.Utilities.RncLabel).SemiBold();
                        row.RelativeItem().AlignRight().Text(_companyUser.RucNumber ?? "-");
                    });

                    if (!string.IsNullOrEmpty(_companyUser.Address))
                        column.Item().AlignCenter().Text(_companyUser.Address).Style(ThermalStyles.SmallStyle);
                }

                column.Item().PaddingVertical(5).LineHorizontal(0.8f);
                column.Item().AlignCenter().Text(_title).Style(ThermalStyles.HeaderStyle);
                column.Item().PaddingVertical(5).LineHorizontal(0.8f);
            });
        }
    }
}
