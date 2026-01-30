using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace SITRA.Core.Documents
{
    public abstract class ThermalDocumentBase : IDocument
    {
        public virtual DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.ContinuousSize(ThermalStyles.ThermalWidth);
                page.Margin(10);
                page.DefaultTextStyle(ThermalStyles.ContentStyle);

                page.Header().Element(BuildHeader);
                page.Content().Element(BuildContent);
                page.Footer().Element(BuildFooter);
            });
        }

        protected abstract void BuildHeader(IContainer container);
        protected abstract void BuildContent(IContainer container);
        protected abstract void BuildFooter(IContainer container);

        public byte[] GenerateToBytes()
        {
            return Document.Create(container => Compose(container)).GeneratePdf();
        }

        public void GeneratePdf(string filePath)
        {
            Document.Create(container => Compose(container)).GeneratePdf(filePath);
        }
    }
}
