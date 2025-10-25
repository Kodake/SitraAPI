using BackEnd.DTOs.CashBoxes;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace BackEnd.DTOs
{
    public class ClosePettyCashDTO : IDocument
    {
        public ClosePettyCashDTO(PettyCashDTO pettyCash)
        {
            PettyCash = pettyCash;
        }

        public PettyCashDTO PettyCash { get; }

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.ContinuousSize(226.77f);
                page.Margin(10);
                page.DefaultTextStyle(x => x.FontSize(9).FontFamily("Arial"));
                page.Content().Element(BuildContent);
            });
        }

        private void BuildContent(IContainer container)
        {
            container.Column(column =>
            {
                column.Spacing(2);

                var logoPath = Path.Combine(AppContext.BaseDirectory, "Resources", "logo-black.png");

                // Logo
                if (File.Exists(logoPath))
                {
                    column.Item().AlignCenter().MaxHeight(1.5f, Unit.Centimetre)
                          .Width(100).Image(logoPath).FitWidth();
                }
                else
                {
                    column.Item().AlignCenter().Text("LOGO NO DISPONIBLE")
                          .FontSize(8).Italic();
                }

                column.Item().Text(" ");
                column.Item().AlignCenter().Text("CIERRE DE CAJA CHICA").SemiBold();
                column.Item().AlignCenter().Text($"FECHA: {PettyCash.ClosingDate:dd-MM-yyyy}");
                column.Item().Text(" ");
                column.Item().Text(" ");

                // Detalle de caja
                column.Item().Text($"Monto de Apertura: {PettyCash.OpeningAmount:N}");
                column.Item().Text($"Retiro de Caja: {PettyCash.WithdrawalAmount:N}");
                column.Item().Text($"Venta en Efectivo: {PettyCash.CashBilled:N}");
                column.Item().Text($"Efectivo en Caja: {PettyCash.ClosingAmount:N}");
                column.Item().Text($"Venta Total: {PettyCash.TotalBilled:N}").SemiBold();

                if (!string.IsNullOrWhiteSpace(PettyCash.Comments))
                {
                    column.Item().Text(" ");
                    column.Item().Text($"Comentarios: {PettyCash.Comments}");
                }

                column.Item().Text(" ");
                column.Item().AlignCenter().Text("¡Cierre realizado con éxito!").Italic();
            });
        }

        public void GeneratePdf(string filePath)
        {
            Document.Create(container => Compose(container)).GeneratePdf(filePath);
        }

        public byte[] GenerateToBytes()
        {
            return Document.Create(container => Compose(container)).GeneratePdf();
        }
    }
}
