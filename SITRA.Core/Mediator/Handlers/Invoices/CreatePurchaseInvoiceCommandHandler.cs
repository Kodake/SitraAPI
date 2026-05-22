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
            var (certBytes, certPassword) = await _unitOfWork.UserRoleService.GetTenantCertificateAsync(cancellationToken);

            request.DgiiProcess = async (invoice) => 
            {
                bool isAccepted = true;
                string trackId = "";
                string status = "Enviado";
                string securityCode = "000000";
                string rawResponse = "";

                try
                {
                    // Only process E41 fiscal documents (or E31)
                    string tipoEcf = invoice.FiscalSupportNumber?.Length >= 3
                        ? invoice.FiscalSupportNumber.Substring(1, 2)
                        : string.Empty;

                    var validPurchaseTypes = new HashSet<string> { "31", "41" };
                    if (!validPurchaseTypes.Contains(tipoEcf))
                        return (isAccepted, trackId, status, securityCode, rawResponse);

                    var ecfDocument = _mapperToEcf.MapToDgiiDocument(invoice);

                    string seedXml = await _dgiiApiService.GetSemillaAsync();
                    string signedSeed = _signatureService.SignAuthenticationSeed(seedXml, certBytes, certPassword);
                    string token = await _dgiiApiService.GetTokenAsync(signedSeed);

                    if (!string.IsNullOrEmpty(token))
                    {
                        // E31 and E41 purchase invoices always use the standard eCF channel
                        string xmlSinFirma = _ecfXmlGenerator.GenerateEcfXml(ecfDocument);
                        string xmlFirmado = _signatureService.SignXml(xmlSinFirma, certBytes, certPassword);
                        securityCode = GetSecurityCodeFromSignedXml(xmlFirmado);

                        SaveDocumentXml(
                            xmlFirmado,
                            ecfDocument.Encabezado?.IdDoc?.ENCF ?? "",
                            ecfDocument.Encabezado?.Emisor?.RNCEmisor ?? "",
                            tipoEcf,
                            _dgiiSettings.StoragePath);

                        rawResponse = await _dgiiApiService.SendEcfAsync(xmlFirmado, token);

                        try
                        {
                            var jsonDoc = System.Text.Json.JsonDocument.Parse(rawResponse);
                            if (jsonDoc.RootElement.TryGetProperty("trackId", out var trackIdElement))
                            {
                                trackId = trackIdElement.GetString() ?? string.Empty;

                                if (!string.IsNullOrEmpty(trackId) && Guid.TryParse(trackId, out _))
                                {
                                    try
                                    {
                                        await Task.Delay(4000); // Wait for DGII processing
                                        var statusResponse = await _dgiiApiService.CheckStatusAsync(trackId, token);
                                        if (!string.IsNullOrEmpty(statusResponse))
                                        {
                                            rawResponse = statusResponse; // Save the detailed response
                                            jsonDoc = System.Text.Json.JsonDocument.Parse(statusResponse);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        System.Diagnostics.Debug.WriteLine($"Error checking purchase trackId status: {ex.Message}");
                                    }
                                }
                            }
                            
                            if (jsonDoc.RootElement.TryGetProperty("estado", out var estadoElement))
                            {
                                status = estadoElement.GetString() ?? "Enviado";
                                if (status.Contains("Rechazado", StringComparison.OrdinalIgnoreCase))
                                    isAccepted = false;
                            }
                            else if (jsonDoc.RootElement.TryGetProperty("codigo", out var codigoElement))
                            {
                                int codigoInt = -1;
                                if (codigoElement.ValueKind == System.Text.Json.JsonValueKind.Number)
                                    codigoInt = codigoElement.GetInt32();
                                else if (codigoElement.ValueKind == System.Text.Json.JsonValueKind.String)
                                    int.TryParse(codigoElement.GetString(), out codigoInt);

                                if (codigoInt != 1 && codigoInt != 3 && codigoInt != 4 && codigoInt != 5)
                                    isAccepted = false;
                            }
                        }
                        catch
                        {
                            // If JSON parsing fails, keep default values
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"DGII Purchase Flow Error: {ex.Message}");
                    isAccepted = false;
                    status = "Error";
                    rawResponse = ex.Message;
                }

                return (isAccepted, trackId, status, securityCode, rawResponse);
            };

            var dto = await request.ExecuteAction(_unitOfWork, _outputCacheStore, _mapper, cancellationToken);
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
