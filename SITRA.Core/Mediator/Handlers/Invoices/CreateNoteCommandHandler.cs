using System.Text;
using System.Xml.Linq;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Options;
using SITRA.Core.DTOs.DGII;
using SITRA.Core.DTOs.Invoices.SaleInvoices;
using SITRA.Core.Interfaces;
using SITRA.Core.Mediator.Commands.Invoices.SaleInvoices;
using SITRA.Core.Models.Dgii;

namespace SITRA.Core.Mediator.Handlers.Invoices
{
    /// <summary>
    /// Handles the full DGII flow for Electronic Credit Notes (E34) and Debit Notes (E33).
    ///
    /// Key difference from CreateSaleInvoiceCommandHandler:
    ///   After the ECF document is built by <see cref="IInvoiceToEcfMapper"/>, we inject
    ///   the <InformacionReferencia> block (NCFModificado + CodigoModificacion) required
    ///   by the DGII specification for E33/E34 documents.
    /// </summary>
    public class CreateNoteCommandHandler : IRequestHandler<CreateNoteCommand, CreateNoteDTO>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOutputCacheStore _outputCacheStore;
        private readonly IMapper _mapper;
        private readonly IDgiiApiService _dgiiApiService;
        private readonly ISignatureService _signatureService;
        private readonly IEcfXmlGenerator _ecfXmlGenerator;
        private readonly IInvoiceToEcfMapper _mapperToEcf;
        private readonly DgiiSettings _dgiiSettings;

        public CreateNoteCommandHandler(
            IUnitOfWork unitOfWork,
            IOutputCacheStore outputCacheStore,
            IMapper mapper,
            IDgiiApiService dgiiApiService,
            ISignatureService signatureService,
            IEcfXmlGenerator ecfXmlGenerator,
            IInvoiceToEcfMapper mapperToEcf,
            IOptions<DgiiSettings> dgiiSettings)
        {
            _unitOfWork       = unitOfWork;
            _outputCacheStore = outputCacheStore;
            _mapper           = mapper;
            _dgiiApiService   = dgiiApiService;
            _signatureService = signatureService;
            _ecfXmlGenerator  = ecfXmlGenerator;
            _mapperToEcf      = mapperToEcf;
            _dgiiSettings     = dgiiSettings.Value;
        }

        public async Task<CreateNoteDTO> Handle(CreateNoteCommand request, CancellationToken cancellationToken)
        {
            request.DgiiProcess = async (invoice) =>
            {
                bool isAccepted   = true;
                string trackId    = "";
                string status     = "Enviado";
                string securityCode = "000000";
                string rawResponse  = "";

                try
                {
                    // ── 1. Build base ECF document from invoice ──────────────────────
                    var ecfDocument = _mapperToEcf.MapToDgiiDocument(invoice);

                    // ── 2. Inject InformacionReferencia (mandatory for E33 / E34) ────
                    if (!string.IsNullOrEmpty(invoice.ModifiedNcfNumber))
                    {
                        string refDate = DateTimeOffset.Now.ToString("dd-MM-yyyy");
                        var candidates = await _unitOfWork.SaleInvoiceRepository.GetAllAsync(i => i.FiscalSupportNumber == invoice.ModifiedNcfNumber);
                        var originalDoc = candidates.FirstOrDefault();
                        if (originalDoc != null)
                        {
                            refDate = originalDoc.Date.ToString("dd-MM-yyyy");
                        }

                        ecfDocument.InformacionReferencia = new InformacionReferencia
                        {
                            NCFModificado      = invoice.ModifiedNcfNumber,
                            CodigoModificacion = invoice.ModificationCode ?? 3,
                            FechaNCFModificado = refDate
                        };
                    }

                    // ── 3. Authenticate with DGII ────────────────────────────────────
                    string seedXml    = await _dgiiApiService.GetSemillaAsync();
                    string signedSeed = _signatureService.SignAuthenticationSeed(seedXml);
                    string token      = await _dgiiApiService.GetTokenAsync(signedSeed);

                    if (!string.IsNullOrEmpty(token))
                    {
                        // E33/E34 always go through the standard ECF channel (never RFCE)
                        string xmlSinFirma = _ecfXmlGenerator.GenerateEcfXml(ecfDocument);
                        string xmlFirmado  = _signatureService.SignXml(xmlSinFirma);

                        securityCode = GetSecurityCodeFromSignedXml(xmlFirmado);

                        var docType = ecfDocument.Encabezado?.IdDoc?.TipoeCF ?? "UNKNOWN";
                        SaveDocumentXml(
                            xmlFirmado,
                            ecfDocument.Encabezado?.IdDoc?.ENCF ?? "",
                            ecfDocument.Encabezado?.Emisor?.RNCEmisor ?? "",
                            docType,
                            _dgiiSettings.StoragePath);

                        rawResponse = await _dgiiApiService.SendEcfAsync(xmlFirmado, token);

                        trackId = string.Empty;

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
                                        await Task.Delay(4000);
                                        var statusResponse = await _dgiiApiService.CheckStatusAsync(trackId, token);
                                        if (!string.IsNullOrEmpty(statusResponse))
                                        {
                                            rawResponse = statusResponse;
                                            jsonDoc = System.Text.Json.JsonDocument.Parse(statusResponse);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        System.Diagnostics.Debug.WriteLine($"Error checking trackId status: {ex.Message}");
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
                            // Parsing failure — treat as accepted but unverified
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"DGII Note Flow Error: {ex.Message}");
                    isAccepted  = false;
                    status      = "Error";
                    rawResponse = ex.Message;
                }

                return (isAccepted, trackId, status, securityCode, rawResponse);
            };

            var dto = await request.ExecuteAction(_unitOfWork, _outputCacheStore, _mapper, cancellationToken);
            return dto;
        }

        // ── Helpers (identical to CreateSaleInvoiceCommandHandler) ─────────────────

        private static void SaveDocumentXml(string xml, string encf, string rnc, string docType, string? storagePath)
        {
            try
            {
                string baseDir = !string.IsNullOrEmpty(storagePath)
                    ? storagePath
                    : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "DgiiDocuments");

                string saveDir = Path.Combine(baseDir, rnc, docType);
                if (!Directory.Exists(saveDir)) Directory.CreateDirectory(saveDir);

                string fileName = $"{rnc}{encf}.xml";
                string filePath = Path.Combine(saveDir, fileName);
                File.WriteAllText(filePath, xml, new UTF8Encoding(false));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to save DGII Note XML: {ex.Message}");
            }
        }

        private static string GetSecurityCodeFromSignedXml(string signedXml)
        {
            try
            {
                var doc = XDocument.Parse(signedXml);
                XNamespace ds = "http://www.w3.org/2000/09/xmldsig#";
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
    }
}
