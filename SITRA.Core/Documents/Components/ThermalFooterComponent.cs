using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace SITRA.Core.Documents.Components
{
    public class ThermalFooterComponent : IComponent
    {
        private readonly string? _customMessage;

        public ThermalFooterComponent(string? customMessage = null)
        {
            _customMessage = customMessage;
        }

        public void Compose(IContainer container)
        {
            container.Column(column =>
            {
                column.Item().PaddingTop(5).LineHorizontal(0.8f);

                if (!string.IsNullOrEmpty(_customMessage))
                {
                    column.Item()
                          .PaddingTop(4)
                          .AlignCenter()
                          .Text(_customMessage)
                          .Style(ThermalStyles.SmallStyle);
                }

                column.Item().AlignCenter().Text(x =>
                {
                    x.CurrentPageNumber();
                    x.Span(" / ");
                    x.TotalPages();
                });
            });
        }
    }
}
