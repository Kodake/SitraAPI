using System.Text;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Options;
using SITRA.Core.DTOs.Invoices.PurchaseInvoices;
using SITRA.Core.Interfaces;
using SITRA.Core.Mediator.Commands.Invoices.PurchaseInvoices;
using SITRA.Core.Models.Dgii;

namespace SITRA.Core.Mediator.Handlers.Invoices
{
    public class CreatePurchaseInvoiceCommandHandler : IRequestHandler<CreatePurchaseInvoiceCommand, CreatePurchaseInvoiceDTO>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOutputCacheStore _outputCacheStore;
        private readonly IMapper _mapper;
        private readonly IDgiiApiService _dgiiApiService;
        private readonly ISignatureService _signatureService;
        private readonly IEcfXmlGenerator _ecfXmlGenerator;
        private readonly IInvoiceToEcfMapper _mapperToEcf;
        private readonly DgiiSettings _dgiiSettings;

        public CreatePurchaseInvoiceCommandHandler(
            IUnitOfWork unitOfWork,
            IOutputCacheStore outputCacheStore,
            IMapper mapper,
            IDgiiApiService dgiiApiService,
            ISignatureService signatureService,
            IEcfXmlGenerator ecfXmlGenerator,
            IInvoiceToEcfMapper mapperToEcf,
            IOptions<DgiiSettings> dgiiSettings)
        {
            _unitOfWork = unitOfWork;
            _outputCacheStore = outputCacheStore;
            _mapper = mapper;
            _dgiiApiService = dgiiApiService;
            _signatureService = signatureService;
            _ecfXmlGenerator = ecfXmlGenerator;
            _mapperToEcf = mapperToEcf;
            _dgiiSettings = dgiiSettings.Value;
        }

        public async Task<CreatePurchaseInvoiceDTO> Handle(CreatePurchaseInvoiceCommand request, CancellationToken cancellationToken)
        {
            // 1. Execute original database persistence and logic
            var dto = await request.ExecuteAction(_unitOfWork, _outputCacheStore, _mapper, cancellationToken);

            if (dto == null)
                return null!;

            // 2. Perform DGII Transmission Workflow for Purchase ECFs (E31 and E41)
            try
            {
                var invoice = await _unitOfWork.PurchaseInvoiceRepository.GetByIdAsync(dto.Id, cancellationToken);
                if (invoice == null || string.IsNullOrEmpty(invoice.FiscalSupportNumber))
                    return dto;

                // Only process E41 fiscal documents
                string tipoEcf = invoice.FiscalSupportNumber.Length >= 3
                    ? invoice.FiscalSupportNumber.Substring(1, 2)
                    : string.Empty;

                // Process E31 (Factura de Crédito Fiscal — used for formal supplier invoices)
                // and E41 (Comprobante de Compras — standard purchase receipt).
                // Both represent inbound purchase transactions that increment stock.
                var validPurchaseTypes = new HashSet<string> { "31", "41" };
                if (!validPurchaseTypes.Contains(tipoEcf))
                    return dto;

                var ecfDocument = _mapperToEcf.MapToDgiiDocument(invoice);

                string seedXml = await _dgiiApiService.GetSemillaAsync();
                string signedSeed = _signatureService.SignAuthenticationSeed(seedXml, _dgiiSettings.CertificatePath, _dgiiSettings.CertificatePassword);
                string token = await _dgiiApiService.GetTokenAsync(signedSeed);

                if (!string.IsNullOrEmpty(token))
                {
                    // E31 and E41 purchase invoices always use the standard eCF channel —
                    // no RFCE / B2C redirect applies to purchase invoices.
                    string xmlSinFirma = _ecfXmlGenerator.GenerateEcfXml(ecfDocument);
                    string xmlFirmado = _signatureService.SignXml(xmlSinFirma, _dgiiSettings.CertificatePath, _dgiiSettings.CertificatePassword);
                    string securityCode = GetSecurityCodeFromSignedXml(xmlFirmado);

                    SaveDocumentXml(
                        xmlFirmado,
                        ecfDocument.Encabezado?.IdDoc?.ENCF ?? "",
                        ecfDocument.Encabezado?.Emisor?.RNCEmisor ?? "",
                        tipoEcf,
                        _dgiiSettings.StoragePath);

                    string rawResponse = await _dgiiApiService.SendEcfAsync(xmlFirmado, token);

                    string extractedTrackId = rawResponse;
                    try
                    {
                        var jsonDoc = System.Text.Json.JsonDocument.Parse(rawResponse);
                        if (jsonDoc.RootElement.TryGetProperty("trackId", out var trackIdElement))
                        {
                            extractedTrackId = trackIdElement.GetString() ?? rawResponse;
                        }
                    }
                    catch
                    {
                        // If JSON parsing fails, keep raw response as a safety fallback
                    }

                    invoice.DgiiTrackId = extractedTrackId;
                    invoice.DgiiStatus = "Enviado";
                    invoice.SecurityCode = securityCode;

                    _unitOfWork.PurchaseInvoiceRepository.Update(invoice);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }
            }
            catch (Exception ex)
            {
                // TODO: Handle exception (e.g., log failure, mark as Pending DGII)
                System.Diagnostics.Debug.WriteLine($"DGII Purchase Flow Error: {ex.Message}");
            }

            return dto;
        }

        private static string GetSecurityCodeFromSignedXml(string signedXml)
        {
            try
            {
                var doc = System.Xml.Linq.XDocument.Parse(signedXml);
                System.Xml.Linq.XNamespace ds = "http://www.w3.org/2000/09/xmldsig#";
                var sigValue = doc.Descendants(ds + "SignatureValue").FirstOrDefault()?.Value;

                if (!string.IsNullOrEmpty(sigValue))
                {
                    sigValue = sigValue.Replace("\n", "").Replace("\r", "").Replace(" ", "");
                    return sigValue.Length >= 6 ? sigValue.Substring(0, 6) : sigValue;
                }
            }
            catch { }
            return "000000";
        }

        private static void SaveDocumentXml(string xml, string encf, string rnc, string docType, string? storagePath)
        {
            try
            {
                string baseDir = !string.IsNullOrEmpty(storagePath)
                    ? storagePath
                    : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "DgiiDocuments");

                string saveDir = Path.Combine(baseDir, rnc, docType);

                if (!Directory.Exists(saveDir))
                {
                    Directory.CreateDirectory(saveDir);
                }

                string fileName = $"{rnc}{encf}.xml";
                string filePath = Path.Combine(saveDir, fileName);
                File.WriteAllText(filePath, xml, new UTF8Encoding(false));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to save DGII E41 XML: {ex.Message}");
            }
        }
    }
}
