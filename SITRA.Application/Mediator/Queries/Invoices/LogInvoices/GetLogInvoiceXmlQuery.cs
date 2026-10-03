using AutoMapper;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.Invoices.LogInvoices
{
    public class GetLogInvoiceXmlQuery(Guid id) : GetByIdQuery<string>(id)
    {
        public override async Task<string> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var logInvoice = await unitOfWork.LogInvoiceRepository.GetByIdAsync(Id);
            if (logInvoice == null || string.IsNullOrEmpty(logInvoice.XmlUrl))
            {
                return string.Empty;
            }

            try
            {
                var xmlContent = await unitOfWork.DgiiStorageService.ReadDocumentXmlFromUrlAsync(logInvoice.XmlUrl);
                if (!string.IsNullOrEmpty(xmlContent))
                {
                    return xmlContent;
                }

                return "Error fetching XML from server: No se pudo obtener el contenido del XML desde el almacenamiento.";
            }
            catch (Exception ex)
            {
                return $"Error fetching XML from server: {ex.Message}";
            }
        }
    }
}
