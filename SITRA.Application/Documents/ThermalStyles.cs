using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace SITRA.Core.Documents
{
    public static class ThermalStyles
    {
        public static float ThermalWidth = 226.77f; // 80mm
        public static string DefaultFont = "Arial";

        public static TextStyle HeaderStyle => TextStyle.Default.FontSize(10).SemiBold().FontFamily(DefaultFont);
        public static TextStyle ContentStyle => TextStyle.Default.FontSize(9).FontFamily(DefaultFont);
        public static TextStyle SubtitleStyle => TextStyle.Default.FontSize(8).Italic().FontFamily(DefaultFont);
        public static TextStyle SmallStyle => TextStyle.Default.FontSize(7).Italic().FontFamily(DefaultFont);

        public static IContainer CellStyleHeader(IContainer container)
            => container.DefaultTextStyle(x => x.SemiBold())
                        .PaddingVertical(1)
                        .BorderBottom(1)
                        .BorderColor(Colors.Black);

        public static IContainer CellStyleRow(IContainer container)
            => container.BorderBottom(1)
                        .BorderColor(Colors.Grey.Lighten2)
                        .PaddingVertical(1);
    }
}
