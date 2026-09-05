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

            using var httpClient = new HttpClient();
            try
            {
                var xmlContent = await httpClient.GetStringAsync(logInvoice.XmlUrl, cancellationToken);
                return xmlContent;
            }
            catch (Exception ex)
            {
                return $"Error fetching XML from server: {ex.Message}";
            }
        }
    }
}
